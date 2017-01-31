using System.Collections.Generic;
using System.Linq;
using System;
using FileHelpers;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of CSVMappingRecord.
	/// </summary>
	[DelimitedRecord(";")]
	public class CSVMappingRecord
	{
		public string sourcePath;
		public string targetPath;
		public string mappingLogic;
	}
}
