using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Models
{
    public class MealIngredient : Ingredient
    {
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public int Unit_id { get; set; }
        public int Ingredient_id { get; set; }
        public MealIngredient()
        {
            Id = -1;
            Name = "";
            Price = 0;
            CalorieCount = 0;
            Quantity = 0;
            Unit = "";
            Unit_id = -1;
            Ingredient_id = -1;
        }
    }
}