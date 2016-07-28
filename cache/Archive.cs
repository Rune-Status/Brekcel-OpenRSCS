using OpenRSCS.utils;

namespace OpenRSCS.cache {
    public class Archive {

        /**
         * The array of entries in this archive.
         */
        private readonly ByteBuffer[] entries;
        /**
         * Creates a new archive.
         * 
         * @param size
         *            The number of entries in the archive.
         */

        public Archive(int size) {
            entries = new ByteBuffer[size];
        }

        /**
         * Decodes the specified {@link ByteBuffer} into an {@link Archive}.
         * 
         * @param buffer
         *            The buffer.
         * @param size
         *            The size of the archive.
         * @return The decoded {@link Archive}.
         */

        public Archive(ByteBuffer buffer, int size) : this(size) {
            buffer.Position = buffer.Length - 1;
            int chunks = buffer.get();
            int[][] chunkSizes = new int[chunks][];
            for(int i = 0; i < chunkSizes.Length; i++)
                chunkSizes[i] = new int[size];

            int[] sizes = new int[size];
            buffer.Position = buffer.Length - 1 - chunks*size*4;
            for(int chunk = 0; chunk < chunks; chunk++) {
                int chunkSize = 0;
                for(int id = 0; id < size; id++) {
                    int delta = buffer.getInt();
                    chunkSize += delta;
                    chunkSizes[chunk][id] = chunkSize;
                    sizes[id] += chunkSize;
                }
            }
            for(int id = 0; id < size; id++)
                entries[id] = new ByteBuffer(sizes[id]);

            buffer.Position = 0;
            for(int chunk = 0; chunk < chunks; chunk++) {
                for(int id = 0; id < size; id++) {
                    int chunkSize = chunkSizes[chunk][id];
                    byte[] temp = new byte[chunkSize];
                    buffer.getBytes(temp);
                    entries[id].put(temp);
                }
            }
            for(int id = 0; id < size; id++)
                entries[id].flip();
        }

        /**
         * Gets the entry with the specified id.
         * 
         * @param id
         *            The id.
         * @return The entry.
         */

        public ByteBuffer getEntry(int id) {
            return entries[id];
        }

        /**
         * Inserts/replaces the entry with the specified id.
         * 
         * @param id
         *            The id.
         * @param buffer
         *            The entry.
         */

        public void putEntry(int id, ByteBuffer buffer) {
            entries[id] = buffer;
        }

        /**
         * Gets the size of this archive.
         * 
         * @return The size of this archive.
         */

        public int size() {
            return entries.Length;
        }

    }
}
