namespace EskerAP.Service.Interface
{
	public interface IVoucherImportService
	{
		void ImportVouchers(string remoteDirectory, string localDirectory, string erpAckDirectory);
	}
}
