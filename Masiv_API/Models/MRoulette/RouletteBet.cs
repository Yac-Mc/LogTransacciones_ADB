using System;
using System.ComponentModel.DataAnnotations;

namespace Masiv_API.Models.MRoulette
{
    public class RouletteBet
    {
        [Required]
        public Guid RouletteId { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public decimal Value { get; set; }
    }
}
