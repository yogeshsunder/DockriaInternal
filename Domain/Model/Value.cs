using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{

    public class ValueInfo
    {
        public bool IsSavedInCurrentSession;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public string? ValueId { get; set; }
        public string? Name { get; set; }

        
        public string ?ContainerId { get; set; }
        public ContainerType? ContainerType { get; set; }
        
    }



}
