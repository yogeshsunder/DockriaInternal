using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

public class EditCompanyCommand : IRequest
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string MobileNumber { get; set; }

    public string Address { get; set; }


    public string? Filename { get; set; }
    public string? FileType { get; set; }
    public byte[]? DocByte { get; set; }
    public  int SignatureLimit { get; set; }    
    [Display(Name = ("Company Domain"))]
    public string? CompanyDomain { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Designation { get; set; }
}
