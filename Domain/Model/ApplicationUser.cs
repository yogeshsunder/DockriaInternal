using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Model
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = " * Full Name Is Required.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = " * Email Is Required.")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = " * Confirm Email Is Required.")]
        [EmailAddress]
        [Compare("EmailAddress")]
        public string ConfirmEmailAddress { get; set; }

        [Required(ErrorMessage = " * Phone Number Is Required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = " * Invalid phone number. Please enter a 10-digit numeric phone number.")]
        [Phone(ErrorMessage = " * Invalid phone number format.")]
        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }


        public DateTime LastLoginDate { get; set; } = DateTime.Now;

        [NotMapped]
        public string? Role { get; set; }

    }





}

