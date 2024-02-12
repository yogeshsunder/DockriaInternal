using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.ViewModel
{
    public class UserGroupPermissionVM
    {
        public UserGroup? UserGroup { get; set; }
        public EndUserManagement? EndUserManagement { get; set; }
        public DocumentManagement? DocumentManagement { get; set; }
        public RADManagement? RADManagement { get; set; }        
    }
}
