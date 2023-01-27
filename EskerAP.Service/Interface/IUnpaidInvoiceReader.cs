using EskerAP.Domain;
using System.Collections.Generic;

namespace EskerAP.Service.Interface
{
	public interface IUnpaidInvoiceReader
	{
		public IEnumerable<UnpaidInvoice> GetUnpaidInvoices(string remoteDirectory, string localDirectory);
	}
}
