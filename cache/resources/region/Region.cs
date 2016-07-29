using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRSCS.utils;

namespace OpenRSCS.cache.resources.region {
    public class Region {
        public readonly int regionID;
        public readonly int baseX;
        public readonly int baseY;
        public readonly int[,,] tileHeights = new int[4,104,104];
        public readonly byte[,,] renderRules = new byte[4,104,104];
        public readonly byte[,,] overlayIds = new byte[4,104,104];
        public readonly byte[,,] overlayPaths = new byte[4,104,104];
        public readonly byte[,,] overlayRotations = new byte[4,104,104];
        public readonly byte[,,] underlayIds = new byte[4,104,104];
        public readonly List<Location> locations = new List<Location>();

        public Region(int id) {
            this.regionID = id;
            this.baseX = (id >> 8 & 0xFF) << 6;
            this.baseY = (id & 0xFF) << 6;
        }

        /**
         * Decodes terrain data stored in the specified {@link ByteBuffer}.
         *
         * @param buffer
         *            The ByteBuffer.
         */
        public void loadTerrain(ByteBuffer buf) {
            for (int z = 0; z < 4; z++) {
                for (int x = 0; x < 64; x++) {
                    for (int y = 0; y < 64; y++) {
                        while (true) {
                            int attribute = buf.get() & 0xFF;
                            if (attribute == 0) {
                                if (z == 0) {
                                    // TODO Verify the height calculation was
                                    // correctly ripped from client
                                    tileHeights[0,x,y] = HeightCalc.calculate(baseX, baseY, x, y) << 3;
                                } else
                                    tileHeights[z,x,y] = tileHeights[z - 1,x,y] - 240;
                                break;
                            } else if (attribute == 1) {
                                int height = buf.get() & 0xFF;
                                if (height == 1)
                                    height = 0;
                                if (z == 0)
                                    tileHeights[0,x,y] = -height << 3;
                                else
                                    tileHeights[z,x,y] = tileHeights[z - 1,x,y] - height << 3;
                                break;
                            } else if (attribute <= 49) {
                                overlayIds[z,x,y] = buf.get();
                                overlayPaths[z,x,y] = (byte)((attribute - 2) / 4);
                                overlayRotations[z,x,y] = (byte)(attribute - 2 & 0x3);
                            } else if (attribute <= 81) {
                                renderRules[z,x,y] = (byte)(attribute - 49);
                            } else {
                                underlayIds[z,x,y] = (byte)(attribute - 81);
                            }
                        }
                    }
                }
            }
        }

        /**
         * Decodes location data stored in the specified {@link ByteBuffer}.
         *
         * @param buffer
         *            The ByteBuffer.
         */
        public void loadLocations(ByteBuffer buf) {
            int id = -1;
            int idOffset;
            while ((idOffset = buf.getUnsignedSmart()) != 0) {
                id += idOffset;
                int position = 0;
                int positionOffset;
                while ((positionOffset = buf.getUnsignedSmart()) != 0) {
                    position += positionOffset - 1;
                    int localY = position & 0x3F;
                    int localX = position >> 6 & 0x3F;
                    int height = position >> 12 & 0x3;
                    int attributes = buf.get() & 0xFF;
                    int type = attributes >> 2;
                    int orientation = attributes & 0x3;
                    locations.Add(new Location(id, type, orientation, new Position(baseX + localX, baseY + localY, height)));
                }
            }
        }

    }
}
