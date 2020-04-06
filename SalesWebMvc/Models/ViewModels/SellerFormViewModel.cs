using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SalesWebMvc.Models.ViewModels
{
    public class SellerFormViewModel
    {
        public Seller Seller { get; set; }
        public Product Product { get; set; }
        public SalesRepository SalesRepository { get; set; }
        public SalesRecord SalesRecord { get; set; }
        public ICollection<Seller> SellerColection { get; set; }

        public ICollection<Department> Departments { get; set; }

        public ICollection<Product> ProductsColetion { get; set; }

        public ICollection<SalesRecord> SalesRecords { get; set; }

        public ICollection<SalesRepository> SalesRepositoryList { get; set; }


    }
}
