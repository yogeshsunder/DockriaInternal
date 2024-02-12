using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class UserLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ?SessionToken { get; set; }


        [Required]
        public string? UserId { get; set; }

        [Required]
        public string? IPAddress { get; set; }

        [Required]
        public string? UserAgent { get; set; }

        [Required]
        public DateTime LoginDateTime { get; set; }

        public DateTime? LogoutDateTime { get; set; }

    }
}
