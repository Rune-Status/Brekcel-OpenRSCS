using System;
using System.IO;
using OpenRSCS.cache.utils;
using OpenRSCS.utils;

namespace OpenRSCS.cache {
    public class Container {

        /**
         * This type indicates that no compression is used.
         */
        public const int COMPRESSION_NONE = 0;
        /**
         * This type indicates that BZIP2 compression is used.
         */
        public const int COMPRESSION_BZIP2 = 1;
        /**
         * This type indicates that GZIP compression is used.
         */
        public const int COMPRESSION_GZIP = 2;
        /**
         * The decompressed data.
         */
        private readonly ByteBuffer data;
        /**
     * The type of compression this container uses.
     */
        private int type;
        /**
         * The version of the file within this container.
         */
        private int version;

        public Container(ByteBuffer buffer) : this(buffer, XTEAManager.NULL_KEYS) { }

        public Container(ByteBuffer buffer, int[] keys) {
            type = buffer.get();
            int length = buffer.getInt();
            //decrypt 
            /*if (keys[0] != 0 || keys[1] != 0 || keys[2] != 0 || keys[3] != 0) {
                Xtea.decipher(buffer, 5, length + (type == COMPRESSION_NONE ? 5 : 9), keys);
            }*/
            if(type == COMPRESSION_NONE) {
                data = new ByteBuffer(buffer, (int)buffer.Position, length);
                version = -1;
                if(buffer.remaining() >= 2) {
                    version = buffer.getShort();
                }
            } else {
                int uncompressedLength = buffer.getInt();
                byte[] compressed = new byte[length];
                buffer.getBytes(compressed);
                if(type == COMPRESSION_BZIP2)
                    data = CompressionUtils.decompressBZIP2(compressed, uncompressedLength);
                else
                    if(type == COMPRESSION_GZIP)
                        data = CompressionUtils.decompressGZIP(compressed, uncompressedLength);
                    else
                        throw new IOException("Invalid compression type.");

                if(data.Length != uncompressedLength)
                    throw new IOException("Length mismatch. [ " + data.Length + ", " + uncompressedLength + " ] - " + type);

                version = -1;
                if(buffer.remaining() >= 2) {
                    version = buffer.getShort();
                }
            }
        }

        /**
         * Gets the decompressed data.
         * 
         * @return The decompressed data.
         */

        public ByteBuffer getData() {
            return data;
        }

        /**
         * Gets the type of this container.
         * 
         * @return The compression type.
         */

        public int getType() {
            return type;
        }

        /**
         * Gets the version of the file in this container.
         * 
         * @return The version of the file.
         * @throws IllegalArgumentException
         *             if this container is not versioned.
         */

        public int getVersion() {
            if(!isVersioned())
                throw new Exception();

            return version;
        }

        /**
         * Checks if this container is versioned.
         * 
         * @return {@code true} if so, {@code false} if not.
         */

        public bool isVersioned() {
            return version != -1;
        }

        /**
         * Removes the version on this container so it becomes unversioned.
         */

        public void removeVersion() {
            version = -1;
        }

        /**
         * Sets the type of this container.
         * 
         * @param type
         *            The compression type.
         */

        public void setType(int type) {
            this.type = type;
        }

        /**
         * Sets the version of this container.
         * 
         * @param version
         *            The version.
         */

        public void setVersion(int version) {
            this.version = version;
        }

    }
}
