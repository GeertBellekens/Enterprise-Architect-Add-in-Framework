using System.Collections.Generic;
using System.Linq;
using System;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of CSVMappingRecord.
	/// </summary>
	public class CSVMappingRecord
	{
		public string sourcePath;
		public string targetPath;
		public string mappingLogic;
	}
    public sealed class CSVMappingRecordMap : CsvHelper.Configuration.ClassMap<CSVMappingRecord>
    {
        public CSVMappingRecordMap()
        {
            Map(m => m.sourcePath).Index(0);
            Map(m => m.sourcePath).Name("Source Path");
            Map(m => m.targetPath).Index(1);
            Map(m => m.targetPath).Name("Target Path");
            Map(m => m.mappingLogic).Index(2);
            Map(m => m.mappingLogic).Name("Mapping Logic");
        }

    }
}
