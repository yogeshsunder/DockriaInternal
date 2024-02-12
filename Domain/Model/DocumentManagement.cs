using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class DocumentManagement
    {
        public int Id { get; set; }
        public bool ViewDoc { get; set; }
        public bool DocSinAdd { get; set; }
        public bool DocMulAdd { get; set; }
        public bool DocCopy { get; set; }
        public bool DocMove { get; set; }        
        public bool DocDelete { get; set; }
        public bool DocRename { get; set; }
        public bool DocPrivate { get; set; }
        public bool DocDown { get; set; }
        public bool DocPrint { get; set; }
        public bool ViewMatadata { get; set; }
        public bool EditMatadata { get; set; }
        public bool ShareDocInt { get; set; }
        public bool ShareDocExt { get; set; }
        public bool ShareSigExt { get; set; }
        public bool AuditLogDoc { get; set; }
        public bool DocVerView { get; set; }
        public bool DocRollBack { get; set; }
        public bool DownCsvRpt { get; set; }
        public bool AuditLogUser { get; set; }
        public bool AsgnDocUser { get; set; }
        public bool MaxDocUpSize { get; set; }        
        public string? MaxDocUpNum { get; set; }        
        public int GroupId { get; set; }
        public UserGroup? UserGroup { get; set; }
    }
}
