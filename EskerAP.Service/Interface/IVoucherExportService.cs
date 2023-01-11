namespace EskerAP.Service.Interface
{
	public interface IVoucherExportService
	{
		void ExportPaidInvoices(string localDirectory, string remoteDirectory, string companyCode, int daysPast);		
	}
}
