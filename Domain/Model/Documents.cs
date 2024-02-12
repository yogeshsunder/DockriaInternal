using Domain.Model.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model
{
    public class Documents
    {
        public Documents()
        {
            MetaDataList = new List<MetaData>();
            UserGroupNames = new List<DocumentsUserGroup>();
        }
        public int Id { get; set; }
        [Display(Name = "DOC TYPE NAME")]
        [Required(ErrorMessage = " * Doc Type Name Is Required")]
        public string? DocTypeName { get; set; }
        public bool? OCR { get; set; }
        public bool? VERSIONING { get; set; }
        [Display(Name = "AUTO FILE NAME")]
        //[Required(ErrorMessage = "* AUTO FILE NAME Is Required")]
        public int? FileNameTypeId { get; set; }
        public FileNameTypeDocument? FileNameTypeDocument { get; set; }
        public List<MetaData>? MetaDataList { get; set; }
        public ICollection<DocumentsUserGroup>? UserGroupNames { get; set; }
        // Foreign key property
        public int? CompanyAdminId { get; set; }
        [ForeignKey("CompanyAdminId")]
        // Navigation property
        public CompanyAdminUser? CompanyAdminUser { get; set; }
    }

    public class MetaData
    {
        public int Id { get; set; }
        [Display(Name = "METADATA NAME")]
        [Required(ErrorMessage = " * Metadata Name Is Required")]
        public string? MetadataName { get; set; }
        public bool isRequired { get; set; }
        // Foreign key property
        public int? DocumentsId { get; set; }
        // Navigation property
        public Documents? Documents { get; set; }
    }

    public class DocumentsUserGroup
    {
        public int Id { get; set; }
        public int DocumentsId { get; set; }
        public Documents? Documents { get; set; }
        public int UserGroupId { get; set; }
        public UserGroup? UserGroup { get; set; }
    }

}
