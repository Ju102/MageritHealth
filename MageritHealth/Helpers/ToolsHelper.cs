namespace MageritHealth.Helpers
{
    public class ToolsHelper
    {
        public static string GenerateSalt()
        {
            Random rand = new Random();
            string salt = "";

            for (int i = 0; i < 50; i++)
            {
                int num = rand.Next(1, 255);
                char letra = Convert.ToChar(num);
                salt += letra;
            }

            return salt;
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
    }
}
