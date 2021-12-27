using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeStoreAspCore.Models
{
    public class KM
    {
        public int Id { get; set; }

        public int? DiscountPercent {get;set;}

        public int TimesOfUsed { get; set; }

        public int AvailableTimes { get; set; }

        public decimal? DiscountAmount { get; set; }
        public string CodeText { get; set; }

    }
}
