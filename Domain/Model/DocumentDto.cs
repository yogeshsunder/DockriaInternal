using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string DocTypeName { get; set; }
        public bool OCR { get; set; }
        public bool Versioning { get; set; }
        public string AutoFileName { get; set; }
    }
}
