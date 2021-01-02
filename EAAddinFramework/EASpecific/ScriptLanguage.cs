/*
 * Created by SharpDevelop.
 * User: wij
 * Date: 22/11/2014
 * Time: 6:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;

namespace EAAddinFramework.EASpecific
{
	/// <summary>
	/// Description of ScriptLanguage.
	/// </summary>
	public abstract class ScriptLanguage
	{

		public ScriptFunction addFunction(Script script, MethodInfo operation)
		{
			//get the script code
			//get the function code
			string functionCode = this.translate(operation);
			//add function code to script code
			script.addCode(functionCode);
			//return the function
			return script.functions.Find(x => x.name == operation.Name);
		}
		public abstract string name{get;}
		protected abstract string functionStart {get;}
		protected abstract string parameterListStart {get;}
		protected abstract string parameterSeparator {get;}
		protected abstract string parameterListEnd{get;}
		protected abstract string bodyStart {get;}
		protected abstract string bodyEnd {get;}
		protected abstract string functionEnd {get;}
		public abstract string commentLine {get;}
		public abstract string extension { get; }

		public string translate(MethodInfo operation)
		{
			//start with e new line
			string code = Environment.NewLine;
			//keyword				
			code += functionStart;
			//name of the method
			code += operation.Name;
			//open parenthesis
			code += parameterListStart;
			//parameters
			bool firstParameter = true;
			foreach (ParameterInfo parameter in operation.GetParameters()) 
			{
				//don't add the repository parameter as it is added directly to the scriptcontroller
				if (parameter.Name.ToLower() != "repository")
				{
					if (firstParameter)
					{
						firstParameter = false;
					}
					else
					{
						//add a comma and space starting from the second parameter
						code += parameterSeparator;
					}
					//parameter name
					code += parameter.Name;
					
				}
			}
			//close parenthesis
			code += parameterListEnd;
			//add newline
			code += Environment.NewLine;
			//begin of body
			code += bodyStart;
			//add newline if there was a bodyStart
			if (!string.IsNullOrEmpty(bodyStart))
			{
				code += Environment.NewLine;							
			}
			//add tab + comment
			code += "\t "+commentLine+"Add code here";
			//add newline
			code += Environment.NewLine;
			//add end of body 
			code += bodyEnd;
			//add newline if there was a body end
			if (!string.IsNullOrEmpty(bodyEnd))
			{
				code += Environment.NewLine;
			}
			//add end keyword
			code += functionEnd;
			return code;							
		}

	}
}
