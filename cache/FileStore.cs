using System;
using System.Collections;
using System.IO;
using OpenRSCS.cache.enums;
using OpenRSCS.utils;

namespace OpenRSCS.cache {
    public class FileStore : IDisposable {

        private readonly BinaryReader dataReader;
        private readonly BinaryReader[] indexReaders;
        private readonly BinaryReader metaReader;
        private bool disposed;
        /**
        * Opens the file store stored in the specified directory.
        * 
        * @param root
        *            The directory containing the index and data files.
        * @return The file store.
        * @throws FileNotFoundException
        *             if any of the {@code main_file_cache.*} files could not be
        *             found.
        */

        public FileStore(string root) {
            string data = Path.Combine(root, "main_file_cache.dat2");
            if(!File.Exists(data))
                throw new FileNotFoundException();

            dataReader = new BinaryReader(File.OpenRead(data));
            ArrayList indexReaders = new ArrayList();
            for(int i = 0; i < 254; i++) {
                string index = Path.Combine(root, "main_file_cache.idx" + i);
                if(!File.Exists(index))
                    break;

                indexReaders.Add(new BinaryReader(File.OpenRead(index)));
            }

            if(indexReaders.Count == 0)
                throw new FileNotFoundException();

            this.indexReaders = (BinaryReader[])indexReaders.ToArray(typeof(BinaryReader));
            string meta = Path.Combine(root, "main_file_cache.idx255");
            if(!File.Exists(meta))
                throw new FileNotFoundException();

            metaReader = new BinaryReader(File.OpenRead(meta));
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool hasData() {
            return dataReader.BaseStream.Length > 0;
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
            if((type < 0 || type >= indexReaders.Length) && type != 255)
                throw new FileNotFoundException();

            if(type == 255)
                return (int)metaReader.BaseStream.Length/Index.SIZE;

            return (int)indexReaders[type].BaseStream.Length/Index.SIZE;
        }

        /**
         * Gets the number of index files, not including the meta index file.
         * 
         * @return The number of index files.
         * @throws IOException
         *             if an I/O error occurs.
         */

        public int getTypeCount() {
            return indexReaders.Length;
        }

        /**
         * Reads a file.
         * 
         * @param type
         *            The type of the file.
         * @param id
         *            The id of the file.
         * @return A {@link ByteBuffer} containing the contents of the file.
         * @throws IOException
         *             if an I/O error occurs.
         */
          public ByteBuffer read(CacheIndex index, CacheIndex archive) {
              return read((int)index, (int) archive);
          }

        /**
         * Reads a file.
         * 
         * @param type
         *            The type of the file.
         * @param id
         *            The id of the file.
         * @return A {@link ByteBuffer} containing the contents of the file.
         * @throws IOException
         *             if an I/O error occurs.
         */

        public ByteBuffer read(int type, int id) {
            if((type < 0 || type >= indexReaders.Length) && type != 255)
                throw new FileNotFoundException();

            BinaryReader reader = type == 255 ? metaReader : indexReaders[type];
            long ptr = id*Index.SIZE;
            if(ptr < 0 || ptr > reader.BaseStream.Length) {
                Console.WriteLine("PTR: " + ptr);
                Console.WriteLine("Len:" + reader.BaseStream.Length);
                throw new FileNotFoundException();
            }
            //MemoryStream buf = StreamUtils.readerToStream(reader, ptr, Index.SIZE);
            ByteBuffer buf = new ByteBuffer(reader, (int)ptr, Index.SIZE);
            Index index = new Index(buf);
            ByteBuffer data = new ByteBuffer(index.getSize());
            int chunk = 0, remaining = index.getSize();
            ptr = index.getSector()*Sector.SIZE;
            do {
                buf.Dispose();
                buf = new ByteBuffer(dataReader, (int)ptr, Sector.SIZE);
                bool extended = id > 0xFFFF;
                Sector sect = new Sector(buf, extended);
                int dataSize = extended ? Sector.EXTENDED_DATA_SIZE : Sector.DATA_SIZE;
                if(remaining > dataSize) {
                    data.put(sect.getData());
                    remaining -= dataSize;
                    if(sect.getType() != type)
                        throw new IOException("File type mismatch.");
                    if(sect.getId() != id)
                        throw new IOException("File id mismatch.");
                    if(sect.getChunk() != chunk++)
                        throw new IOException("Chunk mismatch.");

                    ptr = sect.getNextSector()*Sector.SIZE;
                } else {
                    data.put(sect.getData());
                    remaining = 0;
                }
            } while(remaining > 0);

            data.flip();

            return data;
        }

        public virtual void Dispose(bool disposing) {
            if(disposed)
                return;

            dataReader.Close();
            foreach(BinaryReader r in indexReaders)
                r.Close();

            metaReader.Close();
            disposed = true;
        }

    }
}
