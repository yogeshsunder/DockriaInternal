using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class EndUserManagement
    {
        public int Id { get; set; }
        public bool EditEmail { get; set; }
        public bool EditPassword { get; set; }
        public bool EditSign { get; set; }        
        public int GroupId { get; set; }
        public UserGroup? UserGroup { get; set; }
    }
}
