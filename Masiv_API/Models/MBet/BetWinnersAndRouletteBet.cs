using Masiv_API.Models.MRoulette;
using System.Collections.Generic;

namespace Masiv_API.Models.MBet
{
    public class BetWinnersAndRouletteBet
    {
        public int WinningNumber { get; set; }
        public string WinningColor { get; set; }
        public List<RouletteBet> ListRouletteBet { get; set; } = new List<RouletteBet>();
        public List<BetWinners> ListBetWinners { get; set; } = new List<BetWinners>();
    }
}
