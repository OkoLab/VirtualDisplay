using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VirtualDisplay
{
    class Crypt
    {
        public void EncryptFile(string filePath, string key)
        {
            try
            {
                byte[] plainContent = File.ReadAllBytes(filePath);
                using (var DES = new DESCryptoServiceProvider())
                {
                    DES.IV = Encoding.UTF8.GetBytes(key);
                    DES.Key = Encoding.UTF8.GetBytes(key);
                    DES.Mode = CipherMode.CBC;
                    DES.Padding = PaddingMode.PKCS7;

                    using (var memStream = new MemoryStream())
                    {
                        CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateEncryptor(),
                            CryptoStreamMode.Write);

                        cryptoStream.Write(plainContent, 0, plainContent.Length);
                        cryptoStream.FlushFinalBlock();
                        File.WriteAllBytes(filePath, memStream.ToArray());
                    }
                }
            }
            catch(Exception ex)
            {
                String str = ex.Message;
            }            
        }

        public string DecryptFile(string filePath, string key)
        {
            byte[] encrypted = File.ReadAllBytes(filePath);
            using (var DES = new DESCryptoServiceProvider())
            {
                DES.IV = Encoding.UTF8.GetBytes(key);
                DES.Key = Encoding.UTF8.GetBytes(key);
                DES.Mode = CipherMode.CBC;
                DES.Padding = PaddingMode.PKCS7;

                using (var memStream = new MemoryStream())
                {
                    CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateDecryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(encrypted, 0, encrypted.Length);
                    cryptoStream.FlushFinalBlock();

                    //File.WriteAllBytes(filePath, memStream.ToArray());
                    
                    return Encoding.ASCII.GetString(memStream.ToArray());
                    //File.WriteAllBytes(filePath, memStream.ToArray());
                }

                /*
                using (FileStream fout = new FileStream("file1.txt", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fout.SetLength(0);

                    CryptoStream cryptoStream = new CryptoStream(fout, DES.CreateDecryptor(DES.Key, DES.IV),
                        CryptoStreamMode.Write);



                    cryptoStream.Write(encrypted, 0, encrypted.Length);
                    //cryptoStream.FlushFinalBlock();

                    //File.WriteAllBytes(filePath, memStream.ToArray());

                    //string str = Encoding.ASCII.GetString(memStream.ToArray());

                    return "test";
                    //File.WriteAllBytes(filePath, memStream.ToArray());
                }
                */
            }

        }
    }
}
