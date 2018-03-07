using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Bot.Models.Q20GameBot
{
    public static class Q20GameBotTools
    {
        private static HttpClient client = new HttpClient();

        public static Q20GameState ResolveGameState(HtmlDocument gamePage)
        {
            var h2Nodes = gamePage.DocumentNode.SelectNodes("//h2");
            if (h2Nodes != null && h2Nodes.First().InnerText.Contains(" won"))
            {
                return Q20GameState.GameFinish;
            }
            var question = gamePage.DocumentNode.SelectNodes("//td/big/b").First().InnerText.ToLowerInvariant();

            if (question.Contains("classified as"))
            {
                return Q20GameState.CategoryChoice;
            }

            if (new Regex(@"^(q[1-9]\.|q[1-2][0-9]\.|q30\.)").IsMatch(question))
            {
                if (question.Contains("guessing"))
                {
                    return Q20GameState.GuessConfirmation;
                }
                else
                {
                    return Q20GameState.AnswerPending;
                }
            }


            throw new ArgumentException("Uknown game page type");
        }

        public static void EnsureCategoryCorrespondance(Q20GameState state, string messageText)
        {
            var commands = Correspondance.CorrespondingCommands[state];
            if (!commands.Contains(messageText))
            {
                throw new ArgumentException($"Invalid command {messageText} for category {state}");
            }
        }

        public async static Task<HtmlDocument> GetGamePageAsync(string currentAddress, string referer = null)
        {
            HtmlDocument gamePage = new HtmlDocument();
            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(currentAddress),
                Method = HttpMethod.Post
            };
            if (referer != null)
            {
                requestMessage.Headers.Referrer = new Uri(referer);
            }

            using (var response = await client.SendAsync(requestMessage))
            {
                response.EnsureSuccessStatusCode();

                using (var content = response.Content)
                {
                    string body = await content.ReadAsStringAsync();

                    gamePage.LoadHtml(body);

                    return gamePage;
                }
            }
        }

        public static string GetNextUri(HtmlDocument currentPage, string messageText)
        {
            var a = currentPage.DocumentNode.SelectNodes("//a").Where(x =>
                string.Equals(messageText, HtmlEntity.DeEntitize(x.InnerText).Trim(), 
                StringComparison.InvariantCultureIgnoreCase))
                .First();

            string uri = a.GetAttributeValue("href", def: null);
            uri = "http://y.20q.net" + uri;
            return uri;
        }

        public async static Task<HtmlDocument> GetNewGamePageAsync()
        {
            HtmlDocument startingPage = new HtmlDocument();

            var startingPageRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://y.20q.net/gsq-en");
            startingPageRequestMessage.Headers.Referrer = new Uri("http://www.20q.net/flat/play.html");

            using(var response = await client.SendAsync(startingPageRequestMessage))
            {
                using(var content = response.Content)
                {
                    string body = await content.ReadAsStringAsync();
                    startingPage.LoadHtml(body);
                }
            }

            var gameStartUri = startingPage.DocumentNode
                .SelectSingleNode("//form")
                .GetAttributeValue("action", def: null);
            gameStartUri = "http://y.20q.net" + gameStartUri;
            var newGameRequestMessage = new HttpRequestMessage(HttpMethod.Post, gameStartUri);
            newGameRequestMessage.Headers.Referrer = new Uri("http://y.20q.net/gsq-en");
            newGameRequestMessage.Headers.Add("Origin", "http://y.20q.net");
            newGameRequestMessage.Content = new StringContent("age=&cctkr=UA%2CHR%2CRU%2CSK%2CPL&submit=++Play++");

            HtmlDocument newGamePage = new HtmlDocument();
            using(var response = await client.SendAsync(newGameRequestMessage))
            {
                using(var content = response.Content)
                {
                    string body = await content.ReadAsStringAsync();
                    newGamePage.LoadHtml(body);
                }
            }

            return newGamePage;
        }

        public static string GetQuestion(HtmlDocument nextGamePage)
        {
            var h2Nodes = nextGamePage.DocumentNode.SelectNodes("//h2");
            if (h2Nodes != null)
            {
                return h2Nodes.First().InnerText;
            }
            else
            {
                return nextGamePage.DocumentNode.SelectSingleNode("//big/b").InnerText.Split('\n').First();
            }
        }
    }
}
