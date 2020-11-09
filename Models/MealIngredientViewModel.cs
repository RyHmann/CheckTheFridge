using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Models
{
    public class MealIngredientViewModel
    {
        public int MealIngredientId { get; set; }
        public int MealId { get; set; }
        public int IngredientId { get; set; }
        public int UnitId { get; set; }
        public string UnitLabel { get; set; }
        public decimal Quantity { get; set; }
        public string Label { get; set; }
        public int FridgeIngredientId { get; set; }

        //Data stored for forms
        public List<Unit> AvailableUnitLabels { get; set; }
        public int SelectedUnitId { get; set; }

        public MealIngredientViewModel()
        {
            IngredientId = -1;
        }
    }
}