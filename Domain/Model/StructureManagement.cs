using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class StructureManagement
    {
        public int Id { get; set; }
        public bool DocTypeAdd { get; set; }
        public bool DocTypeEdit { get; set; }        
        public bool DocTypeDelete { get; set; }
        public bool CabinetAdd { get; set; }
        public bool CabinetEdit { get; set; }
        public bool CabinetDelete { get; set; }
        public bool ManageFolder { get; set; }
        public bool LMS { get; set; }
        public bool HtmlForms { get; set; }
        public int GroupId { get; set; }
        public UserGroup? UserGroup { get; set; }
    }
}
