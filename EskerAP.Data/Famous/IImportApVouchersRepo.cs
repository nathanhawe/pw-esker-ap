using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public interface IImportApVouchersRepo
	{
		public void ImportVoucher(Domain.Voucher voucher);
	}
}
