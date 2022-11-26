using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RickAndMortyWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RickAndMortyWebApi.Services
{
    public class RickAndMortyApiRepository:IRickAndMortyApiRepository
    {
        private Regex _idRegex;

        public RickAndMortyApiRepository()
        {
            _idRegex= new Regex(@"[0-9]+?");
        }

        private async Task<string> GetJson(string url)
        {
            HttpClient _client=new HttpClient();
            return await _client.GetStringAsync(url);
        }

        private JToken GetElement(string type, string name)
        {
            JObject result = null;
            try
            {
                var task = GetJson($@"https://rickandmortyapi.com/api/{type}/?name={name}");
                task.Wait();
                result = JObject.Parse(task.Result);
            }
            catch (AggregateException)
            {
                throw;
            }
            return result;
        }

        private JToken GetElement(string url)
        {
            try
            {
                var task = GetJson(url);
                task.Wait();
                return JObject.Parse(task.Result);
            }
            catch (AggregateException)
            {
                throw;
            }
        }

        private string GetCharacterId(string characterName)
        {
            return (string)GetElement("character", characterName)["results"][0]["id"];
        }
        

        public bool IsCharacterExistsInEpisode(string characterName, string episodeName)
        {
            try
            {
                var characterId = GetCharacterId(characterName);
                var episode = GetElement("episode", episodeName);
                if (episode["results"][0]["characters"].
                    Any(a => _idRegex.Match((string)a).Value == characterId))
                {
                    return true;
                }
                return false;
            }
            catch (AggregateException)
            {
                throw;
            }
        }

        public string GetCharacterInfo(string name)
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
            catch (AggregateException)
            {
                throw;
            }
        }
    }
}
