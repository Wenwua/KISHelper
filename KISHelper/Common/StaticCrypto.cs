using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    /// <summary>
    /// 静态加密/解密工具，AES-256-GCM，密钥派生自密码。
    /// </summary>
    public static class StaticCrypto
    {
        private const int KeySize = 32;        // 256-bit
        private const int NonceSize = 12;      // GCM 标准 96-bit IV
        private const int TagSize = 16;        // GCM 认证标签 128-bit
        private const int SaltSize = 16;       // PBKDF2 盐 128-bit
        private const int Pbkdf2Iter = 60_000; // 可根据性能调整

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="password">密码</param>
        /// <returns>Base64 密文（内部已含 Salt+IV+Tag）</returns>
        public static string Encrypt(string plainText, string password)
        {
            if (plainText is null) throw new ArgumentNullException(nameof(plainText));
            if (password is null) throw new ArgumentNullException(nameof(password));

            byte[] salt = RandomBytes(SaltSize);
            byte[] key = DeriveKey(password, salt);

            using var aes = new AesGcm(key);
            byte[] nonce = RandomBytes(NonceSize);
            byte[] plain = Encoding.UTF8.GetBytes(plainText);
            byte[] cipher = new byte[plain.Length];
            byte[] tag = new byte[TagSize];

            aes.Encrypt(nonce, plain, cipher, tag);

            // 拼接：salt|nonce|tag|ciphertext
            using var ms = new MemoryStream();
            ms.Write(salt, 0, salt.Length);
            ms.Write(nonce, 0, nonce.Length);
            ms.Write(tag, 0, tag.Length);
            ms.Write(cipher, 0, cipher.Length);

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherB64">由 Encrypt 生成的 Base64 密文</param>
        /// <param name="password">密码</param>
        /// <returns>明文</returns>
        public static string Decrypt(string cipherB64, string password)
        {

            if (cipherB64 is null) throw new ArgumentNullException(nameof(cipherB64));
            if (password is null) throw new ArgumentNullException(nameof(password));

            byte[] data = Convert.FromBase64String(cipherB64);
            if (data.Length < SaltSize + NonceSize + TagSize)
                throw new CryptographicException("Invalid cipher text");

            ReadOnlySpan<byte> salt = data.AsSpan(0, SaltSize);
            ReadOnlySpan<byte> nonce = data.AsSpan(SaltSize, NonceSize);
            ReadOnlySpan<byte> tag = data.AsSpan(SaltSize + NonceSize, TagSize);
            ReadOnlySpan<byte> cipher = data.AsSpan(SaltSize + NonceSize + TagSize);

            byte[] key = DeriveKey(password, salt.ToArray());
            using var aes = new AesGcm(key);
            byte[] plain = new byte[cipher.Length];
            aes.Decrypt(nonce, cipher, tag, plain);
            return Encoding.UTF8.GetString(plain);
        }

        /* ---------- 私有辅助 ---------- */
        private static byte[] DeriveKey(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Pbkdf2Iter, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(KeySize);
        }

        private static byte[] RandomBytes(int count)
        {
            byte[] buf = new byte[count];
            RandomNumberGenerator.Fill(buf);
            return buf;
        }
    }
}