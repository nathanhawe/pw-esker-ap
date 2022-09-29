using System.IO;

namespace EskerAP.Service
{
	public abstract class Exporter
	{
		public void EnsureFolderExists(string folderPath)
		{
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
		}
	}
}
