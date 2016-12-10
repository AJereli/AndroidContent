using System.Text;
using System.Security.Cryptography;

namespace AllContent_Client
{
    class MD5Hashing
    {
        private static MD5 md5_hash;

       private MD5Hashing()
        {
            
        }
        /// <summary>
        /// Determinate MD5 hash from input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Returns a hash of your string</returns>
        static public string GetMd5Hash(string input)
        {
            if (md5_hash == null)
                md5_hash = MD5.Create();
            byte[] data = md5_hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            string ans = "";
            for (int i = 0; i < data.Length; i++)
                ans += data[i].ToString("x2");
            return ans;
        }

        /// <summary>
        /// It compares two hash
        /// </summary>
        /// <param name="input">Non hashed string</param>
        /// <param name="hash">Hashed string</param>
        /// <param name="isInputHashed">Set true if input - hased string</param>
        /// <returns></returns>
        static public bool CompareHashes (string input, string hash, bool isInputHashed = false)
        {
            if (md5_hash == null)
                md5_hash = MD5.Create();
            if (!isInputHashed)
                return GetMd5Hash(input) == hash;
            else
                return input == hash;
        }
        
    }
}
