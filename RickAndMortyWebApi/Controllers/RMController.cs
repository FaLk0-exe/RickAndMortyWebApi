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
    public class RMController : ControllerBase
    {

        public RMController()
        {
        }
        [Route("/api/v1/check-person")]
        [HttpPost]
        public ActionResult<bool> Post([FromBody] PersonEpisodeModel model)
        {
            try
            {
                return RMApiRepository.IsCharacterExistsInEpisode(model.PersonName, model.EpisodeName);
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
                return RMApiRepository.GetCharacterJsonInfoByPersonName(name);
            }
            catch
            {
                return NotFound();
            }

        }
    }
}
