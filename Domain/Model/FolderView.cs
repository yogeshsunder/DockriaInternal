using Domain.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class FolderView
    {
        public FolderView()
        {
            RowsData = new List<FolderViewList>();
        }
        [Key]
        public int Id { get; set; }
        public string? FolderViewName { get; set; }
        public virtual ICollection<FolderViewList>? RowsData { get; set; }
        // Foreign key property
        public int? CompanyAdminId { get; set; }

        [ForeignKey("CompanyAdminId")]
        // Navigation property
        public CompanyAdminUser? CompanyAdminUser { get; set; }
    }

    public class FolderViewList
    {
        public int Id { get; set; }
        public string? SelectObject { get; set; }
        public string? ObjectType { get; set; }
        public string? Operator { get; set; }
        public string? MetaDataList { get; set; }
        public string? Value { get; set; }
        // Foreign key property
        public int FolderViewId { get; set; }
        // Navigation property
        public FolderView? FolderView { get; set; }
        
    }
}
