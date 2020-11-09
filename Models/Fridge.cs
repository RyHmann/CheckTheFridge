using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Models
{
    public class Fridge
    {
        public List<MealIngredientViewModel> Contents { get; set; }

        public Fridge()
        {
            Contents = new List<MealIngredientViewModel>();
        }
    }
}