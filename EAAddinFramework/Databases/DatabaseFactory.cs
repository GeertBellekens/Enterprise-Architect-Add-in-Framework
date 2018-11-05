
using System;
using System.Collections.Generic;
using EAAddinFramework.Databases.Strategy;
using TSF.UmlToolingFramework.Wrappers.EA;
using DB = DatabaseFramework;
using System.Linq;
using UML = TSF.UmlToolingFramework.UML;
using DB_EA = EAAddinFramework.Databases;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Databases
{
    /// <summary>
    /// Description of DatabaseFactory.
    /// </summary>
    public class DatabaseFactory : DB.DataBaseFactory
    {
        private string _type;
        private Model _model;
        private StrategyFactory _strategyFactory;
        public string type
        {
            get { return _type; }
        }

        public string databaseName
        {
            get
            {
                return type;
            }
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
        /// adds a factory to the list of possible database factories or return the existing factory for the given type and model
        /// </summary>
        /// <param name="type">the type of the factory</param>
        /// <param name = "model">the model for this factory</param>
        /// <param name = "strategyFactory">the strategy for this factory</param>
        public static DatabaseFactory getFactory(string type, Model model, StrategyFactory strategyFactory)
        {

            if (!modelFactories.ContainsKey(model))
            {
                var newFactories = new Dictionary<string, DatabaseFactory>();
                modelFactories.Add(model, newFactories);
            }
            var factories = modelFactories[model];
            if (!factories.ContainsKey(type))
            {
                DatabaseFactory factory = new DatabaseFactory(type, model, strategyFactory);
                factories.Add(type, factory);
            }
            return factories[type];
        }
        public static DB_EA.Column createColumn(TSF_EA.Attribute attribute)
        {
            //create the table
            var tableElement = attribute.owner as Class;
            if (tableElement == null) return null;
            var table = createTable(tableElement);
            if (table == null) return null;
            //create the column
            return new Column(table, attribute);
        }
        public static DB_EA.Table createTable(TSF_EA.Class tableElement)
        {
            //get the database
            var database = getDatabase(tableElement);
            if (database == null) return null;
            //create the table
            return new Table(database, tableElement);
        }

        public static DB_EA.Database getDatabase(TSF_EA.Element element)
        {
            //first get the database based on the attribute
            var databasePackage = element.getAllOwners().OfType<Package>().FirstOrDefault(x => x.stereotypeNames.Any(y => y.ToLower() == "database"));
            //no database found, return empty
            if (databasePackage == null) return null;
            string databaseType = databasePackage.taggedValues.FirstOrDefault(x => x.name.ToLower() == "dbms")?.tagValue.ToString();
            //no database type found. We cannot create the database
            if (string.IsNullOrEmpty(databaseType)) return null;
            //create the factory and database
            // TODO: add caching?
            var dbFactory = getFactory(databaseType, element.EAModel, new StrategyFactory());
            return dbFactory?.createDataBase(databasePackage);
        }

        //private static Dictionary<string, DatabaseFactory> factories = new Dictionary<string, DatabaseFactory>();
        private static Dictionary<Model, Dictionary<string, DatabaseFactory>> modelFactories = new Dictionary<Model, Dictionary<string, DatabaseFactory>>();

        public UML.Extended.UMLFactory modelFactory
        {
            get
            {
                if (this._model != null) return (Factory)this._model.factory;
                else return null;
            }
        }
        public Factory _modelFactory
        {
            get
            {
                return modelFactory as Factory;
            }
        }
        private DatabaseFactory(string type, Model model, StrategyFactory strategyFactory)
        {
            this._type = type;
            this._model = model;
            this._baseDataTypes = getBaseDataTypes(type, model);
            this._strategyFactory = strategyFactory;
        }
        private static Dictionary<Model, List<BaseDataType>> modelBaseDataTypes = new Dictionary<Model, List<BaseDataType>>();
        public static void loadBaseDataTypes(Model model)
        {
            var newBaseDataTypes = new List<BaseDataType>();
            foreach (global::EA.Datatype eaDataType in model.wrappedModel.Datatypes)
            {
                if (eaDataType.Type == "DDL")
                {
                    newBaseDataTypes.Add(new BaseDataType(eaDataType));
                }
            }
            modelBaseDataTypes[model] = newBaseDataTypes;
        }
        public static Dictionary<string, BaseDataType> getBaseDataTypes(string databaseType, Model model)
        {
            //make sure the BaseDataTypes are loaded
            if (!modelBaseDataTypes.ContainsKey(model)) loadBaseDataTypes(model);
            //find the database types
            var datatypes = new Dictionary<string, BaseDataType>();
            foreach (var baseDataType in modelBaseDataTypes[model])
            {
                if (baseDataType.databaseProduct.Equals(databaseType, StringComparison.InvariantCultureIgnoreCase))
                {
                    datatypes.Add(baseDataType.name, baseDataType);
                }
            }
            return datatypes;
        }
        public static List<BaseDataType> getBaseDatatypesByName(string name, Model model)
        {
            //make sure the BaseDataTypes are loaded
            if (!modelBaseDataTypes.ContainsKey(model)) loadBaseDataTypes(model);
            //find the base datatypes by their name
            var datatypes = new List<BaseDataType>();
            foreach (var baseDataType in modelBaseDataTypes[model])
            {
                if (baseDataType.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    datatypes.Add(baseDataType);
                }
            }
            return datatypes;
        }
        /// <summary>
        /// creates a database based on the given package
        /// </summary>
        /// <param name="package">the «database»package containing the database</param>
        /// <param name="compareOnly">if true then no indexes or check constraints are generated because we don't need them when comparing</param>
        /// <returns>the created database</returns>
        public Database createDataBase(Package package, bool compareOnly = false)
        {
            return new Database(package, this, this._strategyFactory.getStrategy<Database>(), compareOnly);
        }
        public Database createDatabase(string name)
        {
            return new Database(name, this, this._strategyFactory.getStrategy<Database>());
        }
        public DataType createDataType(string compositeName)
        {
            string baseTypeName;
            int length = 0;
            int precision = 0;
            if (compositeName.Contains("("))
            {
                baseTypeName = compositeName.Substring(0, compositeName.IndexOf("("));
                string scaleString = compositeName.Substring(compositeName.IndexOf("(") + 1);
                if (scaleString.Contains(","))
                {
                    int.TryParse(scaleString.Substring(0, scaleString.IndexOf(",")), out length);
                    string precisionString = scaleString.Substring(scaleString.IndexOf(",") + 1, scaleString.Length - scaleString.IndexOf(",") - 2);
                    int.TryParse(precisionString, out precision);
                }
                else
                {
                    int.TryParse(scaleString.Substring(0, scaleString.Length - 1), out length);
                }
            }
            else
            {
                baseTypeName = compositeName;
            }
            BaseDataType basetype = this.baseDataTypes.FirstOrDefault(x => x.name == baseTypeName) as BaseDataType;
            if (basetype != null)
            {
                return new DataType(basetype, length, precision);
            }
            else
            {
                return null;
            }

        }
    }
}
