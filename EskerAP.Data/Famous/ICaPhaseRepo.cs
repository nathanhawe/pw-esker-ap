using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public interface ICaPhaseRepo
	{
		public IEnumerable<EskerAP.Domain.Phase> GetPhases();
	}
}
