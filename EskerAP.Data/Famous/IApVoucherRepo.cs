﻿using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public interface IApVoucherRepo
	{
		public IEnumerable<EskerAP.Domain.PaidInvoice> GetPaidInvoices(int daysPast);
	}
}