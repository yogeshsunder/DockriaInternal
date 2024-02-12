using Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Domain.Model.ViewModel;
using Microsoft.AspNetCore.Http;

namespace Application.Companies.Queries
{
    public class CreateCompanyCommand : IRequest<CompanyViewModel>

    {
        //public Company? Company { get; set; }    

        //public User? User { get; set; }
        //public IFormFile docbyte;

        public CompanyViewModel ViewModel { get; set; }

        public CreateCompanyCommand(CompanyViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }

}
