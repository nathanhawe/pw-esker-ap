using EskerAP.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAP.Service.Interface
{
	public interface IErpAckService
	{
		string GetErpAckXmlString(ImportApVoucherResponse importApVoucherResponse, string ruid);
	}
}
