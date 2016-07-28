using System;
using System.IO;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using OpenRSCS.utils;
using StreamUtils = ICSharpCode.SharpZipLib.Core.StreamUtils;

namespace OpenRSCS.cache.utils {
    internal class CompressionUtils {

        public static ByteBuffer decompressBZIP2(byte[] inBytes, int length) {
            byte[] hBytes = new byte[inBytes.Length + 4];
            hBytes[0] = Convert.ToByte('B');
            hBytes[1] = Convert.ToByte('Z');
            hBytes[2] = Convert.ToByte('h');
            hBytes[3] = Convert.ToByte('1');
            Array.Copy(inBytes, 0, hBytes, 4, inBytes.Length);
            ByteBuffer buf = new ByteBuffer();
            using(BZip2InputStream gz = new BZip2InputStream(new MemoryStream(hBytes))) {
                StreamUtils.Copy(gz, buf, new byte[4096]);
            }
            buf.Position = 0;
            return buf;
        }

        public static ByteBuffer decompressGZIP(byte[] inBytes, int length) {
            ByteBuffer buf = new ByteBuffer();

            using(GZipInputStream gz = new GZipInputStream(new MemoryStream(inBytes))) {
                StreamUtils.Copy(gz, buf, new byte[4096]);
            }
            buf.Position = 0;
            return buf;
        }

    }
}
