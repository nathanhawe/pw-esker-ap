using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public interface IApVendorRepo
	{
		public IEnumerable<EskerAP.Domain.Vendor> GetVendors();
	}
}
