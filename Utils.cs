using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Leaf.xNet;
using System.Threading;
using Microsoft.VisualBasic;

namespace KaliGuildedGenerator
{
    class Utils
    {
        public static Random random = new Random();
        public static List<string> Queue = new List<string>();
        public static List<string> Queue2 = new List<string>();

        public static string GetRandomUsername()
        {
            try
            {
                string[] lines = File.ReadAllLines("usernames.txt");
                return lines[random.Next(0, lines.Length)];
            }
            catch
            {
                return "";
            }
        }

        public static string GetRandomBio()
        {
            try
            {
                string[] lines = File.ReadAllLines("bios.txt");
                return lines[random.Next(0, lines.Length)];
            }
            catch
            {
                return "Viva i sassi sassosi sassolini rocciosi C':";
            }
        }

        public static HttpProxyClient GetRandomProxy()
        {
            HttpProxyClient client = null;
            try
            {
                string proxy = File.ReadAllLines("proxies.txt")[random.Next(0, File.ReadAllLines("proxies.txt").Length)];
                string[] splitted = Strings.Split(proxy, ":");
                if (splitted[0] != null && splitted[1] != null && Information.IsNumeric(splitted[1]))
                    client = new HttpProxyClient(splitted[0], int.Parse(splitted[1]));

                if (splitted[2] != null && splitted[2] != "ignore")
                    client.Username = splitted[2];

                if (splitted[3] != null && splitted[3] != "ignore")
                    client.Password = splitted[3];

                return client;
            }
            catch
            {
                return client;
            }
        }

        public static string RandomString(int Lenght)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, Lenght)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
