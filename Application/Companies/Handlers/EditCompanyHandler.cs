using Application.Companies.Queries;
using CleanArchitecture.Application.Common.Exceptions;
using Dockria.Data;
using Domain.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Companies.Handlers
{
    public class EditCompanyHandler : IRequestHandler<EditCompanyRequest, bool>
    {
        private readonly ApplicationDbContext _context;

        public EditCompanyHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(EditCompanyRequest request, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId);

            if (company == null)
            {
                throw new NotFoundException(nameof(Company), request.CompanyId);
            }

            // Update the company data with the values from the view model
            company.CompanyName = request.ViewModel.Company.CompanyName;
            company.Address = request.ViewModel.Company.Address;
            company.MobileNumber = request.ViewModel.Company.MobileNumber;
            company.CompanyDomain = request.ViewModel.Company.CompanyDomain;
            company.SignatureLimit = request.ViewModel.Company.SignatureLimit;

            // Update file data
            if (request.DocByte != null && request.DocByte.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    request.DocByte.CopyTo(ms);
                    company.DocByte = ms.ToArray();
                    company.Filename = request.DocByte.FileName;
                    company.FileType = request.DocByte.ContentType;
                }
            }

            // Update the user data with the values from the view model
            var user = company.Users.FirstOrDefault();
            if (user != null)
            {
                user.FirstName = request.ViewModel.User.FirstName;
                user.LastName = request.ViewModel.User.LastName;
                user.Email = request.ViewModel.User.Email;
                user.Designation = request.ViewModel.User.Designation;
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
