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
    public class FileNameTypeDocument
    {
        public FileNameTypeDocument()
        {
            MetaDataTypeList = new List<FileNameMetaData>();
        }

        public int Id { get; set; }
        [Display(Name = "FILE NAME")]
        [Required(ErrorMessage = " * FILE Name Is Required")]
        public string? FileName { get; set; }
        public List<FileNameMetaData>? MetaDataTypeList { get; set; }
        public int? CompanyAdminId { get; set; }
        [ForeignKey("CompanyAdminId")]
        // Navigation property
        public CompanyAdminUser? CompanyAdminUser { get; set; }
    }

    public class FileNameMetaData
    {
        public int Id { get; set; }
        [Display(Name = "METADATA NAME")]
        [Required(ErrorMessage = " * Metadata Name Is Required")]
        public string? MetadataName { get; set; }
        public string? Seperator { get; set; }
        // Foreign key property
        public int? FileNameTypeDocumentId { get; set; }
        // Navigation property
        public FileNameTypeDocument? FileNameTypeDocument { get; set; }
    }
}
