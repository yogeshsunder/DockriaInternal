using Application.Companies.Queries;
using CleanArchitecture.Application.Common.Exceptions;
using Dockria.Data;
using Domain.Model;
using Domain.Model.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Companies.Handlers
{
    public class GetCompanyByIdQueryDeleteHandler : IRequestHandler<GetCompanyByIdDeleteQuery, CompanyViewModel>
    {
        private readonly ApplicationDbContext _context;

        public GetCompanyByIdQueryDeleteHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CompanyViewModel> Handle(GetCompanyByIdDeleteQuery request, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId, cancellationToken);

            if (company == null)
            {
                throw new NotFoundException(nameof(Company), request.CompanyId);
            }

            _context.RemoveRange(company.Users);
            _context.Remove(company);
            await _context.SaveChangesAsync(cancellationToken);

            return new CompanyViewModel
            {
                Company = company,
                User = company.Users.FirstOrDefault()
            };
        }
    }
}

