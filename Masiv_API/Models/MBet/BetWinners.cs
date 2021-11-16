using Masiv_API.Models.MRoulette;

namespace Masiv_API.Models.MBet
{
    public class BetWinners
    {
        public RouletteBet Bet { get; set; }
        public bool IsWinningNumber { get; set; }
        public bool IsWinningColor { get; set; }
        public decimal EarnedValue { get; set; }
    }
}
