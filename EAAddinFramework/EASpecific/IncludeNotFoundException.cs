using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer;
using System.Text;
using System.Threading.Tasks;

namespace EAAddinFramework.EASpecific
{
    [Serializable]
    public class IncludeNotFoundException : Exception
    {
        /// <summary>
        /// Creates an IncludeNotFoundException for the includeStatement that can not be found
        /// 
        /// The message will be of the form:
        /// Could not locate include file for '{includeStatement}'.
        /// The location of the file should be added by wrapping this in IncludeNotFoundException(containedInFile, inner);
        /// </summary>
        /// <param name="includeStatement">the includeStatement that can not be found</param>
        public IncludeNotFoundException(string includeStatement) : base($"Could not locate include file for '{includeStatement}'") { }

        /// <summary>
        /// Create an IncludeNotFoundException for the specified file and caused by exception.
        /// 
        /// The message will be of the form:
        /// {causedBy.Message} /{containedInFile}
        /// </summary>
        /// <param name="containedInFile"></param>
        /// <param name="causedBy"></param>
        public IncludeNotFoundException(string containedInFile, IncludeNotFoundException causedBy) : base($"{causedBy.Message} /{containedInFile}") { }
    }
}
