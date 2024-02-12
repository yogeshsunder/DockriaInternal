using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.ViewModel
{
    public class CompanyViewModel
    {

        public Company? Company { get; set; }
        public User? User { get; set; }


       
        [NotMapped]
        public IFormFile? DocByte { get; set; }
    }

}
