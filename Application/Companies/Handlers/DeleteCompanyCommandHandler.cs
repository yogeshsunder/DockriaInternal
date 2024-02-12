using Application.Companies.Queries;
using Dockria.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Companies.Handlers
{
    public class DeleteCompanyCommandHandler : AsyncRequestHandler<DeleteCompanyCommand>
    {
        private readonly ApplicationDbContext _context;

        public DeleteCompanyCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override async Task Handle(DeleteCompanyCommand command, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.CompanyId == command.CompanyId, cancellationToken);

            if (company == null)
            {
                return;
            }

            _context.RemoveRange(company.Users);
            _context.Remove(company);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

}
