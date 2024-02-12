using Domain.Model.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Companies.Queries
{
    public class EditCompanyRequest : IRequest<bool>
    {
        public int CompanyId { get; set; }
        public CompanyViewModel ViewModel { get; set; }
        public IFormFile DocByte { get; set; }
    }

}
