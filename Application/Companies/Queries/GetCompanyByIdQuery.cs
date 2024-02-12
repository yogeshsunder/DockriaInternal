using Domain.Model.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Companies.Queries
{
    public class GetCompanyByIdQuery : IRequest<CompanyViewModel>
    {
        public int Id { get; set; }
        

    }
}
