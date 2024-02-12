using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Domain.Model.ViewModel
{
    public class smgViewModel
    {
        public smgViewModel()
        {
            PaymentCurrencyList = new List<SelectListItem>();
            PaymentIntervalList = new List<SelectListItem>();
            PaymentTypeList = new List<SelectListItem>();
            CartItems = new List<SmgCost>();
        }

        public int Id { get; set; }

        [Display(Name = "SMG NAME")]
        
        [Required(ErrorMessage = " * SMG Name Is Required")]
        //[Remote(action: "IsSmgNameUnique", controller: "Smg", ErrorMessage = "SMG Names should be Unique and not repeated.")]
        public string SmgName { get; set; }

        [Display(Name = "TAX NAME")]
        [Required(ErrorMessage = " * Tax Name Is Required")]
        public string TaxName { get; set; }

        [Display(Name = "TAX PERCENTAGE")]
        [Required(ErrorMessage = "* Tax Percentage Is Required")]
        public string TaxPercentage { get; set; }

        [Required(ErrorMessage = "* Payment Type Name Is Required")]
        public string PaymentTypeName { get; set; }

        [Required(ErrorMessage = " * Payment Currency Name Is Required")]
        public string PaymentCurrencyName { get; set; }

        [Required(ErrorMessage = " * Payment Interval Name Is Required")]
        public string PaymentIntervalName { get; set; }

        public List<SmgCost> CartItems { get; set; }

        [NotMapped]
        [Display(Name = "PAYMENT CURRENCY")]
        public List<SelectListItem> PaymentCurrencyList { get; set; }

        [NotMapped]
        [Display(Name = "PAYMENT INTERVAL")]
        public List<SelectListItem> PaymentIntervalList { get; set; }

        [NotMapped]
        [Display(Name = "PAYMENT TYPE")]
        public List<SelectListItem> PaymentTypeList { get; set; }
    }

    public class SmgCost
    {
        public int Id { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Qty { get; set; }

        [Required(ErrorMessage = "Cost is required")]
        public int Cost { get; set; }

        public decimal Total { get; set; }

    //    public decimal SubTotal { get; set; }

       // public decimal? VatPercentage { get; set; }

       // public decimal Vat { get; set; }

        public decimal GrandTotal { get; set; }

        // Foreign key property
        public int? SmgViewModelId { get; set; }

        // Navigation property
        public smgViewModel? smgViewModel { get; set; }
    }

}