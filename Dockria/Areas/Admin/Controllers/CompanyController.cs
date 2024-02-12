using Domain.Model.ViewModel;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dockria.Data;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using NuGet.ContentModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using NuGet.Versioning;
using Dockria.Models;

using System.Data.Common;
using System.Drawing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using System.ComponentModel;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using MediatR;
using Application.Companies.Queries;
using Application.Companies.Handlers;

using System.Collections.Generic;
using CleanArchitecture.Application.Common.Exceptions;

namespace WebApp.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CompanyController(IMediator mediator, ApplicationDbContext context)
        {
            _context = context;
            _mediator = mediator;



        }



        public async Task<IActionResult> Index()
        {
            var companies = await _mediator.Send(new GetCompanyViewModelsQuery());

            return View(companies);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CompanyViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CompanyViewModel viewModel, IFormFile docByte)
        {
            viewModel.DocByte = docByte;

            if (ModelState.IsValid)
            {
                var command = new CreateCompanyCommand(viewModel);
                var result = await _mediator.Send(command);

                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var company = await _mediator.Send(new GetCompanyByIdQuery { Id = id });

                return View(company);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CompanyViewModel viewModel, IFormFile docbyte)
        {
            if (ModelState.IsValid)
            {
                var request = new EditCompanyRequest
                {
                    CompanyId = id,
                    ViewModel = viewModel,
                    DocByte = docbyte
                };

                await _mediator.Send(request);

                return RedirectToAction("Index");
            }

            var company = await _context.Companies
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.CompanyId == id);

            if (company == null)
            {
                return NotFound();
            }

            viewModel.Company = company;
            viewModel.User = company.Users.FirstOrDefault();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var query = new GetCompanyByIdDeleteQuery(id);
            var viewModel = await _mediator.Send(query);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CompanyViewModel viewModel)
        {
            var command = new DeleteCompanyCommand(id);
            await _mediator.Send(command);

            return RedirectToAction("Index");
        }
    }

}


