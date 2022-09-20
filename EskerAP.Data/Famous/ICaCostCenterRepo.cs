using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public interface ICaCostCenterRepo
	{
		public IEnumerable<EskerAP.Domain.CostCenter> GetCostCenters();
	}
}
