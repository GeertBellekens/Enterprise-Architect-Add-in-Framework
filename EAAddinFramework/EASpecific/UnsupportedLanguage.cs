using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EAAddinFramework.EASpecific
{
    public class UnsupportedLanguage : ScriptLanguage
    {
        private string _name;

        public UnsupportedLanguage(string language) : base() {
            this._name = language;
        }

        public override string name => _name;

        public override string commentLine => throw new NotImplementedException();

        public override string extension => throw new NotImplementedException();

        protected override string functionStart => throw new NotImplementedException();

        protected override string parameterListStart => throw new NotImplementedException();

        protected override string parameterSeparator => throw new NotImplementedException();

        protected override string parameterListEnd => throw new NotImplementedException();

        protected override string bodyStart => throw new NotImplementedException();

        protected override string bodyEnd => throw new NotImplementedException();

        protected override string functionEnd => throw new NotImplementedException();
    }
}
