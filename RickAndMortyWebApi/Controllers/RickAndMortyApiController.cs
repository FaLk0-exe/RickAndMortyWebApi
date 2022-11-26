using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RickAndMortyWebApi.Interfaces;
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
    public class RickAndMortyApiController : ControllerBase
    {

        public RickAndMortyApiController()
        {
        }
        [Route("/api/v1/check-person")]
        [HttpPost]
        public ActionResult<bool> Post([FromServices] IRickAndMortyApiRepository repos,[FromBody] PersonEpisodeModel model)
        {
            try
            {
                return repos.IsCharacterExistsInEpisode(model.PersonName, model.EpisodeName);
            }
            catch (AggregateException)
            {
                return NotFound();
            }
        }

        [Route("/api/v1/person")]
        [HttpGet]
        public ActionResult<string> Get([FromServices] IRickAndMortyApiRepository repos)
        {
            try
            {
                string name = Request.Query["name"];
                return repos.GetCharacterInfo(name);
            }
            catch(AggregateException)
            {
                return NotFound();
            }

        }
    }
}
