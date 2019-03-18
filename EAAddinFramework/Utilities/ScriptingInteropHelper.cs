using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace EAAddinFramework.Utilities
{
    /// <summary>
    /// Class contains a number of helper functions to enable scripts to work with .net objects and generics etc..
    /// </summary>
    public class ScriptingInteropHelper
    {
        public ArrayList toArrayList(IEnumerable collection)
        {
            var arrayList = new ArrayList();
            foreach (object element in collection)
            {
                arrayList.Add(element);
            }
            return arrayList;
        }
        public object toObject(object someObject)
        {
            return someObject as object;
        }
        public string getObjectTypeName(object someObject)
        {
            return someObject.GetType().Name;
        }
        /// <summary>
        /// get an enumerated property by name
        /// </summary>
        /// <param name="owner">the object owning the property</param>
        /// <param name="propertyName">the name of the property</param>
        /// <returns>an ArrayList containing the objects of the enumerated property</returns>
        public ArrayList getEnumeratedProperty(object owner, string propertyName)
        {
            var enumerated = owner.GetType().GetProperty(propertyName).GetValue(owner, null) as IEnumerable;
            return enumerated != null ? this.toArrayList(enumerated) : new ArrayList();
        }
        /// <summary>
        /// executes the given operation and returns the result as an object
        /// </summary>
        /// <param name="owner">the instance we need to callt he operation on</param>
        /// <param name="operationName">the name of the operation</param>
        /// <param name="parameters">the parameters</param>
        /// <returns></returns>
        public object executeMethod(object owner, string operationName, object[] parameters)
        {
            return owner.GetType().GetMethod(operationName).Invoke(owner, parameters) as object;
        }
        /// <summary>
        /// Execute a static method on the 
        /// </summary>
        /// <param name="qualifiedClassName"></param>
        /// <param name="operationName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object executeStaticMethod(string qualifiedClassName, string operationName, object parameters)
        {
            try
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in assembly.GetTypes())
                    {

                        if (type.Namespace + "." + type.Name == qualifiedClassName
                            && type.IsClass)
                        {
                            var methodInfo = type.GetMethod(operationName, (BindingFlags.Static | BindingFlags.Public));
                            if (methodInfo != null)
                            {
                                return methodInfo.Invoke(null, (object[])parameters);
                            }
                        }
                    }
                }
                throw new ArgumentException($"Static method {qualifiedClassName}.{operationName}() does not exist or is not loaded");
            }
            catch (Exception e)
            {
                Logger.logError(e.Message + Environment.NewLine + e.StackTrace);
                if (e.InnerException != null)
                {
                    Logger.logError(e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace);
                }
                throw e;
            }
        }
    }
}
