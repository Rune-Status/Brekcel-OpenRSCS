using System;
using System.Collections.Generic;
using System.Linq;
using OpenRSCS.cache.enums;
using OpenRSCS.utils;

namespace OpenRSCS.cache {
    public class ReferenceTable {

        /**
         * A flag which indicates this {@link ReferenceTable} contains {@link BKDR}
         * hashed identifiers.
         */
        public const int FLAG_IDENTIFIERS = 0x01;
        /**
         * A flag which indicates this {@link ReferenceTable} contains whirlpool
         * digests for its entries.
         */
        public const int FLAG_WHIRLPOOL = 0x02;
        /**
         * A flag which indicates this {@link ReferenceTable} contains compression
         * sizes.
         */
        public const int FLAG_SIZES = 0x04;
        /**
         * A flag which indicates this {@link ReferenceTable} contains a type of
         * hash.
         */
        public const int FLAG_HASH = 0x08;
        /**
         * The entries in this table.
         */
        public SortedDictionary<int, Entry> entries = new SortedDictionary<int, Entry>();
        /**
         * The flags of this table.
         */
        private int flags;
        /**
         * The format of this table.
         */
        private int format;
        /**
         * The version of this table.
         */
        private int version;
        /**
         * Decodes the slave checksum table contained in the specified
         * {@link ByteBuffer}.
         * 
         * @param buffer
         *            The buffer.
         * @return The slave checksum table.
         */

        public ReferenceTable(ByteBuffer buf) {
            format = buf.get();
            if(format < 5 || format > 7)
                throw new Exception();

            if(format >= 6)
                version = buf.getInt();
            flags = buf.get();
            int[] ids = new int[format >= 7 ? buf.getSmartInt() : buf.getUShort()];
            int acum = 0, size = -1;
            for(int i = 0; i < ids.Length; i++) {
                int delta = format >= 7 ? buf.getSmartInt() : buf.getUShort();
                ids[i] = acum += delta;
                if(ids[i] > size)
                    size = ids[i];
            }

            size++;
            int index = 0;
            foreach(int id in ids) {
                entries.Add(id, new Entry(index++));
            }

            if((flags & FLAG_IDENTIFIERS) != 0) {
                foreach(int id in ids)
                    entries[id].setIdentifier(buf.getInt());
            }

            foreach(int id in ids) {
                entries[id].setCrc(buf.getInt());
            }

            if((flags & FLAG_HASH) != 0) {
                foreach(int id in ids) {
                    entries[id].setHash(buf.getInt());
                }
            }

            if((flags & FLAG_WHIRLPOOL) != 0) {
                foreach(int id in ids) {
                    buf.getBytes(entries[id].whirlpool);
                }
            }

            if((flags & FLAG_SIZES) != 0) {
                foreach(int id in ids) {
                    entries[id].compressed = buf.getInt();
                    entries[id].uncompressed = buf.getInt();
                }
            }

            foreach(int id in ids) {
                entries[id].setVersion(buf.getInt());
            }

            int[][] members = new int[size][];
            foreach(int id in ids) {
                int test = format >= 7 ? buf.getSmartInt() : buf.getShort();
                members[id] = new int[test];
            }
            foreach(int id in ids) {
                acum = 0;
                size = -1;
                for(int i = 0; i < members[id].Length; i++) {
                    int delta = format >= 7 ? buf.getSmartInt() : buf.getUShort();
                    members[id][i] = acum += delta;
                    if(members[id][i] > size) {
                        size = members[id][i];
                    }
                }

                size++;
                index = 0;
                foreach(int child in members[id])
                    entries[id].putEntry(child, new ChildEntry(index++));
            }

            if((flags & FLAG_IDENTIFIERS) != 0) {
                foreach(int id in ids) {
                    foreach(int child in members[id])
                        entries[id].getEntry(child).setIdentifier(buf.getInt());
                }
            }
        }

        /**
         * Gets the maximum number of entries in this table.
         * 
         * @return The maximum number of entries.
         */

        public int capacity() {
            if(entries.Count == 0)
                return 0;

            return entries.Keys.Last() + 1;
        }

        /**
         * Gets the entry with the specified id, or {@code null} if it does not
         * exist.
         * 
         * @param id
         *            The id.
         * @return The entry.
         */

        public Entry getEntry(int id) {
            return entries[id];
        }

        /**
         * Gets the entry with the specified id, or {@code null} if it does not
         * exist.
         * 
         * @param id
         *            The id.
         * @return The entry.
         */

        public Entry getEntry(ConfigArchive archive) {
            return getEntry((int)archive);
        }

        /**
         * Gets the child entry with the specified id, or {@code null} if it does
         * not exist.
         * 
         * @param id
         *            The parent id.
         * @param child
         *            The child id.
         * @return The entry.
         */

        public ChildEntry getEntry(int id, int child) {
            Entry entry = entries[id];
            if(entry == null)
                return null;

            return entry.getEntry(child);
        }

        /**
         * Gets the flags of this table.
         * 
         * @return The flags.
         */

        public int getFlags() {
            return flags;
        }

        /**
         * Gets the format of this table.
         * 
         * @return The format.
         */

        public int getFormat() {
            return format;
        }

        /**
         * Gets the version of this table.
         * 
         * @return The version of this table.
         */

        public int getVersion() {
            return version;
        }

        /**
         * Replaces or inserts the entry with the specified id.
         * 
         * @param id
         *            The id.
         * @param entry
         *            The entry.
         */

        public void putEntry(int id, Entry entry) {
            entries.Add(id, entry);
        }

        /**
         * Removes the entry with the specified id.
         * 
         * @param id
         *            The id.
         */

        public void removeEntry(int id) {
            entries.Remove(id);
        }

        /**
         * Sets the flags of this table.
         * 
         * @param flags
         *            The flags.
         */

        public void setFlags(int flags) {
            this.flags = flags;
        }

        /**
         * Sets the format of this table.
         * 
         * @param format
         *            The format.
         */

        public void setFormat(int format) {
            this.format = format;
        }

        /**
         * Sets the version of this table.
         * 
         * @param version
         *            The version.
         */

        public void setVersion(int version) {
            this.version = version;
        }

        /**
         * Gets the number of actual entries.
         * 
         * @return The number of actual entries.
         */

        public int size() {
            return entries.Count;
        }

        /**
         * Gets the uncompressed size of the index
         * 
         * @return The size
         */

        public int getArchiveSize() {
            long sum = 0;
            for(int i = 0; i < capacity(); i++) {
                Entry e = entries[i];
                if(e != null) {
                    sum += e.getUncompressed();
                }
            }

            return (int)sum;
        }

        /**
         * Represents a child entry within an {@link Entry} in the
         * {@link ReferenceTable}.
         * 
         * @author Graham Edgecombe
         */

        public class ChildEntry {

            /**
             * The cache index of this entry
             */
            private readonly int index;
            /**
             * This entry's identifier.
             */
            private int identifier = -1;

            public ChildEntry(int index) {
                this.index = index;
            }

            /**
             * Gets the cache index for this child entry
             * 
             * @return The cache index
             */

            public int getIndex() {
                return index;
            }

            /**
             * Gets the identifier of this entry.
             * 
             * @return The identifier.
             */

            public int getIdentifier() {
                return identifier;
            }

            /**
             * Sets the identifier of this entry.
             * 
             * @param identifier
             *            The identifier.
             */

            public void setIdentifier(int identifier) {
                this.identifier = identifier;
            }

        }

        /**
         * Represents a single entry within a {@link ReferenceTable}.
         * 
         * @author Graham Edgecombe
         */

        public class Entry {

            /**
             * The children in this entry.
             */
            private readonly SortedDictionary<int, ChildEntry> entries = new SortedDictionary<int, ChildEntry>();
            /**
             * The cache index of this entry
             */
            private readonly int index;
            /**
             * The compressed size of this entry.
             */
            public int compressed;
            /**
             * The CRC32 checksum of this entry.
             */
            private int crc;
            /**
             * The hash of this entry
             */
            public int hash;
            /**
             * The identifier of this entry.
             */
            private int identifier = -1;
            /**
             * The uncompressed size of this entry.
             */
            public int uncompressed;
            /**
             * The version of this entry.
             */
            private int version;
            /**
             * The whirlpool digest of this entry.
             */
            public byte[] whirlpool = new byte[64];

            public Entry(int index) {
                this.index = index;
            }

            /**
             * Gets the cache index for this entry
             * 
             * @return The cache index
             */

            public int getIndex() {
                return index;
            }

            /**
             * Gets the maximum number of child entries.
             * 
             * @return The maximum number of child entries.
             */

            public int capacity() {
                if(entries.Count == 0)
                    return 0;

                return entries.Keys.Last() + 1;
            }

            /**
             * Gets the CRC32 checksum of this entry.
             * 
             * @return The CRC32 checksum.
             */

            public int getCrc() {
                return crc;
            }

            /**
             * Gets the child entry with the specified id.
             * 
             * @param id
             *            The id.
             * @return The entry, or {@code null} if it does not exist.
             */

            public ChildEntry getEntry(int id) {
                return entries[id];
            }

            /**
             * Gets the identifier of this entry.
             * 
             * @return The identifier.
             */

            public int getIdentifier() {
                return identifier;
            }

            /**
             * Gets the version of this entry.
             * 
             * @return The version.
             */

            public int getVersion() {
                return version;
            }

            /**
             * Gets the uncompressed size of this entry.
             * 
             * @return The uncompressed size.
             */

            public int getUncompressed() {
                return uncompressed;
            }

            /**
             * Gets the compressed size of this entry.
             * 
             * @return The compressed size.
             */

            public int getCompressed() {
                return compressed;
            }

            /**
             * Gets the hash this entry
             * 
             * @return The hash
             */

            public int getHash() {
                return hash;
            }

            /**
             * Gets the whirlpool digest of this entry.
             * 
             * @return The whirlpool digest.
             */

            public byte[] getWhirlpool() {
                return whirlpool;
            }

            /**
             * Replaces or inserts the child entry with the specified id.
             * 
             * @param id
             *            The id.
             * @param entry
             *            The entry.
             */

            public void putEntry(int id, ChildEntry entry) {
                entries.Add(id, entry);
            }

            /**
             * Removes the entry with the specified id.
             * 
             * @param id
             *            The id.
             */

            public void removeEntry(int id) {
                entries.Remove(id);
            }

            /**
             * Sets the CRC32 checksum of this entry.
             * 
             * @param crc
             *            The CRC32 checksum.
             */

            public void setCrc(int crc) {
                this.crc = crc;
            }

            /**
             * Sets the hash of this entry.
             * 
             * @param hash
             *            The hash.
             */

            public void setHash(int hash) {
                this.hash = hash;
            }

            /**
             * Sets the identifier of this entry.
             * 
             * @param identifier
             *            The identifier.
             */

            public void setIdentifier(int identifier) {
                this.identifier = identifier;
            }

            /**
             * Sets the version of this entry.
             * 
             * @param version
             *            The version.
             */

            public void setVersion(int version) {
                this.version = version;
            }

            /**
             * Sets the whirlpool digest of this entry.
             * 
             * @param whirlpool
             *            The whirlpool digest.
             * @throws IllegalArgumentException
             *             if the digest is not 64 bytes long.
             */

            public void setWhirlpool(byte[] whirlpool) {
                if(whirlpool.Length != 64)
                    throw new ArgumentException();

                Array.Copy(whirlpool, 0, this.whirlpool, 0, whirlpool.Length);
            }

            /**
             * Gets the number of actual child entries.
             * 
             * @return The number of actual child entries.
             */

            public int size() {
                return entries.Count;
            }

        }

    }
}
