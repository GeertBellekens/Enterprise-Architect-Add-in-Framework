/*
 * Created by SharpDevelop.
 * User: wij
 * Date: 15/03/2015
 * Time: 5:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace EAAddinFramework.EASpecific
{
	/// <summary>
	/// Description of EAAddin.
	/// </summary>
	public class EAAddin
	{
		private Assembly addinDLL;
		private object wrappedAddin;
		public string name {get;set;}
		public List<string> lastMenuOptions {get;set;}
		
		public EAAddin(string fileName,string name):base()
		{
			this.lastMenuOptions = new List<string>();
			this.name = name;
			//load the dll
			this.addinDLL = Assembly.LoadFrom(fileName);
			//register the COM visible classes
			this.registerClasses(this.addinDLL);
			//load referenced dll's
			foreach (AssemblyName reference in this.addinDLL.GetReferencedAssemblies())
			{
			      if (System.IO.File.Exists(
			             System.IO.Path.GetDirectoryName(this.addinDLL.Location) + 
			                @"\" + reference.Name + ".dll"))
			      {
			       	Assembly referencedAssembly = System.Reflection.Assembly.LoadFrom(System.IO.Path.GetDirectoryName(this.addinDLL.Location) + @"\" + reference.Name + ".dll");
			       	//register the COM visible classes for the registered assembly
			       	this.registerClasses(referencedAssembly);
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
		
		private void registerClasses (Assembly assembly)
		{
			// can't use RegisterAssmebly as it requires elevated privileges
			//new RegistrationServices().RegisterAssembly(assembly, AssemblyRegistrationFlags.SetCodeBase);
			
			foreach (Type type in assembly.GetExportedTypes()) 
			{
					bool isAlreadyRegistered = register(type);
					if (isAlreadyRegistered)
					{
						//don't bother, this assembly has already been registered in HKCR
						break;
					}
					//debug
					//cleanRegistry(type);
			}
		}
		
		private bool register(Type type)
		{
			bool isAlreadyRegistered = isAlreadyRegisteredInHKCR(type);
				
			if (!isAlreadyRegistered && isComVisible(type))
			{
				
				// software classes
				RegistryKey controlKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + type.FullName);
				controlKey.SetValue(string.Empty,type.FullName);
				RegistryKey clsidKey = controlKey.CreateSubKey("CLSID");
				clsidKey.SetValue(string.Empty,type.GUID.ToString("B"));
				
				//CLSID
				RegistryKey classKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\CLSID\" + type.GUID.ToString("B"));
				classKey.SetValue(string.Empty,type.FullName);
				
				//implemented category
				Registry.CurrentUser.CreateSubKey(@"Software\Classes\CLSID\" + type.GUID.ToString("B") + @"\Implemented Categories");
				Registry.CurrentUser.CreateSubKey(@"Software\Classes\CLSID\" + type.GUID.ToString("B") + @"\Implemented Categories\{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}");
				
				//inprocerver
				RegistryKey inprocKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\CLSID\" + type.GUID.ToString("B") + @"\InprocServer32");
				inprocKey.SetValue(string.Empty,"mscoree.dll"); //hardcoded
				inprocKey.SetValue("ThreadingModel","Both"); //hardcoded?
				inprocKey.SetValue("Class",type.FullName);
				inprocKey.SetValue("Assembly",type.Assembly.FullName);
				inprocKey.SetValue("RuntimeVersion",type.Assembly.ImageRuntimeVersion);
				inprocKey.SetValue("CodeBase",type.Assembly.EscapedCodeBase);
				
				//version
				RegistryKey versionKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\CLSID\" + type.GUID.ToString("B") + @"\InprocServer32\" + type.Assembly.GetName().Version);
				versionKey.SetValue("Class",type.FullName);
				versionKey.SetValue("Assembly",type.Assembly.FullName);
				versionKey.SetValue("RuntimeVersion",type.Assembly.ImageRuntimeVersion);
				versionKey.SetValue("CodeBase",type.Assembly.EscapedCodeBase);
	
				//ProgID
				RegistryKey progIdkey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\CLSID\" + type.GUID.ToString("B") + @"\ProgId");
				progIdkey.SetValue(string.Empty,type.FullName);
			}
			return isAlreadyRegistered;
		}
		
		private bool isAlreadyRegisteredInHKCR(Type type)
		{
			return (Registry.ClassesRoot.OpenSubKey(type.FullName) != null);
		}
		
		private void cleanRegistry(Type type)
		{
			if (isComVisible(type))
			{
				// software classes			
				Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\" + type.FullName,false);
			
				//CLSID
				Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\CLSID\" + type.GUID.ToString("B"),false);
			} 	
		}
		/// <summary>
		/// Check if the given type is ComVisible
		/// </summary>
		/// <param name="type">the type to check</param>
		/// <returns>whether or not the given type is ComVisible</returns>
		private bool isComVisible(Type type)
		{
			bool comVisible = true;
			//first check if the type has ComVisible defined for itself
			var typeAttributes = type.GetCustomAttributes(typeof(ComVisibleAttribute),false);
			if 	(typeAttributes.Length > 0)
			{
				 comVisible = ((ComVisibleAttribute)typeAttributes[0]).Value;
			}
			else
			{
				//no specific ComVisible attribute defined, return the default for the assembly
				var assemblyAttributes = type.Assembly.GetCustomAttributes(typeof(ComVisibleAttribute),false);
				if 	(assemblyAttributes.Length > 0)
				{
					 comVisible = ((ComVisibleAttribute)assemblyAttributes[0]).Value;
				}
			}
			return comVisible;
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
					EAAddinFramework.Utilities.Logger.logError ("Error occured executing " + methodName + " : " + e.Message + " stacktrace: " + e.StackTrace);
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
