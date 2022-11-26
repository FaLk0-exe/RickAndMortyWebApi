using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RickAndMortyWebApi.Models;
using RickAndMortyWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RickAndMortyWebApi.Controllers
{
    [ApiController]
    public class RickAndMortyApi : ControllerBase
    {

        public RickAndMortyApi()
        {
        }
        [Route("/api/v1/check-person")]
        [HttpPost]
        public ActionResult<bool> Post([FromBody] PersonEpisodeModel model)
        {
            try
            {
                return RickAndMortyApiRepository.IsCharacterExistsInEpisode(model.CharacterName, model.EpisodeName);
            }
            catch (HttpRequestException)
            {
                return NotFound();
            }
        }

        [Route("/api/v1/person")]
        [HttpGet]
        public ActionResult<string> Get()
        {
            try
            {
                string name = Request.Query["name"];
                return RickAndMortyApiRepository.GetCharacterInfoByPersonName(name);
            }
            catch
            {
                return NotFound();
            }

        }
    }
}
