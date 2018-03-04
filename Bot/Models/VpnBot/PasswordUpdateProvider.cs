using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bot.VpnBot.Models
{
    public class PasswordUpdateProvider
    {
        public string Password { get; private set; } = string.Empty;

        private const string address = "https://vpnbook.com";
        private readonly HttpClient client;
        public PasswordUpdateProvider()
        {
            client = new HttpClient();
        }
        public async Task UpdatePassword()
        {
            var requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(address)
            };
            //requestMessage.Headers.IfModifiedSince = lastModified;

            using (var response = await client.SendAsync(requestMessage))
            {
                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    return;
                }

                response.EnsureSuccessStatusCode();

                //lastModified = response.Content.Headers.LastModified.Value;
                using (var content = response.Content)
                {
                    string body = await content.ReadAsStringAsync();
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(body);

                    var newPassword = ParsePassword(document);
                    Password = newPassword;

                    string oldPassword;
                    using (var stream = new StreamReader("Files/Password.txt"))
                    {
                        oldPassword = await stream.ReadLineAsync();
                    }
                    if (oldPassword == newPassword)
                    {
                        return;
                    }
                    else
                    {
                        using (var stream = new StreamWriter("Files/Password.txt", append: false))
                        {
                            stream.WriteLine(newPassword);
                        }
                    }
                }
            }
        }
        private string ParsePassword(HtmlDocument document)
        {
            var strongNodes = document.DocumentNode.SelectNodes("//strong");
            string password = strongNodes.Last().InnerText.Split(':').Last().Trim();
            return password;
        }
    }
}
