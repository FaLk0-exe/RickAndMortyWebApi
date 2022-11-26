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
        private static async Task<string> GetJson(string url)
        {
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(url);
            return response;
        }

        private static JToken GetElement(string type, string name)
        {
            JObject result = null;
            try
            {
                var task = GetJson($@"https://rickandmortyapi.com/api/{type}/?name={name}");
                task.Wait();
                result = JObject.Parse(task.Result);
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
            return result;
        }

        private static JToken GetElement(string url)
        {
            try
            {
                var task = GetJson(url);
                task.Wait();
                return JObject.Parse(task.Result);
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
                var characterId = (string)GetElement("character", characterName)["results"][0]["id"];
                var episode = GetElement("episode", episodeName);
                if (episode["results"][0]["characters"].
                    Count(a => _idRegex.Match((string)a).Value == characterId) == 0)
                {
                    return false;
                }
                return true;
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

        public static string GetCharacterInfo(string name)
        {
            try
            {
                var character = GetElement("character", name);
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
                                name = (string)GetElement((string)c["origin"]["url"])["name"],
                                type = (string)GetElement((string)c["origin"]["url"])["type"],
                                dimension = (string)GetElement((string)c["origin"]["url"])["dimension"],
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
