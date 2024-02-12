using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
	public class AuditLog
	{
		public int Id { get; set; }				
		public DateTime Date { get; set; }
		public TimeSpan Time { get; set; }        
        public string? Username { get; set; }
        public string? Fullname { get; set; }
        public string? BrowserName { get; set; }
        public string? IPAddress { get; set; }
     // public string? Action { get; set; }
        public string? RecordType { get; set; }
   //     public string? OldData { get; set; }
        public string? NewData { get; set; }
       // public string? Description { get; set; }
    }
}
