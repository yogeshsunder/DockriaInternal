using CleanArchitecture.Application.Common.Exceptions;
using Dockria.Data;
using Domain.Model.ViewModel;
using Domain.Model;
using MediatR;
using Application.Companies.Queries;
using Microsoft.EntityFrameworkCore;

public class GetCompanyForEditQueryHandler : IRequestHandler<GetCompanyByIdQuery, CompanyViewModel>
{
    private readonly ApplicationDbContext _context;

    public GetCompanyForEditQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyViewModel> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.CompanyId == request.Id);

        if (company == null)
        {
            throw new NotFoundException(nameof(Company), request.Id);
        }

        var cmViewModel = new CompanyViewModel
        {
            Company = company,
            User = company.Users.FirstOrDefault(),
        };

        return cmViewModel;
    }
}
