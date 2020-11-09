using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Models
{
    public class MealViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public string Instructions { get; set; }

        public List<MealIngredientViewModel> Ingredients { get; set; }
    }
}