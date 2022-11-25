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
    public static class RMApiRepository
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

        public static bool IsCharacterExistsInEpisode(string personName, string episodeName)
        {
            try
            {
                var personId = (string)GetElementByTypeAndGetParameter("character", personName)["results"][0]["id"];
                var episode = GetElementByTypeAndGetParameter("episode", episodeName);
                if (episode["results"][0]["characters"].
                    Count(a => _idRegex.Match((string)a).Value == personId) == 0)
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

        public static string GetCharacterJsonInfoByPersonName(string name)
        {
            try
            {
                var person = GetElementByTypeAndGetParameter("character", name);
                List<dynamic> personInfo = new List<dynamic>();
                foreach (var p in person["results"])
                {
                    if ((string)p["origin"]["url"] != "")
                        personInfo.Add(new
                        {
                            name = (string)p["name"],
                            status = (string)p["status"],
                            species = (string)p["species"],
                            type = (string)p["type"],
                            gender = (string)p["gender"],
                            origin = new
                            {
                                name = (string)GetElementByUrl((string)p["origin"]["url"])["name"],
                                type = (string)GetElementByUrl((string)p["origin"]["url"])["type"],
                                dimension = (string)GetElementByUrl((string)p["origin"]["url"])["dimension"],
                            }
                        });
                }
                return JsonConvert.SerializeObject(personInfo);
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
        }
    }
}
