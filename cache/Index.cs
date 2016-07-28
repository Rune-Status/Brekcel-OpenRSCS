using System;
using OpenRSCS.utils;

namespace OpenRSCS.cache {
    public class Index {

        /**
         * The size of an index, in bytes.
         */
        public const int SIZE = 6;
        /**
         * The number of the first sector that contains the file.
         */
        private readonly int sector;
        /**
         * The size of the file in bytes.
         */
        private readonly int size;

        public Index(ByteBuffer buf) {
            if(buf.remaining() != SIZE)
                throw new ArgumentException();

            size = buf.getMedium();
            sector = buf.getMedium();
        }

        /**
        * Gets the number of the first sector that contains the file.
        * 
        * @return The number of the first sector that contains the file.
        */

        public int getSector() {
            return sector;
        }

        /**
         * Gets the size of the file.
         * 
         * @return The size of the file in bytes.
         */

        public int getSize() {
            return size;
        }

    }
}
