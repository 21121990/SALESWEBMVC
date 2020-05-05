using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Models
{
    public class User
    {
        public int id { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Campo {0} Obrigatório")]
        [Display(Name = "E-mail")]
        public string email { get; set; }
        [Required(ErrorMessage = "Campo {0} Obrigatório")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        public User()
        {
        }
        public User(int id, string email, string password)
        {
            this.id = id;
            this.email = email;
            this.password = password;
        }


    }

}
