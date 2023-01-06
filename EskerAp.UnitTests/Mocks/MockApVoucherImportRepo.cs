using EskerAP.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAp.UnitTests.Mocks
{
    internal class MockApVoucherImportRepo : EskerAP.Data.Famous.IImportApVouchersRepo
    {
        public Voucher Voucher { get; set; }
        public ImportApVoucherResponse Response {get;set;}
		public ImportApVoucherResponse ImportVoucher(Voucher voucher)
		{
			Voucher = voucher;
			return Response;
		}
	}
}
