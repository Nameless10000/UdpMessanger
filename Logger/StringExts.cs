using System.Text;

namespace Logger
{
    public static class StringExts
    {
        public static byte[] GetBytes(this string str)
        {
            return Encoding.ASCII.GetBytes(str, 0, str.Length);
        }

        public static string GetString(this byte[] bytes, int? count = null)
        {
            return Encoding.ASCII.GetString(bytes, 0, count ?? bytes.Length);
        }
    }
}
