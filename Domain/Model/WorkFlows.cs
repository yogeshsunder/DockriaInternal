using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class WorkFlows
    {
        [Key]
        public int Id { get; set; }

        public string ?Name { get; set; }

        public string  ?Description { get; set; }

        public string ?UserGroupName { get; set; }
        public string  ?VisibilityWorkFlow { get; set; }    

        public string? startingWorkFlow { get; set; }
    }
}
