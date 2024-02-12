using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.ViewModel
{
    public class DocumentTypeMetadata
    {
        public int Id { get; set; }
        public string? MetadataName { get; set; }
        public string? AspId { get; set; }
        [ForeignKey("CompanyAdminId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
