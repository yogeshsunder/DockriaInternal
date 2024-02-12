using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Profile
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NickName { get; set; }
        public string? About { get; set; }
        public string? Address { get; set; }
        public string? Designation { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? ImgUrl { get; set; }
        public byte[]? ImageData { get; set; }
        public string? TwitterAct { get; set;}
        public string? FacebookAct { get; set;}
        public string? GooglePlusAct { get; set;}
        public string? LinkedAct { get; set;}
        public string? AspId { get; set; }
        [ForeignKey("AspId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
