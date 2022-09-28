using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public interface IImportApVouchersRepo
	{
		public Domain.ImportApVoucherResponse ImportVoucher(Domain.Voucher voucher);
	}
}
