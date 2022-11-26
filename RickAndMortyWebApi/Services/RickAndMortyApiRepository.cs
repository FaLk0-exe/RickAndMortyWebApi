using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RickAndMortyWebApi.Services
{
    public static class RickAndMortyApiRepository
    {
        private static Regex _idRegex = new Regex(@"[0-9]+?");
        private static async Task<string> GetJsonFromApi(string url)
        {
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(url);
            return response;
        }

        private static JToken GetElementByTypeAndGetParameter(string type, string getParameter)
        {
            JObject result = null;
            try
            {
                var elTask = GetJsonFromApi($@"https://rickandmortyapi.com/api/{type}/?name={getParameter}");
                elTask.Wait();
                result = JObject.Parse(elTask.Result);
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
            return result;
        }

        private static JToken GetElementByUrl(string url)
        {
            try
            {
                var elTask = GetJsonFromApi(url);
                elTask.Wait();
                return JObject.Parse(elTask.Result);
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        public static bool IsCharacterExistsInEpisode(string characterName, string episodeName)
        {
            try
            {
                var characterId = (string)GetElementByTypeAndGetParameter("character", characterName)["results"][0]["id"];
                var episode = GetElementByTypeAndGetParameter("episode", episodeName);
                if (episode["results"][0]["characters"].
                    Count(a => _idRegex.Match((string)a).Value == characterId) == 0)
                {
                    return false;
                }
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
        }

        public static string GetCharacterInfoByPersonName(string name)
        {
            try
            {
                var character = GetElementByTypeAndGetParameter("character", name);
                List<dynamic> characterInfo = new List<dynamic>();
                foreach (var c in character["results"])
                {
                    if ((string)c["origin"]["url"] != "")
                        characterInfo.Add(new
                        {
                            name = (string)c["name"],
                            status = (string)c["status"],
                            species = (string)c["species"],
                            type = (string)c["type"],
                            gender = (string)c["gender"],
                            origin = new
                            {
                                name = (string)GetElementByUrl((string)c["origin"]["url"])["name"],
                                type = (string)GetElementByUrl((string)c["origin"]["url"])["type"],
                                dimension = (string)GetElementByUrl((string)c["origin"]["url"])["dimension"],
                            }
                        });
                }
                return JsonConvert.SerializeObject(characterInfo);
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
        }
    }
}
