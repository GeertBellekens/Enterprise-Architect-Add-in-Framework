/*
 * Created by SharpDevelop.
 * User: wij
 * Date: 15/03/2015
 * Time: 5:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;
using System.Linq;

namespace EAAddinFramework.EASpecific
{
	/// <summary>
	/// Description of EAAddin.
	/// </summary>
	public class EAAddin
	{
		private Assembly addinDLL;
		private object wrappedAddin;
		
		
		public EAAddin(string fileName):base()
		{
			this.addinDLL = Assembly.LoadFrom(fileName);
			foreach (AssemblyName reference in this.addinDLL.GetReferencedAssemblies())
			{
			      if (System.IO.File.Exists(
			             System.IO.Path.GetDirectoryName(this.addinDLL.Location) + 
			                @"\" + reference.Name + ".dll"))
			      {
			         System.Reflection.Assembly.LoadFrom(
			            System.IO.Path.GetDirectoryName(this.addinDLL.Location) + 
			               @"\" + reference.Name + ".dll");
			      }
			      else
			      {
			         System.Reflection.Assembly.ReflectionOnlyLoad(reference.FullName);
			      }
			
			      //selectedAssembly.GetExportedTypes();       
			} 
			// find the addin class
			foreach ( Type candidateClass in addinDLL.GetExportedTypes())
			{
				//TODO: find a better way to determine which of these types is the add-in class
				bool foundit = false;
				foreach ( MethodInfo method in candidateClass.GetMethods())
				{
					if (method.Name.StartsWith("EA_"))
				    {
				    	foundit = true;
				    	break;
				    }
				}
				if (foundit)
				{
					this.wrappedAddin = Activator.CreateInstance(candidateClass); 
					break;
				}
			}
		}
		
		public object callmethod(string methodName, object[] parameters)
		{
			var param = parameters;
			MethodInfo method = wrappedAddin.GetType().GetMethod(methodName);
			if (method != null)
			{
				try
				{
					return method.Invoke(this.wrappedAddin,param);
				}
				catch (Exception e)
				{
					EAAddinFramework.Utilities.Logger.logError ("Error occured executing " + methodName + " : " + e.Message + "stacktrace: " + e.StackTrace);
					return null;						
				}
			}
			else
			{
				return null;
			}
		}
		
		

	}
}
