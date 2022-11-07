using EskerAP.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAP.Domain
{
	public class PaymentTerm
	{
		/// <summary>
		/// Unique identifier of the company
		/// </summary>
		public string CompanyCode { get; set; }

		/// <summary>
		/// Code of the payment terms
		/// </summary>
		public string PaymentTermCode { get; set; }

		/// <summary>
		/// Description of the payment terms
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Total number of days allowed by the vendor to make the payment starting from the reference date (either the
		/// invoice date or the posting date).
		/// </summary>
		public int DayLimit => (this.DUEDATETYPE == DueDateType.InvoiceDatePlusDays ? this.DUEDAYS : 0);

		/// <summary>
		/// Penalty rate applied by vendor if the invoice is not paid within the payment period.
		/// </summary>
		public string LatePaymentFeeRate { get; set; }

		/// <summary>
		/// Date used as a reference to calculate the invoice due date and the discount expiration date.
		/// </summary>
		public string ReferenceDate { get; set; } = Constants.ReferenceDate.InvoiceDate;

		/// <summary>
		/// Day of the month that should be considered as the invoice due date after taking into account Payment period (days)
		/// and End of Month.
		/// </summary>
		public int PaymentDay => (this.DUEDATETYPE == DueDateType.TargetDay || this.DUEDATETYPE == DueDateType.TargetDayNextMonth ? this.DUEDAYS : 0);

		/// <summary>
		/// Makes it possible to calculate the due date of the invoice starting from the last day of the month after adding
		/// Payment period (days) to the reference date.
		/// </summary>
		public bool EndOfMonth => (this.DUEDATETYPE == DueDateType.TargetDayNextMonth);

		/// <summary>
		/// Allows vendors to define a dynamic discount rate in case of early payment.  The discount is said dynamic
		/// because the discous rate varies along a sliding scale based on the payment date.
		/// </summary>
		public bool EnableDynamicDiscounting { get; set; } = false;

		/// <summary>
		/// Discount rate granted by the vendor for an early payment issued 30 days before the invoice due date.
		/// Required. Only available when the Enable dynamic discouing option is selected.
		/// </summary>
		public string DiscountRate30Days { get; set; }

		/// <summary>
		/// Period during which a discount is offered on the total amount of the invoice by the vendor.
		/// Only available when the Enable dynamic discounting option is is unselected.
		/// </summary>
		public string DiscountPeriod { get; set; }

		/// <summary>
		/// Discount rate granted by the vendor if the invoice is paid within the defined discount period.
		/// </summary>
		public string DiscountRate { get; set; }

		
		/// <summary>
		/// Value from AP_PAY_TERMS.DUEDATETYPE used to determine other due date values.
		/// </summary>
		public string DUEDATETYPE { get; set; }

		/// <summary>
		/// Value from AP_PAY_TERMS.DUEDAYS used to determine term based on DUEDATETYPE.
		/// </summary>
		public int DUEDAYS { get; set; }
	}
}
