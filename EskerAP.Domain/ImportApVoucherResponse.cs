using System;
using System.Collections.Generic;

namespace EskerAP.Domain
{
	public class ImportApVoucherResponse
	{
		/// <summary>
		/// Indicates whether the import was successful
		/// </summary>
		public bool ImportWasSuccessful => this.SucceededCount > 0;
		
		/// <summary>
		/// Count of successfully imported vouchers
		/// </summary>
		public int SucceededCount { get; set; }

		/// <summary>
		/// Count of failed vouchers
		/// </summary>
		public int Failedcount { get; set; }

		/// <summary>
		/// Count of skipped vouchers
		/// </summary>
		public int SkippedCount { get; set; }

		/// <summary>
		/// Errors that occurred before the ImportApVoucher procedure could be invoked by the database.
		/// </summary>
		public string OtherErrors { get; set; }

		/// <summary>
		/// Errors that occurred with the header of the voucher
		/// </summary>
		public string HeaderErrors { get; set; }

		/// <summary>
		/// Errors that occurred with item lines of the voucher
		/// </summary>
		public List<string> LineErrors { get; set; } = new List<string>();
		
		/// <summary>
		/// Exception that may have been thrown by the program for programmatic errors.
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// Raw Xml response from Famous that contains the original voucher and errors.
		/// </summary>
		public string RawXmlVoucherResponse { get; set; }
	}
}
