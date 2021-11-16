using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Masiv_API.Models.MBet;
using Masiv_API.Models.MGenericReponse;
using Masiv_API.Models.MRoulette;
using Masiv_API.Services.Roulette;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Masiv_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class RouletteController : ControllerBase
    {
        private readonly IRuletaService _ruletaService;
        private readonly IMemoryCache _memoryCache;

        public RouletteController(IRuletaService ruletaService, IMemoryCache memoryCache)
        {
            _ruletaService = ruletaService;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Method that creates betting roulette
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("createroulette")]
        public async Task<ActionResult> CreateRoulette()
        {
            GenericResponse<Roulette> response = await _ruletaService.CreateRoulette();
            List<Roulette> lstRoulettes = new List<Roulette>
            {
                response.Result
            };
            List<Roulette> cacheLstRoulettes = (List<Roulette>)_memoryCache.Get("roulettes");
            if (cacheLstRoulettes == null)
            {
                _memoryCache.GetOrCreate("roulettes", cacheEntry =>
                {
                    cacheEntry.AbsoluteExpiration = DateTime.Now.AddDays(1);
                    return lstRoulettes;
                });
            }
            else
            {
                lstRoulettes.AddRange(cacheLstRoulettes);
                SetMemoryCacheRoulettes(lstRoulettes);
            }

            return Ok(response);
        }

        /// <summary>
        /// Method that opens the bet roulette by id
        /// </summary>
        /// <param name="id">Guid that identifies the roulette</param>
        /// <returns></returns>
        [HttpGet]
        [Route("rouletteopening")]
        public async Task<ActionResult> RouletteOpening(Guid id)
        {
            GenericResponse<Roulette> response = new GenericResponse<Roulette>();
            List<Roulette> cacheLstRoulettes = (List<Roulette>)_memoryCache.Get("roulettes");
            if (cacheLstRoulettes.Any(x => x.Id.Equals(id)))
            {
                cacheLstRoulettes = await _ruletaService.RouletteOpening(cacheLstRoulettes, id);
                SetMemoryCacheRoulettes(cacheLstRoulettes);
                response.Message = "Open roulette";
                response.Result = cacheLstRoulettes.FirstOrDefault(x => x.Id.Equals(id));
            }
            else
            {
                response.Message = $"Roulette with id: {id}. Does not exist";
                response.IsSuccesful = false;
            }
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Method what assigns the bet to roulette
        /// </summary>
        /// <param name="rouletteBet"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("startbet")]
        public async Task<ActionResult> StartBetOnRoulette([FromBody] RouletteBet rouletteBet)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            List<Roulette> cacheLstRoulettes = (List<Roulette>)_memoryCache.Get("roulettes");
            if (cacheLstRoulettes != null)
            {
                response = await _ruletaService.MakeBet(rouletteBet, cacheLstRoulettes);
                if (response.IsSuccesful)
                {
                    AddOrDeleteRouletteBetInCache(rouletteBet);
                }
            }
            else
            {
                response.IsSuccesful = false;
                response.Result = false;
                response.Message = "No roulettes created";
            }
            return Ok(response);
        }

        /// <summary>
        /// Method that closes bets and extracts the list of winners and bets placed from roulette
        /// </summary>
        /// <param name="id">Guid that identifies the roulette</param>
        /// <returns></returns>
        [HttpPost]
        [Route("closedbet")]
        public async Task<ActionResult> ClosedBetOnRoulette(Guid id)
        {
            GenericResponse<BetWinnersAndRouletteBet> response = new GenericResponse<BetWinnersAndRouletteBet>();
            List<Roulette> cacheLstRoulettes = (List<Roulette>)_memoryCache.Get("roulettes");
            List<RouletteBet> cacheLstBetRoulettes = (List<RouletteBet>)_memoryCache.Get("betroulettes");
            if (cacheLstRoulettes == null || cacheLstBetRoulettes == null)
            {
                response.IsSuccesful = false;
                response.Message = cacheLstRoulettes == null ? "No roulettes created" : "No bets have been generated on the roulettes";
            }
            else
            {
                response = await _ruletaService.ClosedBet(cacheLstRoulettes, cacheLstBetRoulettes, id);
                if (response.IsSuccesful)
                {
                    cacheLstRoulettes.FirstOrDefault(x => x.Id.Equals(id)).Open = false;
                    cacheLstRoulettes.FirstOrDefault(x => x.Id.Equals(id)).ClosedDate = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
                    SetMemoryCacheRoulettes(cacheLstRoulettes);
                    AddOrDeleteRouletteBetInCache(cacheLstBetRoulettes.FirstOrDefault(x => x.RouletteId.Equals(id)), false);
                }
            }

            return Ok(response);
        }

        /// <summary>
        /// Method that extracts the list of roulettes created
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallroulettes")]
        public async Task<ActionResult> GetAllRoulettes()
        {
            GenericResponse<IEnumerable<Roulette>> response = new GenericResponse<IEnumerable<Roulette>>();
            var cacheRoulettes = _memoryCache.Get("roulettes");
            if (cacheRoulettes == null)
            {
                response.Message = "There are no roulette";
                response.IsSuccesful = false;
            }
            else
            {
                List<Roulette> lstRoulettes = (List<Roulette>)cacheRoulettes;
                response.Result = lstRoulettes;
            }

            return Ok(response);
        }

        private void SetMemoryCacheRoulettes(List<Roulette> lstRoulettes)
        {
            _memoryCache.Set("roulettes", lstRoulettes, DateTime.Now.AddDays(1));
        }

        private void AddOrDeleteRouletteBetInCache(RouletteBet rouletteBet, bool create = true)
        {
            List<RouletteBet> cacheLstBetRoulettes = (List<RouletteBet>)_memoryCache.Get("betroulettes");
            if (create)
            {
                List<RouletteBet> lstBetRoulette = new List<RouletteBet>
                {
                    rouletteBet
                };
                if (cacheLstBetRoulettes == null)
                {
                    _memoryCache.GetOrCreate("betroulettes", cacheEntry =>
                    {
                        cacheEntry.AbsoluteExpiration = DateTime.Now.AddDays(1);
                        return lstBetRoulette;
                    });
                }
                else
                {
                    lstBetRoulette.AddRange(cacheLstBetRoulettes);
                    _memoryCache.Set("betroulettes", lstBetRoulette, DateTime.Now.AddDays(1));
                }
            }
            else
            {
                cacheLstBetRoulettes.RemoveAll(x => x.RouletteId.Equals(rouletteBet.RouletteId));
                _memoryCache.Set("betroulettes", cacheLstBetRoulettes, DateTime.Now.AddDays(1));
            }
        }
    }
}
