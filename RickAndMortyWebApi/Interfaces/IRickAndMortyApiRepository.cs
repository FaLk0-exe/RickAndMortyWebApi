using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RickAndMortyWebApi.Interfaces
{
    public interface IRickAndMortyApiRepository
    {
        bool IsCharacterExistsInEpisode(string characterName, string episodeName);
        string GetCharacterInfo(string name);
    }
}
