using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Estate.Models
{
    public class DataJson
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string value { get; set; }
    }
}