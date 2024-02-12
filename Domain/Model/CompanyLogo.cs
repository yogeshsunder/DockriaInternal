using Domain.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class CompanyLogo
    {
        public int Id { get; set; }
        public byte[] ?LogoData { get; set; }
        public byte[]? IconData { get; set; }
        public int CompanyAdminId { get; set; }
        [ForeignKey("CompanyAdminId")]
        public CompanyAdminUser? CompanyAdminUser { get; set; }


    }
}
