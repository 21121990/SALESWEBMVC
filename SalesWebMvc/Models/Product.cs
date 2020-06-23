using System.ComponentModel.DataAnnotations;

namespace SalesWebMvc.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Ativo")]
        public bool Situation { get; set; }

        [Required]
        [Display(Name = "Valor")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double Value { get; set; }

        [Display(Name = "Categoria")]
        public Category Category{get; set;}

        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "Campo {0} Obrigatório")]
        public int CategoryId { get; set; }
       

        public Product()
        {

        }

        public Product(int id, string name, bool situation, double value, Category category)
        {
            Id = id;
            Name = name;
            Situation = situation;
            Value = value;
            Category = category;
        }
    }
}
