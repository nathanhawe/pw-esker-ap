namespace EskerAP.Service.Interface
{
	public interface ICostCenterExporter
	{
		public void ExportCostCenters(string companyCode, string folderPath);
	}
}
