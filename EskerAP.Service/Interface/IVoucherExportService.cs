namespace EskerAP.Service.Interface
{
	public interface IVoucherExportService
	{
		void ExportPaidInvoices(string paidInvoiceLocalDirectory, string paidInvoiceRemoteDirectory, string unpaidInvoiceRemoteDirectory, string companyCode);		
	}
}
