using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public interface IGLAccountRepo
	{
		public IEnumerable<EskerAP.Domain.GLAccount> GetGLAccounts();
	}
}
