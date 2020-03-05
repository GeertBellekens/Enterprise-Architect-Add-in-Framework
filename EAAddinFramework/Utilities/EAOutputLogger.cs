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
        List<string> errors = new List<string>();
        List<string> warnings = new List<string>();

        static Dictionary<string, EAOutputLogger> outputLogs = new Dictionary<string, EAOutputLogger>();
		static EAOutputLogger defaultLogger;
		static EAOutputLogger getOutputLogger(Model model, string outputName)
		{
			var logKey = model.projectGUID+outputName;
			if (!outputLogs.ContainsKey(logKey))
			{
				outputLogs.Add(logKey,new EAOutputLogger(model, outputName));
			}
			defaultLogger = outputLogs[logKey]; 
			return defaultLogger;
		}
        public static IEnumerable<string> getErrors (Model model, string outputName)
        {
            var outputLogger = getOutputLogger(model, outputName);
            return outputLogger.errors;
        }
        public static IEnumerable<string> getWarnings(Model model, string outputName)
        {
            var outputLogger = getOutputLogger(model, outputName);
            return outputLogger.warnings;
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
        private void logError(string message, int elementID = 0)
        {
            this.logToOutput(message, elementID);
            this.errors.Add(message);
        }
        private void logWarning(string message, int elementID = 0)
        {
            this.logToOutput(message, elementID);
            this.warnings.Add(message);
        }
        private void logToOutput(string message, int elementID = 0)
		{
			this.model.wrappedModel.WriteOutput(this.name,message,elementID);
		}
		private void clear()
		{
			this.model.wrappedModel.ClearOutput(this.name);
            this.errors.Clear();
            this.warnings.Clear();
		}
		/// <summary>
		/// log a message to the EA output window. If requested the message will also be logged to the logfile
		/// </summary>
		/// <param name="model">the model on which to show the output</param>
		/// <param name="outputName">the name of the output window</param>
		/// <param name="message">the message to show</param>
		/// <param name="elementID">the element ID to associate with the message. Can be used by add-ins when they implement EA_OnOutput...</param>
		/// <param name="logType">the type of logging to the logfile</param>
		public static void log(Model model,string outputName, string message, int elementID = 0,LogTypeEnum logType = LogTypeEnum.none)
		{
			var logger = getOutputLogger(model, outputName);

			//log to logfile if needed
			switch (logType) 
			{
                case LogTypeEnum.none:
                    //log to output
                    logger.logToOutput($"{DateTime.Now.ToString("hh:mm:ss.fff")} {message}", elementID);
                    break;
                case LogTypeEnum.log:
					Logger.log(message);
                    //log to output
                    logger.logToOutput($"{DateTime.Now.ToString("hh:mm:ss.fff")} {message}", elementID);
                    break;
				case LogTypeEnum.warning:
					message = "Warning: " + message;
					Logger.logWarning(message);
                    //log to output
                    logger.logWarning($"{DateTime.Now.ToString("hh:mm:ss.fff")} {message}", elementID);
                    break;
				case LogTypeEnum.error:
					message = "Error: " + message;
					Logger.logError(message);
                    //log to output
                    logger.logError($"{DateTime.Now.ToString("hh:mm:ss.fff")} {message}", elementID);
                    break;
			}
		}
		/// <summary>
		/// logs a message to the currently active outputlogger
		/// </summary>
		/// <param name="message">the message to be logged</param>
		/// <param name="elementID">the element ID to associate with the message. Can be used by add-ins when they implement EA_OnOutput...</param>
		/// <param name="logType">the type of logging to the logfile</param>
		public static void log(string message, int elementID = 0,LogTypeEnum logType = LogTypeEnum.none)
		{
			if (defaultLogger != null)
			{
				log(defaultLogger.model, defaultLogger.name, message, elementID, logType);
			}
		}
		public static void clearLog(Model model,string outputName)
		{
			var logger = getOutputLogger(model, outputName);
			logger.clear();
		}                       

	}
}
