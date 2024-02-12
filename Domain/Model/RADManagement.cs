using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class RADManagement
    {
        public int Id { get; set; }
        public bool RadView { get; set; }
        public bool RadEdit { get; set; }
        public bool RadFormFill { get; set; }        
        public int GroupId { get; set; }
        public UserGroup? UserGroup { get; set; }
    }
}
