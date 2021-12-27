using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeStoreAspCore.ViewModels.Catalog.Drinks
{
   public class DrinkRequest
    {
        public string Name { get; set; }
        public int UnitPrice { get; set; }
        public int IdMenu { get; set; }

        public string fileName { get; set; }
    }
}
