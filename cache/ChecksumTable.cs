using System;
using System.IO;
using OpenRSCS.utils;
using OpenRSCS.utils.crypto;
using Org.BouncyCastle.Math;

namespace OpenRSCS.cache {
    public class ChecksumTable {

        /**
         * The entries in this table.
         */
        private readonly Entry[] entries;
        /**
         * Creates a new {@link ChecksumTable} with the specified size.
         * 
         * @param size
         *            The number of entries in this table.
         */

        public ChecksumTable(int size) {
            entries = new Entry[size];
        }

        /**
         * Decodes the {@link ChecksumTable} in the specified {@link ByteBuffer}.
         * Whirlpool digests are not read.
         * 
         * @param buffer
         *            The {@link ByteBuffer} containing the table.
         * @return The decoded {@link ChecksumTable}.
         * @throws IOException
         *             if an I/O error occurs.
         */

        public ChecksumTable(ByteBuffer buf) : this(buf, false) { }

        /**
         * Decodes the {@link ChecksumTable} in the specified {@link ByteBuffer}.
         * 
         * @param buffer
         *            The {@link ByteBuffer} containing the table.
         * @param whirlpool
         *            If whirlpool digests should be read.
         * @return The decoded {@link ChecksumTable}.
         * @throws IOException
         *             if an I/O error occurs.
         */

        public ChecksumTable(ByteBuffer buf, bool whirlpool) : this(buf, whirlpool, null, null) { }

        /**
         * Decodes the {@link ChecksumTable} in the specified {@link ByteBuffer} and
         * decrypts the final whirlpool hash.
         * 
         * @param buffer
         *            The {@link ByteBuffer} containing the table.
         * @param whirlpool
         *            If whirlpool digests should be read.
         * @param modulus
         *            The modulus.
         * @param publicKey
         *            The public key.
         * @return The decoded {@link ChecksumTable}.
         * @throws IOException
         *             if an I/O error occurs.
         */

        public ChecksumTable(ByteBuffer buf, bool whirlpool, BigInteger modulus, BigInteger publicKey) {
            int size = (int)(whirlpool ? buf.get() : buf.Length/8);
            entries = new Entry[size];
            byte[] masterDigest = null;
            if(whirlpool) {
                byte[] temp = new byte[size*80 + 1];
                buf.Position = 0;
                buf.getBytes(temp);
                masterDigest = CryptoUtils.whirlpool(temp);
            }
            buf.Position = whirlpool ? 1 : 0;
            for(int i = 0; i < size; i++) {
                int crc = buf.getInt();
                int version = buf.getInt();
                int files = whirlpool ? buf.getInt() : 0;
                int archiveSize = whirlpool ? buf.getInt() : 0;
                byte[] digest = new byte[64];
                if(whirlpool)
                    buf.getBytes(digest);
                entries[i] = new Entry(crc, version, files, archiveSize, digest);
            }
            //test
            if(whirlpool) {
                byte[] bytes = new byte[buf.remaining()];
                buf.getBytes(bytes);
                byte[] temp = new byte[bytes.Length];
                if(modulus != null && publicKey != null) {
                    temp = CryptoUtils.RSA(bytes, modulus, publicKey);
                }
                if(temp.Length != 65)
                    throw new IOException("Decrypted data is not 65 bytes long");

                for(int i = 0; i < 64; i++) {
                    if(temp[i + 1] != masterDigest[i])
                        throw new IOException("Whrilpool digest mismatch");
                }
            }
        }

        /**
         * Gets the size of this table.
         * 
         * @return The size of this table.
         */

        public int getSize() {
            return entries.Length;
        }

        /**
         * Sets an entry in this table.
         * 
         * @param id
         *            The id.
         * @param entry
         *            The entry.
         * @throws IndexOutOfBoundsException
         *             if the id is less than zero or greater than or equal to the
         *             size of the table.
         */

        public void setEntry(int id, Entry entry) {
            if(id < 0 || id >= entries.Length)
                throw new ArgumentException("Index out of bounds.");

            entries[id] = entry;
        }

        /**
         * Gets an entry from this table.
         * 
         * @param id
         *            The id.
         * @return The entry.
         * @throws IndexOutOfBoundsException
         *             if the id is less than zero or greater than or equal to the
         *             size of the table.
         */

        public Entry getEntry(int id) {
            if(id < 0 || id >= entries.Length)
                throw new ArgumentException("Index out of bounds.");

            return entries[id];
        }

        /**
         * Represents a single entry in a {@link ChecksumTable}. Each entry contains
         * a CRC32 checksum and version of the corresponding {@link ReferenceTable}.
         * 
         * @author Graham Edgecombe
         */

        public class Entry {

            /**
             * The CRC32 checksum of the reference table.
             */
            private readonly int crc;
            /**
             * The number of files contained within the index.
             */
            private readonly int files;
            /**
             * The total size of the archive
             */
            private readonly int size;
            /**
             * The version of the reference table.
             */
            private readonly int version;
            /**
             * The whirlpool digest of the reference table.
             */
            private readonly byte[] whirlpool;
            /**
             * Creates a new entry.
             * 
             * @param crc
             *            The CRC32 checksum of the slave table.
             * @param version
             *            The version of the slave table.
             * @param whirlpool
             *            The whirlpool digest of the reference table.
             */

            public Entry(int crc, int version, int fileCount, int size, byte[] whirlpool) {
                if(whirlpool.Length != 64)
                    throw new ArgumentException();

                this.crc = crc;
                this.version = version;
                files = fileCount;
                this.size = size;
                this.whirlpool = whirlpool;
            }

            /**
             * Gets the CRC32 checksum of the reference table.
             * 
             * @return The CRC32 checksum.
             */

            public int getCrc() {
                return crc;
            }

            /**
             * Gets the version of the reference table.
             * 
             * @return The version.
             */

            public int getVersion() {
                return version;
            }

            /**
             * Gets the number of files stored within the index this table
             * references.
             * 
             * @return The number of files.
             */

            public int getFileCount() {
                return files;
            }

            /**
             * Gets the total size of this archive.
             * 
             * @return The size of the archive.
             */

            public int getSize() {
                return size;
            }

            /**
             * Gets the whirlpool digest of the reference table.
             * 
             * @return The whirlpool digest.
             */

            public byte[] getWhirlpool() {
                return whirlpool;
            }

        }

    }
}
