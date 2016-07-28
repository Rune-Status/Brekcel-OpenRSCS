using System;
using System.IO;
using System.Text;

namespace OpenRSCS.utils {
    public class ByteBuffer : MemoryStream {

        public ByteBuffer(int size) : base(size) { }

        public ByteBuffer(BinaryReader buf, int offset, int size) : base(size) {
            int initPos = (int)buf.BaseStream.Position;
            try {
                buf.BaseStream.Position = offset;
                byte[] bytes = new byte[size];
                buf.Read(bytes, 0, bytes.Length);
                Write(bytes, 0, bytes.Length);
                flip();
            } catch (Exception e) {
                Console.WriteLine(e);
            } finally {
                buf.BaseStream.Position = initPos;
            }
        }

        public ByteBuffer(ByteBuffer buf, int offset, int size) : base(size) {
            int initPos = (int)buf.Position;
            try {
                buf.Position = offset;
                byte[] bytes = new byte[size];
                buf.Read(bytes, 0, bytes.Length);
                Write(bytes, 0, bytes.Length);
                flip();
            } catch (Exception e) {
                Console.WriteLine(e);
            } finally {
                buf.Position = initPos;
            }
        }

        public ByteBuffer() { }

        public ByteBuffer(byte[] bytes) : base(bytes) { }

        public sealed override void Write(byte[] buffer, int offset, int count) {
            base.Write(buffer, offset, count);
        }

        public int remaining() {
            return (int)(Length - Position);
        }

        public void flip() {
            SetLength(Position);
            Position = 0;
        }

        public byte get() {
            return (byte)ReadByte();
        }

        public byte get(long position) {
            long pos = Position;
            Position = position;
            byte retVal = get();
            Position = pos;
            return retVal;
        }

        public sbyte getSByte() {
            return (sbyte)get();
        }

        public short getShort() {
            return (short)((getSByte() & 0xFF) << 8 | getSByte() & 0xFF);
        }

        public short getUShort() {
            return (short)(getShort() & 0xFFFF);
        }

        public int getMedium() {
            return (getSByte() & 0xFF) << 16 | (getSByte() & 0xFF) << 8 | getSByte() & 0xFF;
        }

        public int getInt() {
            return (getSByte() & 0xFF) << 24 | (getSByte() & 0xFF) << 16 | (getSByte() & 0xFF) << 8 | getSByte() & 0xFF;
        }

        public int getSmartInt() {
            if (get(Position) < 0) // Has a warning but I'm too scared to touch. 
                return getInt() & 0x7fffffff;

            return getShort() & 0xFFFF;
        }

        public void getBytes(byte[] data) {
            getBytes(data, 0, data.Length);
        }

        public void getBytes(byte[] data, int offset, int length) {
            for (int i = 0; i < length; i++) {
                data[i + offset] = get();
            }
        }

        public void put(byte[] buffer) {
            put(buffer, 0, buffer.Length);
        }

        public void put(byte[] buffer, int offset, int dataSize) {
            try {
                Write(buffer, offset, dataSize);
            } catch (OutOfMemoryException e) {
                Console.WriteLine(e);
                Console.WriteLine("Cap: " + Capacity);
                Console.WriteLine("Position: " + Position);
                Environment.Exit(0);
            }
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder("Length: ");
            sbyte[] buffer = Array.ConvertAll(ToArray(), b => (sbyte)b);
            sb.Append(buffer.Length);
            sb.Append(" [");
            foreach (sbyte t in buffer) {
                sb.Append(t);
                sb.Append(", ");
            }

            sb.Append("]");
            return sb.ToString();
        }

        public ByteBuffer clone() {
            return new ByteBuffer(this, 0, (int)Length);
        }

        public int getSmartA() {
            int peek = get(Position) & 0xFF;
            if (peek < 128)
                return (get() & 0xFF) - 64;
            return (getShort() & 0xFFFF) - 49152;

        }

        public void position(int pos) {
            Position = pos;
        }

    }
}
