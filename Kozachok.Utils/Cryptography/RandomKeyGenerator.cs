using System;

namespace Kozachok.Utils.Cryptography
{
    public static class RandomKeyGenerator
    {
        public static string Generate()
        {
            var guid = Guid.NewGuid();
            var base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64
                .Replace("=", "")
                .Replace("+", "");
        }
    }
}
