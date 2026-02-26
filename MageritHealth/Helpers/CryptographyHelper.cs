using System.Security.Cryptography;
using System.Text;

namespace MageritHealth.Helpers
{
    public class CryptographyHelper
    {
        public static byte[] EncryptPassword(string password, string salt)
        {
            string content = password + salt; // Da igual la posición: principio, final o en medio.

            SHA512 managed = SHA512.Create();
            byte[] salida = Encoding.UTF8.GetBytes(content);

            for (int i = 1; i <= 30; i++)
            {
                salida = managed.ComputeHash(salida);
            }
            managed.Clear();
            return salida;
        }
    }
}
