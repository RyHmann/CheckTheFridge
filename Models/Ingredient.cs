using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public double Price { get; set; }
        public int CalorieCount { get; set; }

        public Ingredient()
        {
            Id = -1;
            Name = "";
            Price = 0;
            CalorieCount = 0;
        }
    }
}