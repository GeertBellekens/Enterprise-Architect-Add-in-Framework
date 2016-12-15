using System.Collections.Generic;
using System.Linq;
using System;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Utilities
{
	/// <summary>
	/// Description of EAOutputLogger.
	/// </summary>
	public class EAOutputLogger
	{
		Model model;
		string name;
		static Dictionary<string, EAOutputLogger> outputLogs = new Dictionary<string, EAOutputLogger>();
		static EAOutputLogger getOutputLogger(Model model, string outputName)
		{
			var logKey = model.projectGUID+outputName;
			if (!outputLogs.ContainsKey(logKey))
			{
				outputLogs.Add(logKey,new EAOutputLogger(model, outputName));
			}
			return outputLogs[logKey];
		}
		/// <summary>
		/// private constructor
		/// </summary>
		/// <param name="model">the model this output applies to</param>
		/// <param name="outputName"></param>
		private EAOutputLogger(Model model, string outputName)
		{
			this.model = model;
			this.name = outputName;
			//make sure the log exists and is visible and cleared
			this.model.wrappedModel.CreateOutputTab(this.name);
			this.model.wrappedModel.EnsureOutputVisible(this.name);
			this.model.wrappedModel.ClearOutput(this.name);
		}
		private void logToOutput(string message, int elementID = 0)
		{
			this.model.wrappedModel.EnsureOutputVisible(this.name);
			this.model.wrappedModel.WriteOutput(this.name,message,elementID);
		}
		private void clear()
		{
			this.model.wrappedModel.ClearOutput(this.name);
		}
		
		public static void log(Model model,string outputName, string message, int elementID = 0)
		{
			var logger = getOutputLogger(model, outputName);
			logger.logToOutput(message,elementID);
		}
		public static void clearLog(Model model,string outputName)
		{
			var logger = getOutputLogger(model, outputName);
			logger.clear();
		}                       

	}
}
