using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAP.Service.Interface
{
	public interface IMasterDataExportService
	{
		void ExportMasterData(string localDirectory, string RemoteDirectory, string companyCode, bool purchaseOrdersOnly);		
	}
}
