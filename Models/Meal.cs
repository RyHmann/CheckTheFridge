using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public List<MealIngredient> Ingredients { get; set; }



        public Meal()
        {
            Id = -1;
            Name = "";
            Description = "";
            Instructions = "";
            Ingredients = new List<MealIngredient>();
        }
        public Meal(int id, string name, string description, string instructions)
        {
            Id = id;
            Name = name;
            Description = description;
            Instructions = instructions;
        }
    }
}