namespace EskerAP.Service.Interface
{
	public interface IPaidInvoiceExporter
	{
		public void ExportPaidInvoices(string companyCode, string folderPath, int daysPast);
	}
}
