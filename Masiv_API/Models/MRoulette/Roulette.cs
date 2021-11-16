using System;
using System.Globalization;

namespace Masiv_API.Models.MRoulette
{
    public class Roulette
    {
        public Guid Id { get; } = Guid.NewGuid();
        public bool Open { get; set; } = false;
        public string CreationDate { get; } = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        public string OpeningDate { get; set; }
        public string ClosedDate { get; set; }
    }
}
