using Application.Companies.Queries;
using Domain.Model.ViewModel;
using Domain.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dockria.Data;

namespace Application.Companies.Handlers
{
    public class GetCompanyViewModelsQueryHandler : IRequestHandler<GetCompanyViewModelsQuery, List<CompanyViewModel>>
    {
        private readonly ApplicationDbContext _context;

        public GetCompanyViewModelsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyViewModel>> Handle(GetCompanyViewModelsQuery request, CancellationToken cancellationToken)
        {
            var companies = await _context.Set<Company>().ToListAsync();

            var viewModels = companies.Select(company => new CompanyViewModel
            {
                Company = company,
                User = _context.Set<User>().FirstOrDefault(user => user.CompanyId == company.CompanyId)
            }).ToList();

            return viewModels;
        }

    }
}

