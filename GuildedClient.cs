using Leaf.xNet;
using System;
using System.Collections.Generic;

namespace KaliGuildedGenerator
{
    class GuildedClient
    {
        public string Token { get; private set; }
        
        public GuildedClient(string Token)
        {
            this.Token = Token;
        }

        public void JoinGuild(string InviteCode, HttpProxyClient proxyClient)
        {
            HttpRequest request = new HttpRequest();
            try
            {
                request.Proxy = proxyClient;
                request.AddHeader("Cookie", "hmac_signed_session=" + Token);
                request.Put("https://www.guilded.gg/api/invites/" + InviteCode);

                Console.WriteLine($" [!] Joined in {InviteCode} ({Token.Substring(0, 40)}...)");
            }
            catch (HttpException ex)
            {
                Console.WriteLine($" [!] Failed to join in {InviteCode} ({Token.Substring(0, 40)}...) ({ex.Message})");
            }
            catch
            {
                Console.WriteLine($" [!] Failed to join in {InviteCode} ({Token.Substring(0, 40)}...)");
            }
        }

    }
}
