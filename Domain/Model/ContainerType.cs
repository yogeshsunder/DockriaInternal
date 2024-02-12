using Domain.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ContainerType
    {
        public ContainerType()
        {
            MetaDataList = new List<ContainerMetaData>();
            UserGroupNames = new List<ContainerUserGroup>();
            Values = new List<ValueInfo>();

        }

        [Key]        
        public string Id { get; set; }
        [Display(Name = "CONTAINER TYPE NAME")]
        [Required(ErrorMessage = " * Container Type Name Is Required")]
        public string ContainerName { get; set; }
        [Display(Name = "CONTAINER DATA TYPE")]
       // public string? ContainerDataType { get; set; }
        public virtual ICollection<ContainerUserGroup>? UserGroupNames { get; set; }
        public virtual List<ContainerMetaData>? MetaDataList { get; set; }
        // Foreign key property
        public int? CompanyAdminId { get; set; }
        [ForeignKey("CompanyAdminId")]
        // Navigation property
        public CompanyAdminUser? CompanyAdminUser { get; set; }


        [JsonIgnore]
        public virtual List<ValueInfo> Values { get; set; }

    }

    public class ContainerMetaData
    {
        public int Id { get; set; }
        [Display(Name = "METADATA NAME")]
        [Required(ErrorMessage = " * Metadata Name Is Required")]
        public string? MetadataName { get; set; }
        public bool isRequired { get; set; }
        // Foreign key property
        public string? ContainerId { get; set; }
        // Navigation property
        public ContainerType? ContainerType { get; set; }
    }
    public class ContainerUserGroup
    {
        public int Id { get; set; }
        public string? ContainerTypeId { get; set; }
        public ContainerType? ContainerType { get; set; }
        public int UserGroupId { get; set; }
        public UserGroup? UserGroup { get; set; }
    }

    

}
