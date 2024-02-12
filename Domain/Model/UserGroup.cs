using Domain.Model.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Model
{
    public class UserGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserGroupId { get; set; }

        public string? UserGroupName { get; set; }

        public string? UserName { get; set; }

        [NotMapped]
        public List<string>? UsersList { get; set; }
        public int CompanyAdminId { get; set; }
        [ForeignKey("CompanyAdminId")]
        public CompanyAdminUser? CompanyAdminUser { get; set; }
    }
}
