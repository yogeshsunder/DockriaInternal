using Application.Companies.Queries;
using CleanArchitecture.Application.Common.Exceptions;
using Dockria.Data;
using Domain.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class EditCompanyCommandHandler : IRequestHandler<EditCompanyCommand>
{
    private readonly ApplicationDbContext _context;

    public EditCompanyCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(EditCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId, cancellationToken);

        if (company == null)
        {
            throw new NotFoundException(nameof(Company), request.CompanyId);
        }

        if (request.DocByte != null)
        {
            using (var ms = new MemoryStream())
            {
                await ms.WriteAsync(request.DocByte.AsMemory(0, request.DocByte.Length), cancellationToken);
                company.DocByte = ms.ToArray();
                company.Filename = request.Filename;
                company.FileType = request.FileType;
            }
        }

        company.CompanyName = request.CompanyName;
        company.MobileNumber = request.MobileNumber;
        company.Address = request.Address;
        company.CompanyDomain = request.CompanyDomain;

        var user = company.Users.FirstOrDefault();
        if (user != null)
        {
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Designation = request.Designation;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
