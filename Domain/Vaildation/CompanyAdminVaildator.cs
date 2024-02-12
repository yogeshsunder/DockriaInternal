using Domain.Model.ViewModel;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Vaildation
{
    public class CompanyAdminVaildator : AbstractValidator<CompanyAdminUser>
    {
        public CompanyAdminVaildator()
        {
            RuleFor(x => x.AdminName).NotEmpty().WithMessage("Admin Name is required.");
        }

    }
}
