using OpenRSCS.utils;

namespace OpenRSCS.cache.resources.sprite {
    public class Sprite {

        /**
	 * This flag indicates that the pixels should be read vertically instead of
	 * horizontally.
	 */
        public const int FLAG_VERTICAL = 0x01;
        /**
         * This flag indicates that every pixel has an alpha, as well as red, green
         * and blue, component.
         */
        public const int FLAG_ALPHA = 0x02;
        /**
         * Decodes the {@link Sprite} from the specified {@link ByteBuffer}.
         * 
         * @param buffer
         *            The buffer.
         * @return The sprite.
         */

        public Sprite(ByteBuffer buffer) {
            buffer.Position = buffer.Length - 2;
            int size = buffer.getShort();
            int[] offsetsX = new int[size];
            int[] offsetsY = new int[size];
            int[] subWidths = new int[size];
            int[] subHeights = new int[size];
            buffer.Position = buffer.Length - size*8 - 7;
            width = buffer.getShort();
            height = buffer.getShort();
            int[] pallete = new int[buffer.get() + 1];
            for(int i = 0; i < size; i++) {
                offsetsX[i] = buffer.getShort();
            }
            for(int i = 0; i < size; i++) {
                offsetsY[i] = buffer.getShort();
            }
            for(int i = 0; i < size; i++) {
                subWidths[i] = buffer.getShort();
            }
            for(int i = 0; i < size; i++) {
                subHeights[i] = buffer.getShort();
            }

            buffer.Position = buffer.Length - size*8 - 7 - (pallete.Length - 1)*3;
            pallete[0] = 0;
            for(int i = 1; i < pallete.Length; i++) {
                pallete[i] = buffer.getMedium();
                if(pallete[i] == 0)
                    pallete[i] = 1;
            }

            rgbData = new int[size][,];

            buffer.Position = 0;
            for(int i = 0; i < size; i++) {
                int subWidth = subWidths[i];
                int subHeight = subHeights[i];
                int offsetX = offsetsX[i];
                int offsetY = offsetsY[i];
                int[,] indices = new int[subWidth, subHeight];
                rgbData[i] = new int[width, height];
                int flags = buffer.get();
                if((flags & FLAG_VERTICAL) != 0) {
                    for(int x = 0; x < subWidth; x++) {
                        for(int y = 0; y < subHeight; y++) {
                            indices[x, y] = buffer.get();
                        }
                    }
                } else {
                    for(int y = 0; y < subHeight; y++) {
                        for(int x = 0; x < subWidth; x++) {
                            indices[x, y] = buffer.get();
                        }
                    }
                }

                if((flags & FLAG_ALPHA) != 0) {
                    if((flags & FLAG_VERTICAL) != 0) {
                        for(int x = 0; x < subWidth; x++) {
                            for(int y = 0; y < subHeight; y++) {
                                int alpha = buffer.get();
                                rgbData[i][x + offsetX, y + offsetY] = alpha << 24 | pallete[indices[x, y]];
                            }
                        }
                    } else {
                        for(int y = 0; y < subHeight; y++) {
                            for(int x = 0; x < subWidth; x++) {
                                int alpha = buffer.get();
                                rgbData[i][x + offsetX, y + offsetY] = alpha << 24 | pallete[indices[x, y]];
                            }
                        }
                    }
                } else {
                    for(int x = 0; x < subWidth; x++) {
                        for(int y = 0; y < subHeight; y++) {
                            int index = indices[x, y];
                            if(index == 0) {
                                rgbData[i][x + offsetX, y + offsetY] = 0;
                            } else {
                                int test = (int)(pallete[index] | 0xff000000);
                                rgbData[i][x + offsetX, y + offsetY] = test;
                            }
                        }
                    }
                }
            }
        }

        private readonly int[][,] rgbData;
        /**
	 * The width of this sprite.
	 */
        private readonly int width;
        /**
         * The height of this sprite.
         */
        private readonly int height;

        public int[,] getData(int height) {
            return rgbData[height];
        }

        public int size() {
            return rgbData.Length;
        }

        public int getData(int x, int y, int z) {
            return rgbData[z][x, y];
        }

        /**
	 * Gets the height of this sprite.
	 * 
	 * @return The height of this sprite.
	 */

        public int getHeight() {
            return height;
        }

        /**
         * Gets the width of this sprite.
         * 
         * @return The width of this sprite.
         */

        public int getWidth() {
            return width;
        }

    }
}
