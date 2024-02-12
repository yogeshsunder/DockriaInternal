using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Model.ViewModel
{
    public class CompanyAdminUser
    {
        [Display(Name = "COMPANY UNIQUE ID")]

        public int Id { get; set; }
        [Display(Name = "COMPANY LEGAL NAME")]
        [Required(ErrorMessage = "* Company Legal Name Is Required.")]
        public string? Name { get; set; }
        [Display(Name = "COMPANY ADDRESS")]
        [Required(ErrorMessage = " * Company Address Is Required.")]
        public string? Address { get; set; }
        [Display(Name = "COMPANY OFFICIAL EMAIL")]
        [Required(ErrorMessage = " * Company Official Email Is Required.")]
        [EmailAddress]
        public string? OfficialEmail { get; set; }
        [Display(Name = "COMPANY PHONE NUMBER")]
        [Required(ErrorMessage = " * Company Phone Number Is Required.")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        public string? PhoneNumber { get; set; }
        [Display(Name = "COMPANY BUSINESS REG. NUMBER")]
        [Required(ErrorMessage = " * Company Business Reg. Number Is Required.")]
        public string? RegistrationNumber { get; set; }
        [Display(Name = "COMPANY PIN NUMBER")]
        [Required(ErrorMessage = " * Company Pin Number Is Required.")]
        public string? PinNumber { get; set; }
        [Display(Name = "EMAIL ADDRESS TO RECEIVE INVOICE")]
        [Required(ErrorMessage = " * Email Address To Receive Invoice Is Required.")]
        [EmailAddress]
        public string? InvoiceMail { get; set; }
        [Display(Name = "EMAIL ADDRESS TO RECEIVE\r\nLOGIN DETAILS")]
        [Required(ErrorMessage = "* Email address to receive login details is required.")]
        [EmailAddress]
        public string? ReciveEmail { get; set; }
        [Display(Name = "COMPANY ADMIN NAME")]
        [Required(ErrorMessage = " * Company Admin Name Is Required.")]
        public string? AdminName { get; set; }
        [Display(Name = "COMPANY ADMIN EMAIL")]
        [Required(ErrorMessage = "* Company Admin Email Is Required.")]
        [EmailAddress]
        public string? AdminEmail { get; set; }
        [Display(Name = "COMPANY ADMIN DESIGNATION")]
        [Required(ErrorMessage = "* Company Admin Designation Is Required.")]
        public string? AdminDesignation { get; set; }
        [Display(Name = "COMPANY ADMIN PHONE\r\nNUMBER")]
        [Required(ErrorMessage = " * Company Admin Phone Number Is Required.")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = " * Please Enter Valid Phone No.")]
        public string? AdminPhoneNumber { get; set; }
        [Display(Name = " * COMPANY OFFICIAL\r\nDOCUMENTS - attach upto 5\r\npdf documents only.")]
        // public string? Document { get; set; }

        public string? FileNames { get; set; }

        public string? SubscriptionName { get; set; }
        [Display(Name = "SUBSCRIPTION GROUP FOR THE\r\nCOMPANY")]
        [Required(ErrorMessage = " * Company Should Not Be Added Without Adding Subscription List")]
        [NotMapped]
        public int? SubscriptionList { get; set; }

        [Display(Name = "STORAGE SPACE - in GB,\r\nminimum of 100gb")]
        [Required(ErrorMessage = " * Storage Space Is Required.")]
        public string? StorageSpace { get; set; }

        [Column(TypeName = "Date")]
        public DateTime DateFrom { get; set; }

        [Column(TypeName = "Date")]
        public DateTime DateTo { get; set; }

        [Display(Name = "VALIDITY IN MONTHS")]
        public string? TotalTime { get; set; }

        public string? AspId { get; set; }
        [ForeignKey("AspId")]
        public ApplicationUser? ApplicationUser { get; set; }
        public string ?Role { get; set; }

    }
}
