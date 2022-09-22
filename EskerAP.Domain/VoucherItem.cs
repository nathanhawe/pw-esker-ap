using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAP.Domain
{
	public class VoucherItem
	{
		public string CostCenterId { get; set; }
		public string GrowBlockId { get; set; }
		public string PhaseId { get; set; }
		public string DepartmentId { get; set; }
		public string GlAccountCode { get; set; }
		public string LineDescription { get; set; }
		public decimal? Hours { get; set; }
		public decimal Quantity { get; set; }
		public decimal Rate { get; set; }
		public decimal Amount => this.Quantity * this.Rate;
		public string ChargeId { get; set; }
		public string PoolId { get; set; }
		public string LotId { get; set; }
		public string LineReference { get; set; }
	}
}
