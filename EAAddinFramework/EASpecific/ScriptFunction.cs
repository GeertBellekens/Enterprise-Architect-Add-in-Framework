/*
 * Created by SharpDevelop.
 * User: Geert
 * Date: 7/10/2014
 * Time: 19:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using MSScriptControl;

namespace EAAddinFramework.EASpecific
{
    /// <summary>
    /// Description of ScriptFunction.
    /// </summary>
    public class ScriptFunction
    {
        public Script owner { get; private set; }
        public string name { get; set; }

        public bool isAddinFunction => (this.name.StartsWith("EA_") || this.name.StartsWith("MDG_")) && !this.name.ToLower().Contains("addin");
        public string fullName => this.owner.name + "." + this.name;

        public int? numberOfParameters => this.procedure?.NumArgs;

        private Procedure _procedure;
        private Procedure procedure 
        { 
            get => this._procedure;
            set
            {
                this._procedure = value;
                this.name = value.Name;
            }
        }

        public ScriptFunction(Script owner, Procedure procedure)
        {
            this.owner = owner;
            this.procedure = procedure;
        }
        public ScriptFunction (Script owner, String name)
        {
            this.owner = owner;
            this.name = name;
        }

        public override string ToString()
        {
            return this.fullName;
        }

        /// <summary>
        /// execute this function
        /// </summary>
        /// <param name="parameters">the parameters needed by this function</param>
        /// <returns>whatever gets returned by the the actual script function</returns>
        public object execute(object[] parameters)
        {
            if (parameters != null
                && (this.numberOfParameters == parameters.Length || this.numberOfParameters == null))
            {
                return this.owner.executeFunction(this.name, parameters);
            }
            else if (this.numberOfParameters == 0 || this.numberOfParameters == null )
            {
                return this.owner.executeFunction(this.name);
            }
            else if (parameters != null)
            {
                throw new ArgumentException("wrong number of arguments. Script has " + this.numberOfParameters + " argument where the call has " + parameters.Length + " parameters");
            }
            else
            {
                throw new ArgumentException("wrong number of arguments. Script has " + this.numberOfParameters + " argument where the call has 0 parameters");
            }
        }
    }
}
