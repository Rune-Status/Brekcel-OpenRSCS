using System.IO;
using System.Text;

namespace OpenRSCS.utils {
    public class StreamUtils {

        public static int getMedium(BinaryReader br) {
            return (br.ReadByte() & 0xFF) << 16 | (br.ReadByte() & 0xFF) << 8 | br.ReadByte() & 0xFF;
        }

        public static int getUShort(Stream stream) {
            return (stream.ReadByte() & 0xFF) << 8 | stream.ReadByte() & 0xFF;
        }

        public static void getSByteArr(Stream stream, sbyte[] arr) {
            for(int i = 0; i < arr.Length; i++) {
                arr[i] = (sbyte)stream.ReadByte();
            }
        }

        public static string printArray<T>(T[] bytes) {
            StringBuilder sb = new StringBuilder("Length: ");
            sb.Append(bytes.Length);
            sb.Append(" [");
            foreach (T t in bytes) {
                sb.Append(t);
                sb.Append(", ");
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append("]");
            return sb.ToString();
        }

        /*public static MemoryStream readerToStream(BinaryReader buf, int offset, int size) {
            MemoryStream ms = new MemoryStream(size);
            int initPos = (int)buf.BaseStream.Position;
            try {
                buf.BaseStream.Position = offset;
                byte[] bytes = new byte[size];
                buf.Read(bytes, offset, bytes.Length);
                ms.Write(bytes, 0, bytes.Length);
                streamFlip(ms);
            } catch (Exception e) {
                Debug.Log(e);
            } finally {
                buf.BaseStream.Position = initPos;
            }
            return ms;
        }

        public static int getInt(Stream stream) {
            return ((stream.ReadByte() & 0xFF) << 24) | ((stream.ReadByte() & 0xFF) << 16) | ((stream.ReadByte() & 0xFF) << 8) | (stream.ReadByte() & 0xFF);

        }*/

    }
}
