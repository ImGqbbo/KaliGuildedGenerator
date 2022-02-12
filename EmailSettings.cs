using Leaf.xNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaliGuildedGenerator
{
    public class EmailSettings
    {
        public static string NewEmail()
        {
            try
            {
                HttpRequest request = new HttpRequest();
                request.AddHeader("Content-Type", "application/json;charset=UTF-8");
                request.Post("https://api.internal.temp-mail.io/api/v3/email/new", "{\"name\": \"" + Utils.RandomString(20) + "\", \"domain\": \"bestparadize.com\"}", "application/json;charset=UTF-8");

                dynamic json = JsonConvert.DeserializeObject(request.Response.ToString());

                return (string)json.email;
            }
            catch
            {
                return Utils.RandomString(20) + "@gmail.com";
            }
        }

        public static List<EmailMessage> GetEmailMessages(string Email)
        {
            try
            {
                HttpRequest request = new HttpRequest();
                request.Get("https://api.internal.temp-mail.io/api/v3/email/" + Email + "/messages");

                return JsonConvert.DeserializeObject<List<EmailMessage>>(request.Response.ToString());
            }
            catch
            {
                return new List<EmailMessage>() { };
            }
        }
    }
}
