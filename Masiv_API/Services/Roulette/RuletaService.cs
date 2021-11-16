using Masiv_API.Models.MBet;
using Masiv_API.Models.MGenericReponse;
using Masiv_API.Models.MRoulette;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Masiv_API.Services.Roulette
{
    public class RuletaService : IRuletaService
    {
        public Task<GenericResponse<Models.MRoulette.Roulette>> CreateRoulette()
        {
            GenericResponse<Models.MRoulette.Roulette> result = new GenericResponse<Models.MRoulette.Roulette>()
            {
                Message = "Creation done successfully",
                Result = new Models.MRoulette.Roulette()
            };

            return Task.Run(() => result);
        }

        public Task<List<Models.MRoulette.Roulette>> RouletteOpening(List<Models.MRoulette.Roulette> cacheLstRoulettes, Guid id)
        {
            cacheLstRoulettes.FirstOrDefault(x => x.Id.Equals(id)).Open = true;
            cacheLstRoulettes.FirstOrDefault(x => x.Id.Equals(id)).OpeningDate = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

            return Task.Run(() => cacheLstRoulettes);
        }

        public Task<GenericResponse<bool>> MakeBet(RouletteBet rouletteBet, List<Models.MRoulette.Roulette> lstRoulettes)
        {
            BetDone doneBet = ValidateBet(rouletteBet, lstRoulettes);
            GenericResponse<bool> result = new GenericResponse<bool>()
            {
                IsSuccesful = doneBet.Done,
                Message = doneBet.Message,
                Result = doneBet.Done
            };

            return Task.Run(() => result);
        }

        public Task<GenericResponse<BetWinnersAndRouletteBet>> ClosedBet(List<Models.MRoulette.Roulette> lstRoulettes, List<RouletteBet> lstBetRoulettes, Guid id)
        {
            GenericResponse<BetWinnersAndRouletteBet> result = new GenericResponse<BetWinnersAndRouletteBet>();
            string msgExistsOrOpen = RouletteExistsOrOpen(id, lstRoulettes, lstBetRoulettes);
            if (string.IsNullOrEmpty(msgExistsOrOpen))
            {
                result.Result = GenerateBettingResult(lstBetRoulettes.Where(x => x.RouletteId.Equals(id)).ToList());
                result.Message = "Roulette closed successfully";
            }
            else
            {
                result.IsSuccesful = false;
                result.Message = msgExistsOrOpen;
            }

            return Task.Run(() => result);
        }

        private BetWinnersAndRouletteBet GenerateBettingResult(List<RouletteBet> lstBet)
        {
            BetWinnersAndRouletteBet resultListsWinners = new BetWinnersAndRouletteBet();
            Random random = new Random();
            resultListsWinners.WinningNumber = random.Next(0, 36);
            resultListsWinners.WinningColor = resultListsWinners.WinningNumber % 2 == 0 ? "rojo" : "negro";
            List<RouletteBet> lstColorWinners = lstBet.Where(x => x.Color.Trim().ToLower().Equals(resultListsWinners.WinningColor)).ToList();
            List<RouletteBet> lstNumbersWinners = lstBet.Where(x => x.Number == resultListsWinners.WinningNumber).ToList();
            if (lstColorWinners.Count > 0)
            {
                resultListsWinners.ListBetWinners.AddRange(CalculateWinningValues("color", lstColorWinners));
            }
            if (lstNumbersWinners.Count > 0)
            {
                resultListsWinners.ListBetWinners.AddRange(CalculateWinningValues("numero", lstNumbersWinners));
            }
            resultListsWinners.ListRouletteBet = lstBet;

            return resultListsWinners;
        }

        private List<BetWinners> CalculateWinningValues(string type, List<RouletteBet> lstWinners)
        {
            List<BetWinners> lstBetWinners = new List<BetWinners>();
            double valueToCalculate = type.Equals("color") ? 1.8 : 5;
            foreach (RouletteBet item in lstWinners)
            {
                BetWinners betWinners = new BetWinners()
                {
                    Bet = item,
                    EarnedValue = item.Value * Convert.ToDecimal(valueToCalculate),
                    IsWinningColor = type.Equals("color"),
                    IsWinningNumber = type.Equals("numero")
                };
                lstBetWinners.Add(betWinners);
            }

            return lstBetWinners;
        }

        private BetDone ValidateBet(RouletteBet rouletteBet, List<Models.MRoulette.Roulette> lstRoulettes)
        {
            BetDone resultDoneBet = new BetDone();
            bool color = false;
            string msg = RouletteExistsOrOpen(rouletteBet.RouletteId, lstRoulettes);
            if (rouletteBet.Color.Trim().ToLower().Equals("rojo"))
            {
                color = true;
            }
            else if (rouletteBet.Color.Trim().ToLower().Equals("negro"))
            {
                color = true;
            }
            if (!string.IsNullOrEmpty(msg))
            {
                resultDoneBet.Message = msg;
            }
            else if (!color)
            {
                resultDoneBet.Message = "Must add red (rojo) or black (negro) color";
            }
            else if (rouletteBet.Number < 0 || rouletteBet.Number > 36)
            {
                resultDoneBet.Message = "You must bet from number 0 to 36";
            }
            else if (rouletteBet.Value <= 0 || rouletteBet.Value > 10000)
            {
                resultDoneBet.Message = "The minimum value is 1 dollar and the maximum value of the bet must be 10.000 dollars";
            }
            else
            {
                resultDoneBet.Done = true;
                resultDoneBet.Message = "Bet made successfully";
            }

            return resultDoneBet;
        }

        private string RouletteExistsOrOpen(Guid id, List<Models.MRoulette.Roulette> lstRoulettes, List<RouletteBet> lstBetRoulettes = null)
        {
            string message = "";
            if (!lstRoulettes.Any(x => x.Id.Equals(id)))
            {
                message = $"Roulette with id: {id}. Does not exist";
            }
            else if (lstRoulettes.Any(x => x.Id.Equals(id) && !x.Open))
            {
                message = $"Roulette with id: {id}. Is closed";
            }
            else if (lstBetRoulettes != null && !lstBetRoulettes.Any(x => x.RouletteId.Equals(id)))
            {
                message = $"No bet has been placed on roulette with id: {id}";
            }

            return message;
        }

    }
}
