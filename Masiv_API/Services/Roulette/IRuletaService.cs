using Masiv_API.Models.MBet;
using Masiv_API.Models.MGenericReponse;
using Masiv_API.Models.MRoulette;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Masiv_API.Services.Roulette
{
    public interface IRuletaService
    {
        Task<GenericResponse<Models.MRoulette.Roulette>> CreateRoulette();
        Task<List<Models.MRoulette.Roulette>> RouletteOpening(List<Models.MRoulette.Roulette> cacheLstRoulettes, Guid id);
        Task<GenericResponse<bool>> MakeBet(RouletteBet rouletteBet, List<Models.MRoulette.Roulette> lstRoulettes);
        Task<GenericResponse<BetWinnersAndRouletteBet>> ClosedBet(List<Models.MRoulette.Roulette> lstRoulettes, List<RouletteBet> lstBetRoulettes, Guid id);
    }
}
