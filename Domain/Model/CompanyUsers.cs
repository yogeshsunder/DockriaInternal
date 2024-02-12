using Domain.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class CompanyUsers
    {
        public int Id { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string ?Department { get; set; }
        [Required]
        public string ?MobileNumber { get; set; }
        // this is for getting CompanyAdminId
        public string? UserId { get; set; }        
        public string? AspId { get; set; }
        [ForeignKey("AspId")]
        public ApplicationUser ?ApplicationUser { get; set; }
        public bool ReadPermission { get; set; }
        public bool WritePermission { get; set; }
        public bool UploadPermission { get; set; }
    }
}
