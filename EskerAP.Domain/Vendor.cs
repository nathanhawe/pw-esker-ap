namespace EskerAP.Domain
{
	public class Vendor
	{
		/// <summary>
		/// The unique identifier for the specific company defined in Esker
		/// </summary>
		public string CompanyCode { get; set; }

		/// <summary>
		/// The unique identifier of the vendor
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Name of the vendor
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Internationalized phone number (e.g., +33123457896)
		/// </summary>
		public string PhoneNumber { get; set; }

		/// <summary>
		/// Internationalized fax Number (e.g., +33123457896)
		/// </summary>
		public string FaxNumber { get; set; }

		/// <summary>
		/// Internationalized VAT number
		/// </summary>
		public string VATNumber { get; set; }

		/// <summary>
		/// Unique nine-digit identifier of the vendor's company
		/// </summary>
		public string DUNSNumber { get; set; }

		/// <summary>
		///  Default invoice type emitted by this vendor.
		/// </summary>
		public string PreferredInvoiceType { get; set; }

		/// <summary>
		/// Code for the vendor's payment terms
		/// </summary>
		public string PaymentTermsCode { get; set;}

		/// <summary>
		/// Vendor's email address
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Number of the collective G/L account associated with one or several vendors
		/// </summary>
		public string GeneralAccount { get; set; }

		/// <summary>
		/// code of the tax system applicable to the vendor
		/// </summary>
		public string TaxSystem { get; set; }

		/// <summary>
		/// Code for the vendor's currency.
		/// </summary>
		public string Currency { get; set; }

		/// <summary>
		/// Specifies whether the parafiscal tax is application to the vendor or not
		/// </summary>
		public bool ParafiscalTax { get; set; } = false;

		/// <summary>
		/// Code for the vendor's VAT type.
		/// </summary>
		public string SupplierDue { get; set; }

		/// <summary>
		/// Address additional details
		/// </summary>
		public string Sub { get; set; }

		/// <summary>
		/// Vendor's street address
		/// </summary>
		public string Street { get; set; }

		/// <summary>
		/// Vendor's locality or post office box
		/// </summary>
		public string PostOfficeBox { get; set; }

		/// <summary>
		/// Vendor's city
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// Vendor's ZIP code
		/// </summary>
		public string PostalCode { get; set; }

		/// <summary>
		/// State or region code of the vendor
		/// </summary>
		public string Region { get; set; }

		/// <summary>
		/// Vendor's country (ISO 3166-1 code)
		/// </summary>
		public string Country { get; set; }
	}
}
