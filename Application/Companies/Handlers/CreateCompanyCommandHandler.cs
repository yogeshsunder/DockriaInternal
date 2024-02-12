using Application.Companies.Queries;
using Dockria.Data;
using Domain.Model;
using Domain.Model.ViewModel;

using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace Application.Companies.Handlers
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyViewModel>
    {
        private readonly ApplicationDbContext _context;
        public CreateCompanyCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CompanyViewModel> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {

            var viewModel = request.ViewModel;
            var company = viewModel.Company;
            var users = viewModel.User;

            if (request.ViewModel.DocByte != null && request.ViewModel.DocByte.Length > 0)
            {
                byte[] p1 = null;
                using (var ms1 = new MemoryStream())
                {
                    await request.ViewModel.DocByte.CopyToAsync(ms1);
                    p1 = ms1.ToArray();
                }
                viewModel.Company.DocByte = p1;
            }

            // Add User details
            users.FirstName = viewModel.User.FirstName;
            users.LastName = viewModel.User.LastName;
            users.Email = viewModel.User.Email;
            users.Designation = viewModel.User.Designation;

            // Add Company details
            company.CompanyName = viewModel.Company.CompanyName;
            company.MobileNumber = viewModel.Company.MobileNumber;
            company.Address = viewModel.Company.Address;
            company.Filename = viewModel.DocByte.FileName;
            company.FileType = viewModel.DocByte.ContentType;
            company.CompanyDomain = viewModel.Company.CompanyDomain;
            company.SignatureLimit= viewModel.Company.SignatureLimit;   

            // Add the company to the context and save changes
            _context.Add(company);
            await _context.SaveChangesAsync();

            // Set the company ID on the user entity
            viewModel.User.CompanyId = company.CompanyId;

            // Add the user to the context and save changes
            _context.Add(users);
            await _context.SaveChangesAsync();

            return viewModel;
        }





    }
}









    
