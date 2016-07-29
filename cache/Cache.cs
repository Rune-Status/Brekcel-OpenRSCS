using System;
using System.Collections.Generic;
using System.IO;
using OpenRSCS.cache.enums;
using OpenRSCS.utils;
using OpenRSCS.utils.crypto;

namespace OpenRSCS.cache {
    public class Cache : IDisposable {

        /**
         * The list of reference tables for this cache
         */
        private readonly Dictionary<int, ReferenceTable> references;
        /**
         * The file store that backs this cache.
         */
        private readonly FileStore store;
        private bool disposed;

        public Cache(FileStore fs) {
            store = fs;
            references = new Dictionary<int, ReferenceTable>(store.getTypeCount());
            for(int type = 0; type < store.getFileCount(255); type++) {
                ByteBuffer buf = store.read((int)CacheIndex.REFERENCE, type);
                if(buf != null && buf.Length > 0)
                    references.Add(type, new ReferenceTable(new Container(buf).getData()));
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ReferenceTable getReferenceTable(int type) {
            return references[type];
        }

        public bool hasReferenceTable(int type) {
            return references.ContainsKey(type);
        }

        public ChecksumTable createChecksumTable() {
            int size = store.getTypeCount();
            ChecksumTable table = new ChecksumTable(size);
            for(int i = 0; i < size; i++) {
                int crc = 0;
                int version = 0;
                int files = 0;
                int archiveSize = 0;
                byte[] whirlpool = new byte[64];
                if(store.hasData()) {
                    ByteBuffer buf = store.read((int)CacheIndex.REFERENCE, i);
                    if(buf != null && buf.Length > 0) {
                        ReferenceTable refTable = new ReferenceTable(new Container(buf).getData());
                        crc = CryptoUtils.getCrcChecksum(buf);
                        version = refTable.getVersion();
                        files = refTable.capacity();
                        archiveSize = refTable.getArchiveSize();
                        buf.Position = 0;
                        whirlpool = CryptoUtils.whirlpool(buf);
                    }
                }
                table.setEntry(i, new ChecksumTable.Entry(crc, version, files, archiveSize, whirlpool));
            }

            return table;
        }

        /**
         * Gets the number of files of the specified type.
         * 
         * @param type
         *            The type.
         * @return The number of files.
         * @throws IOException
         *             if an I/O error occurs.
         */

        public int getFileCount(int type) {
            return store.getFileCount(type);
        }

        /**
         * Gets the {@link FileStore} that backs this {@link Cache}.
         * 
         * @return The underlying file store.
         */

        public FileStore getStore() {
            return store;
        }

        /**
         * Gets the number of index files, not including the meta index file.
         * 
         * @return The number of index files.
         * @throws IOException
         *             if an I/O error occurs.
         */

        public int getTypeCount() {
            return store.getTypeCount();
        }

        /**
         * Reads a file from the cache.
         * 
         * @param type
         *            The type of file.
         * @param file
         *            The file id.
         * @return The file.
         * @throws IOException
         *             if an I/O error occurred.
         */

        public Container read(CacheIndex index, ConfigArchive archive) {
            return read((int)index, (int)archive);
        }

        /**
         * Reads a file from the cache.
         * 
         * @param type
         *            The type of file.
         * @param file
         *            The file id.
         * @return The file.
         * @throws IOException
         *             if an I/O error occurred.
         */

        public Container read(CacheIndex index, int file) {
            return read((int)index, file);
        }

        /**
         * Reads a file from the cache.
         * 
         * @param type
         *            The type of file.
         * @param file
         *            The file id.
         * @return The file.
         * @throws IOException
         *             if an I/O error occurred.
         */

        public Container read(int type, int file) {
            return read(type, file, null);
        }

        /**
         * Reads a file from the cache.
         * 
         * @param type
         *            The type of file.
         * @param file
         *            The file id.
         * @param keys
         *            The decryption keys.
         * @return The file.
         * @throws IOException
         *             if an I/O error occurred.
         */

        public Container read(int type, int file, int[] keys) {
            /* we don't want people reading/manipulating these manually */
            if(type == 255)
                throw new IOException("Reference tables can only be read with the low level FileStore API!");

            return new Container(store.read(type, file), keys);
        }

        /**
         * Reads a file contained in an archive in the cache.
         * 
         * @param type
         *            The type of the file.
         * @param file
         *            The archive id.
         * @param file
         *            The file within the archive.
         * @return The file.
         * @throws IOException
         *             if an I/O error occurred.
         */

        public ByteBuffer read(int type, int file, int member) {
            /* grab the container and the reference table */
            Container container = read(type, file);
            Container tableContainer = new Container(store.read(255, type));
            ReferenceTable table = new ReferenceTable(tableContainer.getData());
            /* check if the file/member are valid */
            ReferenceTable.Entry entry = table.getEntry(file);
            if(entry == null || member < 0 || member >= entry.capacity())
                throw new FileNotFoundException();
            /* extract the entry from the archive */
            Archive archive = new Archive(container.getData(), entry.capacity());
            return archive.getEntry(member);
        }

        /**
         * Gets a file id from the cache by name
         * 
         * @param type
         *            The type of file.
         * @param name
         *            The name of the file
         * @return The file id.
         * @throws java.io.IOException
         */

        public int getFileId(int type, string name) {
            int identifier = CryptoUtils.djb2(name);
            Container tableContainer = new Container(store.read(255, type));
            ReferenceTable table = new ReferenceTable(tableContainer.getData());
            for(int id = 0; id <= table.capacity(); id++) {
                ReferenceTable.Entry e = table.getEntry(id);
                if(e == null) {
                    continue;
                }

                if(e.getIdentifier() == identifier) {
                    return id;
                }
            }

            return -1;
        }

        public virtual void Dispose(bool disposing) {
            if(disposed)
                return;

            store.Dispose();
            disposed = true;
        }

    }
}
