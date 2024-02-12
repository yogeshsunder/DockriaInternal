using Domain.Model.ViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Companies.Queries
{
    public class GetCompanyViewModelsQuery : IRequest<List<CompanyViewModel>>
    {
    }
}
