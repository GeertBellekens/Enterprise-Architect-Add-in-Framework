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
        public string name
        {
            get
            {
                return this.procedure.Name;
            }
        }
        public bool isAddinFunction => (this.name.StartsWith("EA_") || this.name.StartsWith("MDG_")) && !this.name.ToLower().Contains("addin");
        public string fullName
        {
            get
            {
                return this.owner.name + "." + this.procedure.Name;
            }
        }
        public int numberOfParameters
        {
            get
            {
                return this.procedure.NumArgs;
            }
        }
        private Procedure procedure { get; set; }

        public ScriptFunction(Script owner, Procedure procedure)
        {
            this.owner = owner;
            this.procedure = procedure;
        }
        /// <summary>
        /// execute this function
        /// </summary>
        /// <param name="parameters">the parameters needed by this function</param>
        /// <returns>whatever gets returned by the the actual script function</returns>
        public object execute(object[] parameters)
        {
            if (parameters != null
                && this.procedure.NumArgs == parameters.Length)
            {
                return this.owner.executeFunction(this.name, parameters);
            }
            else if (this.procedure.NumArgs == 0)
            {
                return this.owner.executeFunction(this.name);
            }
            else if (parameters != null)
            {
                throw new ArgumentException("wrong number of arguments. Script has " + this.procedure.NumArgs + " argument where the call has " + parameters.Length + " parameters");
            }
            else
            {
                throw new ArgumentException("wrong number of arguments. Script has " + this.procedure.NumArgs + " argument where the call has 0 parameters");
            }
        }
    }
}
