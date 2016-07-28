using System;
using OpenRSCS.utils;

namespace OpenRSCS.cache {
    internal class Sector {

        /**
         * The size of the header within a sector in bytes.
         */
        public const int HEADER_SIZE = 8;
        /**
         * The size of the data within a sector in bytes.
         */
        public const int DATA_SIZE = 512;
        /**
         * The extended data size
         */
        public const int EXTENDED_DATA_SIZE = 510;
        /**
         * The extended header size
         */
        public const int EXTENDED_HEADER_SIZE = 10;
        /**
         * The total size of a sector in bytes.
         */
        public const int SIZE = HEADER_SIZE + DATA_SIZE;
        /**
         * The chunk within the file that this sector contains.
         */
        private readonly int chunk;
        /**
         * The data in this sector.
         */
        private readonly byte[] data;
        /**
         * The id of the file this sector contains.
         */
        private readonly int id;
        /**
         * The next sector.
         */
        private readonly int nextSector;
        /**
     * The type of file this sector contains.
     */
        private readonly int type;
        /**
      * Decodes the specified {@link ByteBuffer} into a {@link Sector} object.
      * 
      * @param buf
      *            The buffer.
      * @return The sector.
      */

        public Sector(ByteBuffer buf) : this(buf, false) { }

        /**
         * Decodes the specified {@link ByteBuffer} into a {@link Sector} object.
         * 
         * @param buf
         *            The buffer.
         * @return The sector.
         */

        public Sector(ByteBuffer buf, bool extended) {
            if(buf.remaining() != SIZE)
                throw new ArgumentException();

            id = buf.getShort();
            chunk = buf.getShort();
            nextSector = buf.getMedium();
            type = buf.get();
            if(extended)
                data = new byte[EXTENDED_DATA_SIZE];
            else
                data = new byte[DATA_SIZE];
            buf.getBytes(data);
        }

        /**
         * Gets the chunk of the file this sector contains.
         * 
         * @return The chunk of the file this sector contains.
         */

        public int getChunk() {
            return chunk;
        }

        /**
         * Gets this sector's data.
         * 
         * @return The data within this sector.
         */

        public byte[] getData() {
            return data;
        }

        /**
         * Gets the id of the file within this sector.
         * 
         * @return The id of the file in this sector.
         */

        public int getId() {
            return id;
        }

        /**
         * Gets the next sector.
         * 
         * @return The next sector.
         */

        public int getNextSector() {
            return nextSector;
        }

        /**
         * Gets the type of file in this sector.
         * 
         * @return The type of file in this sector.
         */

        public int getType() {
            return type;
        }

    }
}
