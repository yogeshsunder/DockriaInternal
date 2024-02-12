using Domain.Model.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string MobileNumber { get; set; }

        public string Address { get; set; }


        public string? Filename { get; set; }
        public string? FileType { get; set; }
        public byte[]? DocByte { get; set; }
        [Display(Name =("Company Domain"))]
        public string ?CompanyDomain { get; set; }   

        public List<User> ?Users { get; set; }
        
        public int SignatureLimit { get; set; } 
        

	}
	
}
