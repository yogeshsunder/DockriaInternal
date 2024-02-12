using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ContainerTypeData
    {
        public string Id { get; set; }
        public string ContainerName { get; set; }
        public string ContainerDataType { get; set; }
        public List<string> UserGroupNames { get; set; }
        public List<string> MetaDataListNames { get; set; }
    }
}
