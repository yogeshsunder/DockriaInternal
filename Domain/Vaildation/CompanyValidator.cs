using Domain.Model;
using Domain.Model.ViewModel;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Vaildation
{
    public class CompanyValidator : AbstractValidator<CompanyViewModel>
    {
        public CompanyValidator()
        {
            RuleFor(c => c.Company.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(50).WithMessage("Company name must be less than or equal to 50 characters.");

            RuleFor(c => c.Company.MobileNumber)
                .NotEmpty().WithMessage("Mobile number is required.")
                .Matches(@"^\+[1-9]\d{1,14}$").WithMessage("Mobile number must be a valid international phone number.");

            RuleFor(c => c.Company.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(100).WithMessage("Address must be less than or equal to 100 characters.");

            RuleFor(c => c.Company.CompanyDomain)
                .NotEmpty().WithMessage("Company domain is required.")
                .Matches(@"^([a-zA-Z0-9]+(-[a-zA-Z0-9]+)*\.)+[a-zA-Z]{2,}$").WithMessage("Company domain must be a valid domain name.");

            RuleFor(c => c.Company.SignatureLimit)
                .InclusiveBetween(1, 10).WithMessage("Signature limit must be between 1 and 10.");

                RuleFor(x => x.User.FirstName).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.User.LastName).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.User.Email).NotNull().NotEmpty().EmailAddress();
            RuleFor(x => x.User.Designation).MaximumLength(50);
            
        }
    }

    }
