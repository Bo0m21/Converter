using System.Text;

namespace ExamUniverse.Converter.VCE.Extensions
{
    public static class DevelopExtension
    {
        public static string GetHexDevelop(this byte[] bytes)
        {
            if (bytes == null)
            {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder();
            
            for (int i = 0; i < bytes.Length; i++)
            {
                stringBuilder.Append(bytes[i].ToString("X") + " ");
            }

            return stringBuilder.ToString();
        }
    }
}
