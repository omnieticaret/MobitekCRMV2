using Mobitek.CRM.Models.BackLinkModels;
using Mobitek.CRM.Models.NewsSiteModels;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MobitekCRMV2.Extensions
{
    public class CustomReader
    {
        private readonly IHttpClientFactory _httpClientFactory;



        public CustomReader(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }



        public async Task<SearchItem> CheckBackLink(string url, string domain)//refactored 28.09.23
        {
            var connectionResults = await GetConnectionJsonAsync(url);
            var statusMessage = connectionResults[0];
            var jsonContent = connectionResults[1];

            var searchItem = new SearchItem();

            if (statusMessage == "OK")
            {
                var searchResults = ReadJson(jsonContent, domain);
                searchItem = searchResults.Count <= 1 ? new SearchItem { Status = "403" } : searchResults[0];
                searchItem.Status = searchItem.Status == null ? "OK" : searchItem.Status;
            }
            else
            {
                searchItem.Status = statusMessage.Length > 3 ? "ERROR" : statusMessage;
            }

            return searchItem;
        }


        /// <summary>
        /// Bu method url alır istek atar , dönen mesajı(1)  ve HtmlBody'sini(2) string listesi olarak döner.
        /// </summary>
        public async Task<List<string>> GetConnectionJsonAsync(string url)
        {
            List<string> liste = new List<string>();
            string json = "";

            try
            {

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                    var checkingResponse = await httpClient.GetAsync(url);


                    var response = await checkingResponse.Content.ReadAsStringAsync();

                    int statusCodeInt = (int)checkingResponse.StatusCode;

                    if (checkingResponse.IsSuccessStatusCode)
                    {
                        liste.Add(checkingResponse.StatusCode.ToString());
                    }
                    else if (statusCodeInt >= 400 && statusCodeInt < 500)  // All 4xx errors
                    {
                        liste.Add("404");
                    }
                    else
                    {
                        liste.Add("500");
                    }

                    var stream = httpClient.GetStreamAsync(url).Result;
                    var reader = new StreamReader(stream);
                    json = reader.ReadToEnd();
                    reader.Close();
                    httpClient.Dispose();
                }



            }
            catch (Exception e)
            {
                liste.Add(e.Message);
            }
            liste.Add(json);

            return liste;
        }



        /// <summary>
        /// Bu method string HtmlBody ve aranacak domain alır ve List<SearchItem> içerisinde url,urlSection,Text listesi döner.
        /// </summary>
        public List<SearchItem> ReadJson(string inputString, string domain)
        {
            var liste = new List<SearchItem>();
            string hrefPattern = "<a\\s+[^>]*href\\s*=\\s*[\"']([^\"']+)[\"'][^>]*>(.*?)<\\/a>";
            string hrefPattern2 = "<a\\s*[^>]*?href\\s*=\\s*[\"']?([^\"'\\s>]+)[\"']?[^>]*?>(.*?)<\\/a>";


            try
            {
                MatchCollection matches = Regex.Matches(inputString, hrefPattern2,
                                                        RegexOptions.None | RegexOptions.None,
                                                        TimeSpan.FromSeconds(1));

                foreach (Match match in matches)
                {
                    if (match.Value.Contains(domain))
                    {
                        SearchItem search = new SearchItem();

                        var hrefGroup = match.Groups[1].Value;
                        var anchorGroup = match.Groups[2].Value;

                        if (!hrefGroup.Contains(domain))
                            hrefGroup = match.Value;

                        if (anchorGroup == "")
                        {
                            var word = domain + "/" + GetLandingPage(hrefGroup, domain);
                            word = GetAnchorFromHref(hrefGroup, word);
                            search.Anchor = CleanUpAnchorText(word);
                        }
                        else
                        {
                            search.Anchor = CleanUpAnchorText(anchorGroup);
                        }

                        search.LandingPage = GetLandingPage(hrefGroup, domain);
                        search.Url = GetHrefUrl(hrefGroup);

                        liste.Add(search);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("The matching operation timed out.");
            }
            return liste;
        }

        private string GetLandingPage(string hrefGroup, string domain)
        {
            var landingPage = "";
            if (hrefGroup.Contains(domain + "/"))
            {
                landingPage = hrefGroup.Split(domain + "/")[1];
            }

            landingPage = landingPage.Trim('"');
            if (landingPage.Contains('"'))
                landingPage = landingPage.Split('"')[0];

            if (landingPage == "")
            {
                landingPage = @"\";
            }
            if (landingPage.Contains("/"))
                landingPage = landingPage.Trim('/');

            return landingPage;


        }

        private string GetAnchorFromHref(string hrefGroup, string word)
        {

            if (hrefGroup.Contains($"href=\""))
            {
                var list2 = hrefGroup.Split($"href=\"");
                if (list2[1].Contains(word))
                {
                    word = list2[1].Split(word)[1];
                }
            }
            return word;
        }

        private string CleanUpAnchorText(string word)
        {
            // Remove all HTML tags from the anchor text
            word = Regex.Replace(word, "<.*?>", string.Empty);

            if (word.Contains("/"))
            {
                word = word.Split("/")[0];
            }

            return word;
        }
        private string GetHrefUrl(string hrefGroup)
        {

            var result = "";
            // Extract the URL from the href attribute of the anchor tag
            if (hrefGroup.Contains($"href=\""))
            {
                var list2 = hrefGroup.Split($"href=\"");
                if (list2[1].Contains("\""))
                {
                    result = list2[1].Split("\"")[0];
                }
            }
            return result;

        }



        public SeoResult PostMozAPI(string url)
        {
            string uri = "https://lsapi.seomoz.com/v2/links";
            string username = "mozscape-e9b21ecfb9";
            string password = "2d74b74d9681fa1d86db40452942c634";
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            int pageAuthority = 0;
            int domainAuthority = 0;

            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                    using StringContent jsonContent = new(
                        JsonSerializer.Serialize(new
                        {
                            target = url,
                            limit = 1
                        }),
                        Encoding.UTF8,
                        "application/json");
                    using HttpResponseMessage response = httpClient.PostAsync(
                    uri,
                   jsonContent).Result;

                    response.EnsureSuccessStatusCode();

                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"{jsonResponse}\n");

                    JObject json = JObject.Parse(jsonResponse);

                    // Gereken alanlardaki veriyi çek
                    pageAuthority = (int)json["results"][0]["target"]["page_authority"];
                    domainAuthority = (int)json["results"][0]["target"]["domain_authority"];
                }
            }
            catch (Exception ex)
            {
            }

            return new SeoResult()
            {
                name = url,
                pa = pageAuthority,
                da = domainAuthority
            };

        }

    }
}
