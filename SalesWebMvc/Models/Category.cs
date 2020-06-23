using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Models
{
    public class Category
    {

        public int Id { get; set; }
        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        public Category()
        {

        }
        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
