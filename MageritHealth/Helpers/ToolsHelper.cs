using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace MageritHealth.Helpers
{
    public class ToolsHelper
    {
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[36];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static bool CompareArrays(byte[] a, byte[] b)
        {
            bool iguales = true;

            if (a.Length != b.Length)
            {
                iguales = false;
            }
            else
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (!a[i].Equals(b[i]))
                    {
                        iguales = false;
                        break;
                    }
                }
            }

            return iguales;
        }

        public static string GenerateEmailCorporativo(string nombre, string apellido1, string? apellido2)
        {
            string nombreSinTildes = EliminarTildes(nombre);
            string apellido1SinTildes = EliminarTildes(apellido1);
            if (!string.IsNullOrEmpty(apellido2))
            {
                apellido2 = EliminarTildes(apellido2);
            }
            string email = $"{nombreSinTildes.ToLower()}.{apellido1SinTildes.ToLower()}";
            if (!string.IsNullOrEmpty(apellido2))
            {
                email += $"{apellido2.ToLower()}";
            }
            email += "@magerithealth.com";
            return email;
        }

        private static string EliminarTildes(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            normalizedString = normalizedString.Replace(" ", "");
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string GenerateRandomPassword()
        {
            int longitud = 10;

            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#*-_";
            char[] passwordGenerada = new char[longitud];

            for (int i = 0; i < longitud; i++)
            {
                int randomIndex = RandomNumberGenerator.GetInt32(validChars.Length);
                passwordGenerada[i] = validChars[randomIndex];
            }

            return new string(passwordGenerada);
        }
    }
}
