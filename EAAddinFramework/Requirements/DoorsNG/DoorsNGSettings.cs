using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAAddinFramework.Requirements.DoorsNG
{
    public interface DoorsNGSettings
    {

        Dictionary<string, string> projectConnections { get; set; }
        Dictionary<string, string> requirementMappings { get; set; }
        string defaultProject { get; set; }
        string defaultUserName { get; set; }
        string defaultPassword { get; set; }
        List<string> mappedElementTypes { get; }
        List<string> mappedStereotypes { get; }
        string defaultStatus { get; set; }
    }
}
