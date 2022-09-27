using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public interface IPurchaseOrderDetailRepo
	{
		public IEnumerable<EskerAP.Domain.Item> GetPurchaseOrderDetails();
	}
}
