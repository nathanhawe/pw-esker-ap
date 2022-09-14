using System.Collections.Generic;

namespace EskerAP.Data.Quickbase
{
	public interface IItemsRepo
	{
		public IEnumerable<EskerAP.Domain.Item> Get();
	}
}
