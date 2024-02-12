using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    
        public class UserSession
        {
            [Key]
            public int Id { get; set; } // Primary key for the UserSession table
            public string ?UserId { get; set; }
           
        public string ?SessionToken { get; set; }

    }



}
