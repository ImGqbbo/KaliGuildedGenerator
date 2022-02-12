using System;
using System.Threading;
using System.IO;
using Leaf.xNet;
using System.Net;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace KaliGuildedGenerator
{
    class TokenGen
    {
        private static void EmailVerification(string Token, string UserId, string Email, HttpProxyClient proxyClient)
        {
            try
            {
                HttpRequest request = new HttpRequest();
                request.Proxy = proxyClient;
                request.AddHeader("Host", "www.guilded.gg");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Cookie", "hmac_signed_session=" + Token);
                request.Post("https://www.guilded.gg/api/email/verify");
                string VerificationLink = "";
                bool locked = true;
                while (locked)
                {
                    var msgs = EmailSettings.GetEmailMessages(Email);

                    foreach (var msg in msgs)
                    {
                        if (msg.Subject == "Verify your email on Guilded")
                        {
                            try
                            {
                                string[] splitted = Strings.Split(msg.Content, "you can ignore this email.\u003c/div\u003e\u003ca href=\"");
                                string First = splitted[1];
                                VerificationLink = First.Substring(0, First.IndexOf("\" style="));
                                locked = false;
                                break;
                            }
                            catch
                            {

                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
                Console.WriteLine($" [!] Got verification link ({VerificationLink})");
                HttpRequest request1 = new HttpRequest();
                request1.Proxy = proxyClient;
                request1.AddHeader("Host", "www.guilded.gg");
                request1.AddHeader("Connection", "keep-alive");
                request1.AddHeader("Cookie", "hmac_signed_session=" + Token);
                try
                {
                    request1.Get(VerificationLink);
                    Console.WriteLine($" [!] Succesfully email verified ({Token.Substring(0, 40)}...)");
                    Utils.Queue.Add(Token + ":" + UserId);
                }
                catch (HttpException ex)
                {
                    Console.WriteLine($" [!] Email verify fail (on verification) ({Token.Substring(0, 40)}...) > ({ex.Message})");
                    Thread.Sleep(4000);
                    EmailVerification(Token, UserId, Email, proxyClient);
                }
            }
            catch (HttpException ex)
            {
                Console.WriteLine($" [!] Email verify fail ({Token.Substring(0, 40)}...) > ({ex.Message})");
                Thread.Sleep(4000);
                EmailVerification(Token, UserId, Email, proxyClient);
            }
        }

        public static void CreateAccount(bool UseProxies, string Invite)
        {
            var proxyClient = Utils.GetRandomProxy();
            try
            {
                string email = EmailSettings.NewEmail();
                string payload = "{\"extraInfo\":{\"platform\":\"desktop\"},\"name\":\"" + Utils.GetRandomUsername() + "\",\"email\":\"" + email + "\",\"password\":\"#Guilded\",\"fullName\":\"" + Utils.RandomString(10) + "\"}";

                Console.WriteLine($" [!] Using proxy ({proxyClient.Host}:{proxyClient.Port})");
                HttpRequest request = new HttpRequest();
                request.IgnoreProtocolErrors = true;
                if (UseProxies)
                {
                    request.Proxy = proxyClient;
                }
                request.AddHeader("Host", "www.guilded.gg");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("guilded-client-id", "17119702-e1c6-4d8f-8920-4f7ca425b84d");
                request.AddHeader("guilded-device-id", "537b5c96c662685a3c6a9cf97b15fafe68ca4eb5141254ac908d08f4460218cc");
                request.AddHeader("guilded-stag", "5969163916de3cb6e314ab3072d2bdb9");
                request.Post("https://www.guilded.gg/api/users?type=email", payload, "application/json");

                Console.WriteLine($" [!] Generating ({email})");

                if(request.Response.IsOK)
                {
                    HttpRequest r = new HttpRequest();
                    try
                    {
                        string data = "{\"email\":\"" + email + "\", \"password\":\"#Guilded\", \"getMe\": true}";
                        if (UseProxies)
                        {
                            r.Proxy = proxyClient;
                        }
                        r.AddHeader("Host", "www.guilded.gg");
                        r.AddHeader("Content-Type", "application/json");
                        r.Post("https://www.guilded.gg/api/login", data, "application/json");

                        dynamic json = JsonConvert.DeserializeObject(r.Response.ToString());
                        string userId = (string)json.user.id;
                        foreach (Cookie cookie in r.Response.Cookies.GetCookies(new Uri("https://www.guilded.gg/api/login")))
                        {
                            if (cookie.Name == "hmac_signed_session")
                            {
                                try
                                {
                                    string Token = cookie.Value;
                                    Console.WriteLine($" [!] Generated ({Token.Substring(0, 40)}...)");

                                    HttpRequest req = new HttpRequest();
                                    req.IgnoreProtocolErrors = true;
                                    if (UseProxies)
                                    {
                                        req.Proxy = proxyClient;
                                    }
                                    req.AddHeader("Connection", "keep-alive");
                                    req.AddHeader("Cookie", "hmac_signed_session=" + Token);
                                    req.AddHeader("Host", "www.guilded.gg");
                                    req.AddHeader("Content-Type", "application/json");
                                    req.Put("https://www.guilded.gg/api/users/" + userId + "/profilev2", "{\"userId\":\"" + userId + "\",\"aboutInfo\":{\"bio\":\"" + Utils.GetRandomBio() + "\"}}", "application/json");

                                    Console.WriteLine(" [!] Succesfully set bio");

                                    if (Invite != "")
                                    {
                                        new GuildedClient(cookie.Value).JoinGuild(Invite, proxyClient);
                                    }

                                    new Thread(() => EmailVerification(Token, userId, email, proxyClient)).Start();
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    catch (HttpException ex)
                    {
                        HttpResponse response = r.Response;
                        if (response != null)
                        {
                            try
                            {
                                Console.WriteLine($" [!] Login fail ({proxyClient.Host}:{proxyClient.Port}) ({response})");
                            }
                            catch
                            {

                            }
                        }
                        else
                            Console.WriteLine($" [!] Login fail ({proxyClient.Host}:{proxyClient.Port}) ({ex.Message})");
                    }
                    catch (ProxyException ex)
                    {
                        Console.WriteLine($" [!] Login proxy fail ({proxyClient.Host}:{proxyClient.Port}) ({ex.Message})");
                    }
                }
                else Console.WriteLine($" [!] Generation fail ({proxyClient.Host}:{proxyClient.Port}) ({request.Response})");
            }
            catch (HttpException ex)
            {
                Console.WriteLine($" [!] Generation fail ({proxyClient.Host}:{proxyClient.Port}) ({ex.Message})");
            }
        }
    }
}
