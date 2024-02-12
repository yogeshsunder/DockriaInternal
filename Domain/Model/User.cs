using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public string? Email { get; set; }

        public string? Designation { get; set; }
	
		public int CompanyId { get; set; }

        public virtual Company? Company { get; set; }
    }
}
