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
            if (gamePage.DocumentNode.SelectNodes("//h2").FirstOrDefault()
                ?.InnerText.ToLower().Contains("won") == true)
            {
                return Q20GameState.GameFinish;
            }
            var question = gamePage.DocumentNode.SelectNodes("//td/big/b").First().FirstChild;
            //not necessary
            //var answers = gamePage.DocumentNode.SelectNodes("//td/big/b/nobr").First().ChildNodes;

            var questionText = question.InnerText.ToLowerInvariant();

            if (questionText.StartsWith("q1"))
            // || questionText.Contains("is it classified as") not necessary
            {
                return Q20GameState.CategoryChoice;
            }
            if (new Regex(@"^(q[2-9]\.|q[1-2][0-9]\.|q30\.)").IsMatch(questionText))
            {
                if (questionText.Contains("i am guessing"))
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

        public async static Task<HtmlDocument> GetGamePage(string currentAddress)
        {
            HtmlDocument gamePage = new HtmlDocument();
            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(currentAddress),
                Method = HttpMethod.Get
            };

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
            var a = currentPage.DocumentNode.Descendants("a").Where(x =>
                string.Equals(messageText, x.InnerText, StringComparison.InvariantCultureIgnoreCase)).First();

            string uri = a.GetAttributeValue("href", def: null);
            uri = "http://y.20q.net" + uri;
            return uri;
        }
    }
}
