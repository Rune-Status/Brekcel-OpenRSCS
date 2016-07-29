using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRSCS.cache.resources.region {
    public class Position {
        public enum RegionSize {
            DEFAULT = 104,
            LARGE = 120,
            XLARGE = 136,
            XXLARGE = 168
        }
        private readonly RegionSize mapSize;
        private readonly int x;
        private readonly int y;
        private readonly int height;

        public Position(int x, int y, int height) : this(x, y, height, RegionSize.DEFAULT) {
        }

        public Position(int x, int y, int height, RegionSize mapSize) {
            this.x = x;
            this.y = y;
            this.height = height;
            this.mapSize = mapSize;
        }

        public Position(int localX, int localY, int height, int regionId, RegionSize mapSize) : this(localX + (((regionId >> 8) & 0xFF) << 6), localX + ((regionId & 0xff) << 6), height, mapSize) {
        }

        public int getXInRegion() {
            return x & 0x3F;
        }

        public int getYInRegion() {
            return y & 0x3F;
        }

        public int getLocalX() {
            return x - 8 * (getChunkX() - ((int)mapSize >> 4));
        }

        public int getLocalY() {
            return y - 8 * (getChunkY() - ((int)mapSize >> 4));
        }

        public int getLocalX(Position pos) {
            return x - 8 * (pos.getChunkX() - ((int)mapSize >> 4));
        }

        public int getLocalY(Position pos) {
            return y - 8 * (pos.getChunkY() - ((int)mapSize >> 4));
        }

        public int getChunkX() {
            return (x >> 3);
        }

        public int getChunkY() {
            return (y >> 3);
        }

        public int getRegionX() {
            return (x >> 6);
        }

        public int getRegionY() {
            return (y >> 6);
        }

        public int getRegionID() {
            return ((getRegionX() << 8) + getRegionY());
        }

        public int getX() {
            return x;
        }

        public int getY() {
            return y;
        }

        public int getHeight() {
            return height;
        }

        public RegionSize getMapSize() {
            return mapSize;
        }

        public int toRegionPacked() {
            return getRegionY() + (getRegionX() << 8) + (height << 16);
        }

        public int toPositionPacked() {
            return y + (x << 14) + (height << 28);
        }

        public Position toAbsolute() {
            int xOff = x % 8;
            int yOff = y % 8;
            return new Position(x - xOff, y - yOff, height);
        }

        public override string ToString() {
            return "X: " + getX() + ", Y: " + getY() + ", Height: " + getHeight();
        }
    }
}
