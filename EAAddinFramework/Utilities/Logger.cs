using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EAAddinFramework.Utilities
{
    public static class Logger
    {
        private static string _logFileName = System.IO.Path.GetTempPath() + @"\EAAddinFramework.log";
        private static DateTime lastTimeStamp = System.DateTime.Now;
        /// <summary>
        /// the logfile full pathname
        /// </summary>
        public static string logFileName
        {
        	get
        	{
        		return _logFileName;	
        	}
        	set 
        	{
        		_logFileName = value;
        	}
        }
        /// <summary>
        /// set the log file name
        /// </summary>
        /// <param name="fileName">the name of the log file</param>
        public static void setLogFileName(string fileName)
        {
            _logFileName = fileName;
        }
        /// <summary>
        /// log a message
        /// </summary>
        /// <param name="logmessage">the message to be logged</param>
        public static void log(string logmessage)
        { 
        	try {
	        	System.IO.StreamWriter logfile = new System.IO.StreamWriter(_logFileName,true);
	            double diff = (System.DateTime.Now - lastTimeStamp).TotalMilliseconds;
	            lastTimeStamp = System.DateTime.Now;
	            logfile.WriteLine( System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.fff") + " diff: "+ diff.ToString() + " ms " + logmessage);
	            logfile.Close();
        		
        	} catch (Exception) 
        	{
        		
        		// do nothing. If the logging fails we don't want to log anything to avoid eternal loops
        	}

        }
        /// <summary>
        /// log an error
        /// </summary>
        /// <param name="logmessage">the error message</param>
        public static void logError(string logmessage)
        {
            log("Error: " + logmessage);
        }
        /// <summary>
        /// log a warning
        /// </summary>
        /// <param name="logmessage">the warning message</param>
        public static void logWarning(string logmessage)
        {
            log("Warning: " + logmessage);
        }
    }
}

