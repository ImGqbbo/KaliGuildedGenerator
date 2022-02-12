using System;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Linq;
using System.Net;
using System.Text;
using Leaf.xNet;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace KaliGuildedGenerator
{
    class Program
    {
        static bool useProxies = false;
        static string Invite = "";
        static int Threads = 50;
        
        static void Main(string[] args)
        {
            Console.Write(" [?] Do you want to use proxies? [y/n] ");
            string str = Console.ReadLine();
            if (str == "y" || str == "n")
            {
                if (str == "y") useProxies = true;
                else if (str == "n") useProxies = false;
            }
            else useProxies = false;

            Console.Write(" [?] Insert invite? [y/n] ");
            string str2 = Console.ReadLine();
            if (str2 == "y" || str2 == "n")
            {
                if (str2 == "y")
                {
                    Console.Write(" [!] Insert invite > ");
                    string invite = Console.ReadLine();
                    Invite = invite;
                }
                else if (str2 == "n") { }
            }
            else { }

            Console.Write(" [?] Insert number of threads > ");
            string ths = Console.ReadLine();
            if (Information.IsNumeric(ths))
            {
                Threads = int.Parse(ths);
            }
            else { }

            for (int i = 0; i < Threads; i++)
            {
                Thread.Sleep(5);
                new Thread(GenerateToken).Start();
            }

            new Thread(FetchQueue).Start();
            Thread.Sleep(-1);
        }

        public static void FetchQueue()
        {
            try
            {
                while (true)
                {
                    if (Utils.Queue.Count > 0)
                    {
                        try
                        {
                            File.AppendAllText("fixed.txt", Utils.Queue[0] + "\n");
                            Utils.Queue.RemoveAt(0);
                        }
                        catch
                        {

                        }
                    }
                    else Thread.Sleep(200);
                }
            }
            catch
            {

            }
        }

        public static void GenerateToken()
        {
            while (true)
            {
                Thread.Sleep(5);
                TokenGen.CreateAccount(useProxies, Invite);
            }
        }
    }
}
