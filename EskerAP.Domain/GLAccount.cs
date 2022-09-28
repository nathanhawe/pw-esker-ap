using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAP.Domain
{
	public class GLAccount
	{
		/// <summary>
		/// Unique identifier of the company
		/// </summary>
		public string CompanyCode { get; set; }

		/// <summary>
		/// Account identifier
		/// </summary>
		public string Account { get; set; }

		/// <summary>
		/// Account description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Expense category description
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// Indicates whether the account should be allocated to teh analytical axis 1 or not
		/// </summary>
		public bool Allocable1 { get; set; } = false;

		/// <summary>
		/// Indicates whether the account should be allocated to teh analytical axis 2 or not
		/// </summary>
		public bool Allocable2 { get; set; } = false;

		/// <summary>
		/// Indicates whether the account should be allocated to teh analytical axis 3 or not
		/// </summary>
		public bool Allocable3 { get; set; } = false;

		/// <summary>
		/// Indicates whether the account should be allocated to teh analytical axis 4 or not
		/// </summary>
		public bool Allocable4 { get; set; } = false;

		/// <summary>
		/// Indicates whether the account should be allocated to teh analytical axis 5 or not
		/// </summary>
		public bool Allocable5 { get; set; } = false;

		/// <summary>
		/// Identifier of the user responsible for the G/L account (e.g., john.doe@example.com).
		/// This user is defined as a level 1 approver in the approval workflow
		/// </summary>
		public string Manager { get; set; }
	}
}
