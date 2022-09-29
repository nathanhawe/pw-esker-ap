using EskerAP.Domain.Constants;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace EskerAP.Service
{
	public abstract class Exporter
	{
		protected void EnsureFolderExists(string folderPath)
		{
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
		}

		protected string GetFilePath(Erp erp, ExportType exportType, string folderPath)
		{
			return $"{folderPath}\\{GetErpFileNamePrefix(erp)}__{exportType}__{DateTime.Now:yyyyMMddHHmmss}.csv";
		}

		private string GetErpFileNamePrefix(Erp erp)
		{
			switch (erp)
			{
				case Erp.Famous: return "FA";
				case Erp.Quickbase: return "QB";
				default: return "";
			}
		}
	}
}
