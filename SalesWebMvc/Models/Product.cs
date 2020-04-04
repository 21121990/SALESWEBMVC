using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [Display(Name="Nome")]
        public string Name { get; set; }

        [Required]
        [Display(Name ="Valor")]
        public double Value { get; set; }


        public Product()
        {

        }
        public Product(int id, string name, double value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

       
    }
}
