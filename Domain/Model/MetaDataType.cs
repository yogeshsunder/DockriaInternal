using DocumentFormat.OpenXml.Wordprocessing;
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
    public class MetaDataType
    {
        public MetaDataType()
        {
            UserGroupNames = new List<MetaDataTypeUserGroup>();
        }

        public int Id { get; set; }

        [Display(Name = "METADATA TYPE NAME")]
        [Required(ErrorMessage = " * MetaData Type Name Is Required")]
        public string? MetaDataTypeName { get; set; }

        public string? MetaDataDataType { get; set; }

        public string? ContainerName { get; set; } // Add this property to hold containerName

        public ICollection<MetaDataTypeUserGroup>? UserGroupNames { get; set; }

        // Foreign key property for CompanyAdminUser
        public int? CompanyAdminId { get; set; }

        [ForeignKey("CompanyAdminId")]
        // Navigation property
        public CompanyAdminUser? CompanyAdminUser { get; set; }
    }

    public class MetaDataTypeUserGroup
    {
        public int Id { get; set; }
        public int MetaDataTypeId { get; set; }
        public MetaDataType? MetaDataType { get; set; }

        public int UserGroupId { get; set; }
        public UserGroup? UserGroup { get; set; }
    }
}
