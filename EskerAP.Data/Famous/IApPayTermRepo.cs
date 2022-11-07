using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public interface IApPayTermsRepo
	{
		public IEnumerable<EskerAP.Domain.PaymentTerm> GetPayTerms();
	}
}
