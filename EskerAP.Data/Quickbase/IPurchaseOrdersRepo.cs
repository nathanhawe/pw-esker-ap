using System.Collections.Generic;

namespace EskerAP.Data.Quickbase
{
	public interface IPurchaseOrdersRepo
	{
		public IEnumerable<EskerAP.Domain.Header> Get();
	}
}
