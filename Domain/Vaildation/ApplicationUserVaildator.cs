using Domain.Model;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Vaildation
{
    public class ApplicationUserVaildator : AbstractValidator<ApplicationUser>
    {
        public ApplicationUserVaildator()
        {

            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required.");

        }

    }
}
