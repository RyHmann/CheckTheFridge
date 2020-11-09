using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Models
{
    public class Unit
    {
        public int unit_id { get; set; }

        [Required]
        public string label { get; set; }
        
    }
}