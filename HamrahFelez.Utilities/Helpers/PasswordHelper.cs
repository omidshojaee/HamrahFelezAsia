using System.Security.Cryptography;

namespace HamrahFelez.Utilities
{
    public sealed class PasswordHelper
    {
        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (hashedPassword == null)
                return false;

            if (password == null)
                throw new ArgumentNullException("password");

            byte[] src = Convert.FromBase64String(hashedPassword);

            if ((src.Length != 0x31) || (src[0] != 0))
                return false;

            byte[] salt = new byte[0x10];
            Buffer.BlockCopy(src, 1, salt, 0, 0x10);
            byte[] storedSubkey = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, storedSubkey, 0, 0x20);

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, salt, 0x3e8))
            {
                byte[] generatedSubkey = bytes.GetBytes(0x20);
                return ByteArraysEqual(storedSubkey, generatedSubkey);
            }
        }

        private static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2)
                return true;

            if (b1 == null || b2 == null || b1.Length != b2.Length)
                return false;

            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                    return false;
            }
            return true;
        }
    }
}
