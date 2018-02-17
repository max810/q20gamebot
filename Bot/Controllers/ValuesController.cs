using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;
using System.Net;

namespace Bot.Controllers
{
    [Route("vpnbook")]
    public class MainController : Controller
    {
        //https://www.vpnbook.com/

        private string password = "";
        private DateTimeOffset lastModified = DateTime.UtcNow;
        private string address = "https://www.vpnbook.com";
        private HttpClient client = new HttpClient();

        // GET api/values
        [HttpGet]
        [Route("password")]
        public async Task<string> GetPasswordAsync()
        {
            await UpdatePassword();
            return password;
        }

        private async Task UpdatePassword()
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
                    password = ParsePassword(document);
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
