using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Companies.Queries
{
    public class DeleteCompanyCommand : IRequest
    {
        public int CompanyId { get; }

        public DeleteCompanyCommand(int companyId)
        {
            CompanyId = companyId;
        }
    }
}
