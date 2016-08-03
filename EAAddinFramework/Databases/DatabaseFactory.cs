
using System;
using System.Collections.Generic;
using TSF.UmlToolingFramework.Wrappers.EA;
using DB=DatabaseFramework;

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
	}
}
