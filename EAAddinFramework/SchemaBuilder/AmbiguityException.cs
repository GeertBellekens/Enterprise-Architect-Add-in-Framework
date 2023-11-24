using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAAddinFramework.SchemaBuilder
{
    public class AmbiguityException : Exception
    {
        internal AmbiguityException(string message) : base(message) { }
    }
}
