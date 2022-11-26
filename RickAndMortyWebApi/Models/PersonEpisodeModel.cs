using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RickAndMortyWebApi.Models
{
    public class PersonEpisodeModel
    {
        [JsonProperty("personName")]
        public string PersonName { get; set; }
        [JsonProperty("episodeName")]
        public string EpisodeName { get; set; }
    }
}
