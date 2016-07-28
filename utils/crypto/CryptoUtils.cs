using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;

namespace OpenRSCS.utils.crypto {
    public class CryptoUtils {

        public static byte[] whirlpool(ByteBuffer buf) {
            byte[] bytes = new byte[buf.Length];
            buf.getBytes(bytes);
            return whirlpool(bytes);
        }

        public static byte[] whirlpool(byte[] bytes) {
            WhirlpoolDigest wp = new WhirlpoolDigest();
            wp.BlockUpdate(bytes, 0, bytes.Length);
            byte[] digest = new byte[64];
            wp.DoFinal(digest, 0);
            return digest;
        }

        public static byte[] RSA(byte[] bytes, BigInteger modulus, BigInteger key) {
            BigInteger input = new BigInteger(bytes);
            BigInteger output = input.ModPow(key, modulus);
            return output.ToByteArray();
        }

        public static int getCrcChecksum(ByteBuffer buffer) {
            CRC crc = new CRC();
            byte[] bytes = buffer.GetBuffer();
            foreach(byte b in bytes)
                crc.UpdateCRC(b);

            return crc.GetFinalCRC();
        }

        /**
         * An implementation of Dan Bernstein's {@code djb2} hash function which is
         * slightly modified. Instead of the initial hash being 5381, it is zero.
         * 
         * @param str
         *            The string to hash.
         * @return The hash code.
         */

        public static int djb2(string str) {
            int hash = 0;
            char[] chars = str.ToCharArray();
            foreach(char t in chars)
                hash = t + ((hash << 5) - hash);

            return hash;
        }

    }
}
