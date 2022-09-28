using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public interface IPurchaseOrderHeaderRepo
	{
		public IEnumerable<EskerAP.Domain.Header> GetPurchaseOrderHeaders();
	}
}
