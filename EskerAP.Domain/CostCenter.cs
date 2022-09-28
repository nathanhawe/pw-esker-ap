namespace EskerAP.Domain
{
	public class CostCenter
	{
		/// <summary>
		/// Internal Unique ID in Famous for logging
		/// </summary>
		public string Idx { get; set; }

		/// <summary>
		/// Cost center identifier.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Unique identifier of the company.
		/// </summary>
		public string CompanyCode { get; set; }

		/// <summary>
		/// Cost center description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Identifier of the user responsible for the cost center.  This user is defined
		/// as as level 1 approver in the approval workflow.
		/// </summary>
		public string Manager { get; set; }
	}
}
