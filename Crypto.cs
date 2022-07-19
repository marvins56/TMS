using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TMS
{
    public class Crypto
    {
        public static string Hash(string Value)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Value))

                );
        }
    }
}