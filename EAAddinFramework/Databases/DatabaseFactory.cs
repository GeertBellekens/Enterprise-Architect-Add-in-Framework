
using System;
using System.Collections.Generic;
using TSF.UmlToolingFramework.Wrappers.EA;
using DB=DatabaseFramework;
using System.Linq;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of DatabaseFactory.
	/// </summary>
	public class DatabaseFactory:DB.DataBaseFactory
	{
		private string _type;
		public string type 
		{
			get { return _type;}
		}
		private Dictionary<string, BaseDataType> _baseDataTypes;
		public List<DB.BaseDataType> baseDataTypes
		{
			get
			{
				List<DB.BaseDataType> datypes = new List<DB.BaseDataType>();
				foreach (var baseDatatype in _baseDataTypes.Values) 
				{
					datypes.Add(baseDatatype);
				}
				return datypes;
			}
		}
		/// <summary>
		/// adds a factory to the list of possible database factories
		/// </summary>
		/// <param name="type">the type of the factory</param>
		/// <param name="datatypes">the base datatypes for this factory</param>
		public static void addFactory(string type, List<BaseDataType> datatypes)
		{
			if (!factories.ContainsKey(type))
			{
				DatabaseFactory factory = new DatabaseFactory(type, datatypes);
				factories.Add(type, factory);
			}
		}
		private static Dictionary<string, DatabaseFactory> factories = new Dictionary<string, DatabaseFactory>();
		public static DatabaseFactory getFactory(string type)
		{
			if (factories.ContainsKey(type))
			{
				return factories[type];
			}
			return null;
		}
		private DatabaseFactory(string type, List<BaseDataType> baseDataTypes)
		{
			this._type = type;
			_baseDataTypes = new Dictionary<string, BaseDataType>();
			foreach (var baseDataType in baseDataTypes) 
			{
				_baseDataTypes.Add(baseDataType.name, baseDataType);
			}
		}
		public Database createDataBase(Package package)
		{
			return new Database(package, this);
		}
		public Database createDatabase(string name)
		{
			return new Database(name, this);
		}
		public DataType createDataType(string compositeName)
		{
			string baseTypeName;
			int length = 0;
			int precision = 0;
			if (compositeName.Contains("("))
			{
				baseTypeName = compositeName.Substring(0,compositeName.IndexOf("("));
				string scaleString = compositeName.Substring(compositeName.IndexOf("(") +1);
				if (scaleString.Contains(","))
				{
					int.TryParse(scaleString.Substring(0,scaleString.IndexOf(",")),out length );
					string precisionString = scaleString.Substring(scaleString.IndexOf(",") +1 ,scaleString.Length-scaleString.IndexOf(",") -2);
					int.TryParse(precisionString,out precision);
				}
				else
				{
					int.TryParse(scaleString.Substring(0, scaleString.Length -1),out length);
				}
			}
			else
			{
				baseTypeName = compositeName;
			}
			BaseDataType basetype = this.baseDataTypes.FirstOrDefault(x => x.name == baseTypeName) as BaseDataType;
			if (basetype != null)
			{
				return new DataType(basetype,length,precision);
			}
			else
			{
				return null;
			}
			 
		}
	}
}
