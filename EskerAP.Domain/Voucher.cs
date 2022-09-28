using System;
using System.Collections.Generic;
using System.Linq;

namespace EskerAP.Domain
{
	public class Voucher
	{
		public string VendorId { get; set; }
		public string InvoiceNumber { get; set; }
		public char HoldFlag { get; set; } = 'N';
		public DateTime InvoiceDate { get; set; }
		public string StubDescription { get; set; }
		public string PayTerms { get; set; }
		public DateTime? DueDate { get; set; }
		public DateTime? DiscountDate { get; set; }
		public DateTime? PayByDate { get; set; }
		public decimal DiscountAmount { get; set; }
		public string AccessGroupName { get; set; }
		public string PoSourceNumber { get; set; }
		public string AP1099Code { get; set; }
		public decimal VoucherAmount => this.Lines.Sum(s => s.Amount);
		public string VoucherImportStatus { get; set; } = Constants.VoucherImportStatus.ImportReady;
		public char AllowDuplicateVendorInvoice { get; set; } = 'N';
		public int LineCount => this.Lines.Count();
		public int NoteCount => this.Notes.Count();
		public List<VoucherItem> Lines { get; set; } = new List<VoucherItem>();
		public List<VoucherNote> Notes { get; set; } = new List<VoucherNote>();
	}
}
