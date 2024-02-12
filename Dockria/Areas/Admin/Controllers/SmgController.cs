using Dockria.Data;
using Domain.Model;
using Domain.Model.ViewModel;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Infrastructure.Migrations;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SmgController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SmgController(ApplicationDbContext context)
        {
            _context = context;
        }
        private string GenerateRandomCode()
        {
            return Guid.NewGuid().ToString();
        }

        public IActionResult Index(string code)
        {
            ViewBag.RandomString = code;

            if (string.IsNullOrEmpty(code))
            {
                ViewBag.RandomString = GenerateRandomCode();
                return RedirectToAction("Index", new { code = ViewBag.RandomString });
            }

            var smgs = _context.SmgViewModels.ToList();
            return View(smgs);
        }

        public IActionResult ReloadListPartial()
        {
            var smgs = _context.SmgViewModels.ToList();
            return PartialView("_smgList", smgs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            smgViewModel smgViewModel = new smgViewModel();

            var paylists = _context.PaymentCurrencies
                .Select(x => new { Value = x.PaymentCurrencyId.ToString(), Text = x.PaymentCurrencyName })
                .ToList();
            var payments = _context.PaymentTypes
                .Select(x => new { Value = x.PaymentTypeId.ToString(), Text = x.PaymentTypeName })
                .ToList();

            var payInters = _context.PaymentIntervals
                .Select(x => new { Value = x.PaymentIntervalId.ToString(), Text = x.PaymentIntervalName })
                .ToList();

            var paymentData = new { PaymentTypeList = payments, PaymentIntervalList = payInters, PaymentCurrencyList = paylists };

            return Json(paymentData);
        }



        [HttpPost]
        public IActionResult Create([FromBody] smgViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.SmgViewModels.Any(s => s.SmgName == model.SmgName))
                {
                    return BadRequest("SmgName already exists. Please enter a different name.");
                }
                var smg = new smgViewModel
                {
                    SmgName = model.SmgName,
                    TaxName = model.TaxName,
                    TaxPercentage = model.TaxPercentage,
                    PaymentTypeName = model.PaymentTypeName,
                    PaymentCurrencyName = model.PaymentCurrencyName,
                    PaymentIntervalName = model.PaymentIntervalName,
                    CartItems = model.CartItems.ToList() // Convert the IEnumerable to List
                };

                // Save the smg object to the database
                _context.SmgViewModels.Add(smg);
                _context.SaveChanges();

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                return Json(smg, options); // Return smg as JSON
            }

            return BadRequest();
        }


        [HttpPost]
        public IActionResult CheckSmgName([FromBody] string smgName)
        {
            bool smgNameExists = _context.SmgViewModels.Any(s => s.SmgName == smgName);
            return Json(new { exists = smgNameExists });
        }

        [HttpGet]
        public IActionResult Edit(int smgId)
        {
            var paymentCurrencies = _context.PaymentCurrencies
                .Select(x => new { Value = x.PaymentCurrencyId.ToString(), Text = x.PaymentCurrencyName })
                .ToList();

            var paymentTypes = _context.PaymentTypes
        .Select(x => new { Value = x.PaymentTypeId.ToString(), Text = x.PaymentTypeName })
        .ToList();

            var paymentIntervals = _context.PaymentIntervals
                .Select(x => new { Value = x.PaymentIntervalId.ToString(), Text = x.PaymentIntervalName })
                .ToList();
            var cartItems = _context.SmgCosts.Where(x => x.SmgViewModelId == smgId).ToList();

            var smgdata = _context.SmgViewModels.FirstOrDefault(s => s.Id == smgId);
            var dropdownOptions = new
            {
                PaymentCurrencyList = paymentCurrencies,
                PaymentIntervalList = paymentIntervals,
                PaymentTypeList = paymentTypes,
                smg = smgdata,
                cartItem = cartItems,
            };
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return Json(dropdownOptions, options);
        }
        [HttpPost]
        public IActionResult Edit([FromBody] smgViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data");
            }

            var smg = _context.SmgViewModels.Find(model.Id);

            if (smg == null)
            {
                return NotFound();
            }

            smg.Id = model.Id;
            smg.SmgName = model.SmgName;
            smg.TaxName = model.TaxName;
            smg.PaymentCurrencyName = model.PaymentCurrencyName;
            smg.PaymentIntervalName = model.PaymentIntervalName;
            smg.PaymentTypeName = model.PaymentTypeName;
            smg.TaxPercentage = model.TaxPercentage;
            smg.CartItems = model.CartItems.ToList();

            _context.SmgViewModels.Update(smg);
            _context.SaveChanges(); // Save the changes synchronously

            var response = new
            {
                Smg = smg
            };

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return Json(response, options);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var smg = _context.SmgViewModels.Find(id);
                var cartItems = _context.SmgCosts.Where(x => x.SmgViewModelId == id).ToList();
                if (smg == null)
                {
                    return NotFound();
                }
                if (cartItems.Any())
                {
                    _context.SmgCosts.RemoveRange(cartItems);
                }
                _context.SmgViewModels.Remove(smg);

                _context.SaveChanges();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                return Json(smg, options);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
