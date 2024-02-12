using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Domain.Model
{
    public class SmgInfo
    {
        [Key]
        [Display(Name = "SMG UNIQUE ID")]
        public int Id { get; set; }

        [Display(Name = "SMG NAME")]
        public string? SmgName { get; set; }

        [Display(Name = "TAX NAME")]
        public string? TaxName { get; set; }

        [Display(Name = "TAX PERCENTAGE")]
        public string? TaxPercentage { get; set; }

        [ForeignKey("PaymentInterval")]
        public int PaymentIntervalId { get; set; }

        public virtual PaymentInterval? PaymentInterval { get; set; }

        [ForeignKey("PaymentCurrency")]
        public int PaymentCurrencyId { get; set; }

        public virtual PaymentCurrency? PaymentCurrency { get; set; }

        [ForeignKey("PaymentType")]
        public int PaymentTypeId { get; set; }

        public virtual PaymentType? PaymentType { get; set; }
    }
}
