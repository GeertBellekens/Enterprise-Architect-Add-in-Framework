using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using EA;
using EAAddinFramework.EASpecific;
using EAAddinFramework.Utilities;
using UML = TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class Model : UML.Extended.UMLModel
    {
        private global::EA.Repository _wrappedModel;
        private IWin32Window _mainEAWindow;
        private RepositoryType? _repositoryType;
        private static string _applicationFullPath;


        public bool useCache { get; private set; } = false;
        private Dictionary<int, ElementWrapper> elementsByID = new Dictionary<int, ElementWrapper>();
        private Dictionary<string, ElementWrapper> elementsByGUID = new Dictionary<string, ElementWrapper>();

        private Dictionary<int, AttributeWrapper> attributesByID = new Dictionary<int, AttributeWrapper>();
        private Dictionary<string, AttributeWrapper> attributesByGUID = new Dictionary<string, AttributeWrapper>();


        public int EAVersion { get { return this._wrappedModel.LibraryVersion; } }
        public Boolean isLiteEdition { get { return this.wrappedModel.EAEdition == global::EA.EAEditionTypes.piLite; } }
        public string currentUserID { get { return this.wrappedModel.GetCurrentLoginUser(true); } }

        public string convertFromEANotes(string EANotes, string newFormat)
        {
            return this.wrappedModel.GetFormatFromField(newFormat, EANotes);
        }
        public string convertToEANotes(string externalNotes, string externalFormat)
        {
            return this.wrappedModel.GetFieldFromFormat(externalFormat, externalNotes);
        }
        public void flushCache()
        {
            elementsByID.Clear();
            elementsByGUID.Clear();
            attributesByID.Clear();
            attributesByGUID.Clear();
        }
        public void activateTab(string tabName)
        {
            this.wrappedModel.ActivateTab(tabName);
        }
        public bool isTabOpen(string tabName)
        {
            return this.wrappedModel.IsTabOpen(tabName) > 0;
        }
        /// <summary>
        /// returns the full path of the running ea.exe
        /// </summary>
        public static string applicationFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationFullPath))
                {
                    Process[] processes = Process.GetProcessesByName("EA");
                    foreach (var process in processes)
                    {
                        try
                        {
                            _applicationFullPath = process.MainModule.FileName;
                            break;
                        }
                        catch (Exception)
                        {
                            //swallow exception because of access denied
                            //this can happen in case of terminal services solution where there are processes started by other users.
                            //we just have to find the process belonging to this user.
                        }
                    }
                }
                return _applicationFullPath;
            }
        }
        public string projectGUID
        {
            get
            {
                return this.wrappedModel.ProjectGUID;
            }
        }

        public List<string> getStatusses()
        {
            var statusses = new List<string>();
            var statusList = this.wrappedModel.GetReferenceList("Status");
            for (short i = 0; i < statusList.Count; i++)
            {
                statusses.Add(statusList.GetAt(i));
            }
            return statusses;
        }

        /// <summary>
        /// returns the type of repository backend.
        /// This is mostly needed to adjust to sql to the specific sql dialect
        /// </summary>
        public RepositoryType repositoryType
        {
            get
            {
                if (!this._repositoryType.HasValue)
                {
                    this._repositoryType = getRepositoryType();
                }
                return _repositoryType.Value;
            }
        }
        /// <summary>
        /// the main EA window to use when opening properties dialogs
        /// </summary>
        public IWin32Window mainEAWindow
        {
            get
            {
                if //(true)
                    (this._mainEAWindow == null)
                {
                    List<Process> allProcesses = new List<Process>(Process.GetProcesses());
                    Process proc = allProcesses.Find(pr => pr.ProcessName == "EA");
                    //if we don't find the process then we set the mainwindow to null
                    if (proc == null
                           || proc.MainWindowHandle == null)
                    {
                        this._mainEAWindow = null;
                    }
                    else
                    {
                        //found it. Create new WindowWrapper
                        this._mainEAWindow = new WindowWrapper(proc.MainWindowHandle);
                    }
                }
                return this._mainEAWindow;
            }
        }

        /// Creates a model connecting to the first running instance of EA
        public Model()
        {
            object obj = Marshal.GetActiveObject("EA.App");
            global::EA.App eaApp = obj as global::EA.App;
            this.initialize(eaApp.Repository);
        }
        /// <summary>
        /// (re)initialises this model with the given ea repository object
        /// </summary>
        /// <param name="eaRepository">the ea repository object</param>
        public void initialize(global::EA.Repository eaRepository)
        {
            _wrappedModel = eaRepository;
        }
        /// constructor creates EAModel based on the given repository
        public Model(global::EA.Repository eaRepository, bool useCache = false)
        {
            this.initialize(eaRepository);
            this.useCache = useCache;
        }
        public UserControl addWindow(string title, string fullControlName)
        {
            var control = this.wrappedModel.AddWindow(title, fullControlName) as UserControl;
            this.showWindow(title);
            return control;
        }
        public void showWindow(string title)
        {
            this.wrappedModel.ShowAddinWindow(title);
        }
        public UserControl addTab(string title, string fullControlName)
        {
            return this.wrappedModel.AddTab(title, fullControlName) as UserControl;
        }
        public void showTab(string title)
        {
            this.wrappedModel.ActivateTab(title);
        }
        [Obsolete("Use EAAddinFramework.Utilities.ScriptingInteropHelper.toArrayList() ")]
        public ArrayList toArrayList(IEnumerable collection)
        {
            return new ScriptingInteropHelper().toArrayList(collection);
        }
        [Obsolete("Use EAAddinFramework.Utilities.ScriptingInteropHelper.toObject() ")]
        public Object toObject(object someObject)
        {
            return new ScriptingInteropHelper().toObject(someObject);
        }
        public UML.Classes.Kernel.Package selectedTreePackage
        {
            get
            {
                return this.factory.createElement(
                    this.wrappedModel.GetTreeSelectedPackage())
                    as UML.Classes.Kernel.Package;
            }
        }
        /// the Element currently selected in EA
        public UML.Classes.Kernel.Element selectedElement
        {
            get
            {
                Object selectedItem;
                try
                {
                    this.wrappedModel.GetContextItem(out selectedItem);
                    return this.factory.createElement(selectedItem);
                }
                catch (COMException)
                {
                    //something went wrong
                    return null;
                }


            }
            set
            {
                if (value is Package)
                {
                    this.wrappedModel.ShowInProjectView(((Package)value).wrappedPackage);
                }
                else if (value is ElementWrapper)
                {
                    this.wrappedModel.ShowInProjectView(((ElementWrapper)value).wrappedElement);
                }
                else if (value is Operation)
                {
                    this.wrappedModel.ShowInProjectView(((Operation)value).wrappedOperation);
                }
                else if (value is AttributeWrapper)
                {
                    this.wrappedModel.ShowInProjectView(((AttributeWrapper)value).wrappedAttribute);
                }
                else if (value is Parameter)
                {
                    Operation operation = (Operation)((Parameter)value).operation;
                    this.wrappedModel.ShowInProjectView(operation.wrappedOperation);
                }
                else if (value is ConnectorWrapper)
                {
                    var connector = (ConnectorWrapper)value;
                    var diagrams = connector.getDependentDiagrams();
                    if (diagrams.Count > 0)
                    {
                        diagrams[0].open();
                        diagrams[0].selectItem(connector);
                    }
                }
            }
        }

        /// returns the correct type of factory for this model
        public UML.Extended.UMLFactory factory
        {
            get { return Factory.getInstance(this); }
        }

        public ElementWrapper getElementFromCache(int id)
        {
            //check if element exists in cache and return it if found
            ElementWrapper elementWrapper;
            if (this.useCache)
            {
                if (elementsByID.TryGetValue(id, out elementWrapper))
                {
                    return elementWrapper;
                }
            }
            //nothing found, return null
            return null;
        }
        /// Finds the EA.Element with the given id and returns an EAElementwrapper 
        /// wrapping this element.
        public ElementWrapper getElementWrapperByID(int id)
        {
            //don't even try if the ID is not a positive number
            if (id <= 0) return null;
            //check if element exists in cache and return it if found
            ElementWrapper elementWrapper = this.getElementFromCache(id);
            if (elementWrapper != null) return elementWrapper; //found it
            
            //not found in cache (or cache not used) get the regular way
            global::EA.Element eaElement;
            try
            {
                eaElement = this.wrappedModel.GetElementByID(id);
            }
            catch (Exception)
            {
                // element not found, return null
                return null;
            }
            //create the elementWrapper
            elementWrapper = this.factory.createElement(eaElement) as ElementWrapper;
            //add to cache
            if (this.useCache) addElementToCache(elementWrapper);
            //return it
            return elementWrapper;
        }
        /// <summary>
        /// close the tab with the given name
        /// </summary>
        /// <param name="appTitle">the name of the tab to close</param>
        public void closeTab(string appTitle)
        {
            if (this.wrappedModel.IsTabOpen(appTitle) > 0)
                this.wrappedModel.RemoveTab(appTitle);
        }

        /// <summary>
        /// Finds the EA.Element with the given GUID and returns an EAElementwrapper 
        /// wrapping this element.
        /// </summary>
        /// <param name="GUID">the GUID of the element</param>
        /// <returns>the element with the given GUID</returns>
        public ElementWrapper getElementWrapperByGUID(string GUID)
        {
            //check if element exists in cache and return it if found
            ElementWrapper elementWrapper;
            if (this.useCache)
            {
                if (elementsByGUID.TryGetValue(GUID, out elementWrapper))
                {
                    return elementWrapper;
                }
            }
            global::EA.Element eaElement;
            try
            {
                eaElement = this.wrappedModel.GetElementByGuid(GUID);
            }
            catch (Exception)
            {
                // element not found, return null
                return null;
            }
            //if not found then return empty
            if (eaElement == null) return null;
            //create elementWrapper
            elementWrapper = this.factory.createElement(eaElement) as ElementWrapper;
            //add to cache
            if (this.useCache) addElementToCache(elementWrapper);
            //return it
            return elementWrapper;
        }
        public void addElementToCache(ElementWrapper element)
        {
            if (!this.elementsByID.ContainsKey(element.id))
                this.elementsByID.Add(element.id, element);
            if (!this.elementsByGUID.ContainsKey(element.uniqueID))
                this.elementsByGUID.Add(element.uniqueID, element);
        }

        public void refreshDiagram(Diagram diagram)
        {
            this.wrappedModel.ReloadDiagram(diagram.DiagramID);
        }

        public UML.Classes.Kernel.Element getElementByGUID(string GUIDstring)
        {
            UML.Classes.Kernel.Element foundElement = null;
            //first try elementwrapper
            foundElement = this.getElementWrapperByGUID(GUIDstring);
            //then try Attribute
            if (foundElement == null)
            {
                foundElement = this.getAttributeWrapperByGUID(GUIDstring);
            }
            //then try Operation
            if (foundElement == null)
            {
                foundElement = this.getOperationByGUID(GUIDstring);
            }
            //then try ConnectorWrapper
            if (foundElement == null)
            {
                foundElement = this.getRelationByGUID(GUIDstring);
            }
            //then try Parameter
            if (foundElement == null)
            {
                foundElement = this.getParameterByGUID(GUIDstring);
            }
            return foundElement;
        }

        /// <summary>
        /// returns the elementwrappers that are identified by the Object_ID's returned by the given query
        /// </summary>
        /// <param name="sqlQuery">query returning the Object_ID's</param>
        /// <returns>elementwrappers returned by the query</returns>
        public List<ElementWrapper> getElementWrappersByQuery(string sqlQuery)
        {
            // get the nodes with the name "ObjectID"
            XmlDocument xmlObjectIDs = this.SQLQuery(sqlQuery);
            XmlNodeList objectIDNodes = xmlObjectIDs.SelectNodes(formatXPath("//Object_ID"));
            List<ElementWrapper> elements = new List<ElementWrapper>();

            foreach (XmlNode objectIDNode in objectIDNodes)
            {
                ElementWrapper element = this.getElementWrapperByID(int.Parse(objectIDNode.InnerText));
                if (element != null)
                {
                    elements.Add(element);
                }
            }
            return elements;
        }
        /// <summary>
        /// returns the first [maxresults] items who's name starts with the given searchText
        /// </summary>
        /// <param name="searchText">the first part of the name to match</param>
        /// <param name="maxResults">the number of results required</param>
        /// <param name="elements">indicates whether elements should be selected</param>
        /// <param name="operations">indicates whether operations should be selected</param>
        /// <param name="attributes">indicates whether attributes should be selected</param>
        /// <param name="diagrams">indicates whether diagrams should be selected</param>
        /// <returns></returns>
        public List<UML.Extended.UMLItem> getQuickSearchResults(string searchText, int maxResults, bool elements, bool operations, bool attributes, bool diagrams)
        {
            List<UML.Extended.UMLItem> results = new List<UML.Extended.UMLItem>();
            if (elements)
            {
                // get elements
                string SQLSelectElements = @"select top " + maxResults + @" o.Object_ID from t_object o 
								where lcase(o.Name) like lcase('" + searchText + @"%')
								order by o.Name, o.Object_ID";
                results.AddRange(this.getElementWrappersByQuery(SQLSelectElements).Cast<UML.Extended.UMLItem>().ToList());
            }
            if (operations)
            {
                // get operations
                string SQLSelectOperations = @"select top " + maxResults + @" o.OperationID from t_operation o 
								where lcase(o.Name) like lcase('" + searchText + @"%')
								order by o.Name, o.OperationID";
                results.AddRange(this.getOperationsByQuery(SQLSelectOperations).Cast<UML.Extended.UMLItem>().ToList());
            }
            if (attributes)
            {
                // get attributes
                string SQLSelectAttributes = @"select top " + maxResults + @" a.ea_guid from t_attribute a 
							where lcase(a.Name) like lcase('" + searchText + @"%')
							order by a.Name, a.ea_guid";
                results.AddRange(this.getAttributesByQuery(SQLSelectAttributes).Cast<UML.Extended.UMLItem>().ToList());
            }
            if (diagrams)
            {
                // get diagrams
                string SQLSelectDiagrams = @"select top " + maxResults + @" d.Diagram_ID from t_diagram d 
							where  lcase(d.Name) like lcase('" + searchText + @"%')
							order by d.Name, d.Diagram_ID";
                results.AddRange(this.getDiagramsByQuery(SQLSelectDiagrams).Cast<UML.Extended.UMLItem>().ToList());
            }

            //sort alphabetically by name
            results = results.OrderBy(x => x.name).ToList();

            //we need only the first maxresults
            if (results.Count > maxResults)
            {
                results = results.GetRange(0, maxResults);
            }
            return results;
        }

        /// <summary>
        /// gets the Attribute with the given ID
        /// </summary>
        /// <param name="attributeID">the attribute's ID</param>
        /// <returns>the Attribute with the given ID</returns>
        public Attribute getAttributeByID(int attributeID)
        {
            try
            {
                return this.getAttributeWrapperByID(attributeID) as Attribute;
            }
            catch (Exception)
            {
                // attribute not found, return null
                return null;
            }

        }
        /// <summary>
        /// gets the Attribute with the given ID
        /// </summary>
        /// <param name="attributeID">the attribute's ID</param>
        /// <returns>the Attribute with the given ID</returns>
        public AttributeWrapper getAttributeWrapperByID(int attributeID)
        {
            //don't even try if not > 0
            if (attributeID <= 0) return null;

            AttributeWrapper attributeWrapper;
            if (useCache)
            {
                if (attributesByID.TryGetValue(attributeID, out attributeWrapper))
                {
                    return attributeWrapper;
                }
            }
            global::EA.Attribute eaAttribute;
            try
            {
                eaAttribute = this.wrappedModel.GetAttributeByID(attributeID);
            }
            catch (Exception)
            {
                // attribute not found, return null
                return null;
            }
            //return null if not found
            if (eaAttribute == null) return null;
            //create new attributeWrapper
            attributeWrapper = this.factory.createElement(eaAttribute) as AttributeWrapper;
            //add to cache
            if (useCache) this.addAttributeToCache(attributeWrapper);
            //return
            return attributeWrapper;
        }
        /// <summary>
        /// gets the Attribute with the given GUID
        /// </summary>
        /// <param name="GUID">the attribute's GUID</param>
        /// <returns>the Attribute with the given GUID</returns>
        public AttributeWrapper getAttributeWrapperByGUID(string GUID)
        {
            AttributeWrapper attributeWrapper;
            if (useCache)
            {
                if (attributesByGUID.TryGetValue(GUID, out attributeWrapper))
                {
                    return attributeWrapper;
                }
            }
            global::EA.Attribute eaAttribute;
            try
            {
                eaAttribute = this.wrappedModel.GetAttributeByGuid(GUID);
            }
            catch (Exception)
            {
                // attribute not found, return null
                return null;
            }
            //return null if not found
            if (eaAttribute == null) return null;
            //create new attributeWrapper
            attributeWrapper = this.factory.createElement(eaAttribute) as AttributeWrapper;
            //add to cache
            if (useCache) this.addAttributeToCache(attributeWrapper);
            //return
            return attributeWrapper;
        }
        private void addAttributeToCache(AttributeWrapper attributeWrapper)
        {
            this.attributesByID.Add(attributeWrapper.id, attributeWrapper);
            this.attributesByGUID.Add(attributeWrapper.uniqueID, attributeWrapper);
        }
        /// <summary>
        /// gets the parameter by its GUID.
        /// This is a tricky one since EA doesn't provide a getParameterByGUID operation
        /// we have to first get the operation, then loop the pamarameters to find the one
        /// with the GUID
        /// </summary>
        /// <param name="GUID">the parameter's GUID</param>
        /// <returns>the Parameter with the given GUID</returns>
        public ParameterWrapper getParameterByGUID(string GUID)
        {

            //first need to get the operation for the parameter
            string getOperationSQL = @"select p.OperationID from t_operationparams p
    									where p.ea_guid = '" + GUID + "'";
            //first get the operation id
            List<Operation> operations = this.getOperationsByQuery(getOperationSQL);
            if (operations.Count > 0)
            {
                // the list of operations should only contain one operation
                Operation operation = operations[0];
                foreach (ParameterWrapper parameter in operation.ownedParameters)
                {
                    if (parameter.ID == GUID)
                    {
                        return parameter;
                    }
                }
            }
            //parameter not found, return null
            return null;
        }

        public UML.Diagrams.Diagram currentDiagram
        {
            get
            {
                return ((Factory)this.factory).createDiagram
                  (this.wrappedModel.GetCurrentDiagram());
            }
            set { this.wrappedModel.OpenDiagram(((Diagram)value).DiagramID); }
        }

        public Diagram getDiagramByID(int diagramID)
        {
            return ((Factory)this.factory).createDiagram
              (this.wrappedModel.GetDiagramByID(diagramID)) as Diagram;
        }

        public Diagram getDiagramByGUID(string diagramGUID)
        {
            return ((Factory)this.factory).createDiagram
              (this.wrappedModel.GetDiagramByGuid(diagramGUID)) as Diagram;
        }

        public ConnectorWrapper getRelationByID(int relationID)
        {
            return ((Factory)this.factory).createElement
              (this.wrappedModel.GetConnectorByID(relationID)) as ConnectorWrapper;
        }

        public ConnectorWrapper getRelationByGUID(string relationGUID)
        {
            return ((Factory)this.factory).createElement
              (this.wrappedModel.GetConnectorByGuid(relationGUID)) as ConnectorWrapper;
        }

        public List<ConnectorWrapper> getRelationsByQuery(string SQLQuery)
        {
            // get the nodes with the name "Connector_ID"
            XmlDocument xmlrelationIDs = this.SQLQuery(SQLQuery);
            XmlNodeList relationIDNodes =
                xmlrelationIDs.SelectNodes(formatXPath("//Connector_ID"));
            List<ConnectorWrapper> relations = new List<ConnectorWrapper>();
            foreach (XmlNode relationIDNode in relationIDNodes)
            {
                int relationID;
                if (int.TryParse(relationIDNode.InnerText, out relationID))
                {
                    ConnectorWrapper relation = this.getRelationByID(relationID);
                    relations.Add(relation);
                }
            }
            return relations;
        }

        public List<Attribute> getAttributesByQuery(string SQLQuery)
        {
            // get the nodes with the name "ea_guid"
            XmlDocument xmlAttributeIDs = this.SQLQuery(SQLQuery);
            XmlNodeList attributeIDNodes = xmlAttributeIDs.SelectNodes(formatXPath("//ea_guid"));
            List<Attribute> attributes = new List<Attribute>();

            foreach (XmlNode attributeIDNode in attributeIDNodes)
            {
                Attribute attribute = this.getAttributeWrapperByGUID(attributeIDNode.InnerText) as Attribute;
                if (attribute != null)
                {
                    attributes.Add(attribute);
                }
            }
            return attributes;
        }
        public List<Parameter> getParametersByQuery(string SQLQuery)
        {
            // get the nodes with the name "ea_guid"
            XmlDocument xmlParameterIDs = this.SQLQuery(SQLQuery);
            XmlNodeList parameterIDNodes = xmlParameterIDs.SelectNodes(formatXPath("//ea_guid"));
            List<Parameter> parameters = new List<Parameter>();

            foreach (XmlNode parameterIDNode in parameterIDNodes)
            {
                Parameter parameter = this.getParameterByGUID(parameterIDNode.InnerText);
                if (parameter != null)
                {
                    parameters.Add(parameter);
                }
            }
            return parameters;
        }
        public List<Operation> getOperationsByQuery(string SQLQuery)
        {
            // get the nodes with the name "OperationID"
            XmlDocument xmlOperationIDs = this.SQLQuery(SQLQuery);
            XmlNodeList operationIDNodes = xmlOperationIDs.SelectNodes(formatXPath("//OperationID"));
            List<Operation> operations = new List<Operation>();

            foreach (XmlNode operationIDNode in operationIDNodes)
            {
                int operationID;
                if (int.TryParse(operationIDNode.InnerText, out operationID))
                {
                    Operation operation = this.getOperationByID(operationID) as Operation;
                    if (operation != null)
                    {
                        operations.Add(operation);
                    }
                }

            }
            return operations;
        }
        /// <summary>
        /// formats an xpath accordign to the type of database.
        /// For Oracle and Firebird it should be ALL CAPS
        /// </summary>
        /// <param name="xpath">the xpath to format</param>
        /// <returns>the formatted xpath</returns>
        public string formatXPath(string xpath)
        {
            switch (this.repositoryType)
            {

                case RepositoryType.ORACLE:
                case RepositoryType.FIREBIRD:
                    return xpath.ToUpper();
                case RepositoryType.POSTGRES:
                    return xpath.ToLower();
                default:
                    return xpath;
            }
        }
        /// <summary>
        /// escapes a literal string so it can be inserted using sql
        /// </summary>
        /// <param name="sqlString">the string to be escaped</param>
        /// <returns>the escaped string</returns>
        public string escapeSQLString(string sqlString)
        {
            string escapedString = sqlString;
            switch (this.repositoryType)
            {
                case RepositoryType.MYSQL:
                case RepositoryType.POSTGRES:
                    // replace backslash "\" by double backslash "\\"
                    escapedString = escapedString.Replace(@"\", @"\\");
                    break;
            }
            // ALL DBMS types: replace the single qoutes "'" by double single quotes "''"
            escapedString = escapedString.Replace("'", "''");
            return escapedString;
        }
        /// generic query operation on the model.
        /// Returns results in an xml format
        public XmlDocument SQLQuery(string sqlQuery)
        {
            sqlQuery = this.formatSQL(sqlQuery);
            XmlDocument results = new XmlDocument();
            results.LoadXml(this.wrappedModel.SQLQuery(sqlQuery));
            return results;
        }

        /// <summary>
        /// sets the correct wildcards depending on the database type.
        /// changes '%' into '*' if on ms access
        /// and _ into ? on msAccess
        /// </summary>
        /// <param name="sqlQuery">the original query</param>
        /// <returns>the fixed query</returns>
        private string formatSQL(string sqlQuery)
        {
            sqlQuery = replaceSQLWildCards(sqlQuery);
            sqlQuery = formatSQLTop(sqlQuery);
            sqlQuery = formatSQLFunctions(sqlQuery);
            return sqlQuery;
        }

        /// <summary>
        /// Operation to translate SQL functions in there equivalents in different sql syntaxes
        /// supported functions:
        /// 
        /// - lcase -> lower in T-SQL (SQLSVR and ASA)
        /// - like -> ilike in PostGres
        /// </summary>
        /// <param name="sqlQuery">the query to format</param>
        /// <returns>a query with traslated functions</returns>
        private string formatSQLFunctions(string sqlQuery)
        {
            string formattedSQL = sqlQuery;
            //lcase -> lower in T-SQL (SQLSVR and ASA and Oracle and FireBird)
            if (this.repositoryType == RepositoryType.SQLSVR ||
                this.repositoryType == RepositoryType.ASA ||
                   this.repositoryType == RepositoryType.ORACLE ||
                   this.repositoryType == RepositoryType.FIREBIRD ||
                   this.repositoryType == RepositoryType.POSTGRES)
            {
                formattedSQL = formattedSQL.Replace("lcase(", "lower(");
            }
            // like -> ilike
            if (this.repositoryType == RepositoryType.POSTGRES)
            {
                formattedSQL = formattedSQL.Replace(" like ", " ilike ");
            }
            return formattedSQL;
        }

        /// <summary>
        /// limiting the number of results in an sql query is different on different platforms.
        /// 
        /// "SELECT TOP N" is used on
        /// SQLSVR
        /// ADOJET
        /// ASA
        /// OPENEDGE
        /// ACCESS2007
        /// 
        /// "WHERE rowcount <= N" is used on
        /// ORACLE
        /// 
        /// "LIMIT N" is used on
        /// MYSQL
        /// POSTGRES
        /// 
        /// This operation will replace the SELECT TOP N by the appropriate sql syntax depending on the repositorytype
        /// </summary>
        /// <param name="sqlQuery">the sql query to format</param>
        /// <returns>the formatted sql query </returns>
        private string formatSQLTop(string sqlQuery)
        {
            string formattedQuery = sqlQuery;
            string selectTop = "select top ";
            int begintop = sqlQuery.ToLower().IndexOf(selectTop);
            if (begintop >= 0)
            {
                int beginN = begintop + selectTop.Length;
                int endN = sqlQuery.ToLower().IndexOf(" ", beginN) + 1;
                if (endN > beginN)
                {
                    string N = sqlQuery.ToLower().Substring(beginN, endN - beginN);
                    string selectTopN = sqlQuery.Substring(begintop, endN);
                    switch (this.repositoryType)
                    {
                        case RepositoryType.ORACLE:
                            // remove "top N" clause
                            formattedQuery = formattedQuery.Replace(selectTopN, "select ");
                            // find where clause
                            string whereString = "where ";
                            int beginWhere = formattedQuery.ToLower().IndexOf(whereString);
                            string rowcountCondition = "rownum <= " + N + " and ";
                            // add the rowcount condition
                            formattedQuery = formattedQuery.Insert(beginWhere + whereString.Length, rowcountCondition);
                            break;
                        case RepositoryType.MYSQL:
                        case RepositoryType.POSTGRES:
                            // remove "top N" clause
                            formattedQuery = formattedQuery.Replace(selectTopN, "select ");
                            string limitString = " limit " + N;
                            // add limit clause
                            formattedQuery = formattedQuery + limitString;
                            break;
                        case RepositoryType.FIREBIRD:
                            // in firebird top becomes first
                            formattedQuery = formattedQuery.Replace(selectTopN, selectTopN.Replace("top", "first"));
                            break;
                    }
                }
            }
            return formattedQuery;
        }
        /// <summary>
        /// replace the wildcards in the given sql query string to match either MSAccess or ANSI syntax
        /// </summary>
        /// <param name="sqlQuery">the sql string to edit</param>
        /// <returns>the same sql query, but with its wildcards replaced according to the required syntax</returns>
        private string replaceSQLWildCards(string sqlQuery)
        {
            bool msAccess = this.repositoryType == RepositoryType.ADOJET;
            int beginLike = sqlQuery.IndexOf("like", StringComparison.InvariantCultureIgnoreCase);
            if (beginLike > 1)
            {
                int beginString = sqlQuery.IndexOf("'", beginLike + "like".Length);
                if (beginString > 0)
                {
                    int endString = sqlQuery.IndexOf("'", beginString + 1);
                    if (endString > beginString)
                    {
                        string originalLikeString = sqlQuery.Substring(beginString + 1, endString - beginString);
                        string likeString = originalLikeString;
                        if (msAccess)
                        {
                            likeString = likeString.Replace('%', '*');
                            likeString = likeString.Replace('_', '?');
                            likeString = likeString.Replace('^', '!');
                        }
                        else
                        {
                            likeString = likeString.Replace('*', '%');
                            likeString = likeString.Replace('?', '_');
                            likeString = likeString.Replace('#', '_');
                            likeString = likeString.Replace('!', '^');
                        }
                        string next = string.Empty;
                        if (endString < sqlQuery.Length)
                        {
                            next = replaceSQLWildCards(sqlQuery.Substring(endString + 1));
                        }
                        sqlQuery = sqlQuery.Substring(0, beginString + 1) + likeString + next;

                    }
                }
            }
            return sqlQuery;
        }
        /// <summary>
        /// Gets the Repository type for this model
        /// </summary>
        /// <returns></returns>
        public RepositoryType getRepositoryType()
        {
            var eaType = this.wrappedModel.RepositoryType();
            switch(eaType)
            {
                case "JET":
                    return RepositoryType.ADOJET;
                case "FIREBIRD":
                    return RepositoryType.FIREBIRD;
                case "ACCESS2007":
                    return RepositoryType.ACCESS2007;
                case "ASA":
                    return RepositoryType.ASA;
                case "SQLSVR":
                    return RepositoryType.SQLSVR;
                case "MYSQL":
                    return RepositoryType.MYSQL;
                case "ORACLE":
                    return RepositoryType.ORACLE;
                case "POSTGRES":
                    return RepositoryType.POSTGRES;
                default:
                    return RepositoryType.ADOJET;
            }
        }
        /// <summary>
        /// saves unsaved changes to an opened diagram
        /// </summary>
        /// <param name="diagram">the diagram that is currently opened</param>
        public void saveOpenedDiagram(UML.Diagrams.Diagram diagram)
        {
            this.wrappedModel.SaveDiagram(((Diagram)diagram).DiagramID);
        }
        public void adviseChange(UML.Classes.Kernel.Element element)
        {
            if (element is ElementWrapper)
            {
                this.wrappedModel.AdviseElementChange(((ElementWrapper)element).id);
            }
            else if (element is ConnectorWrapper)
            {
                this.wrappedModel.AdviseConnectorChange(((ConnectorWrapper)element).id);
            }
        }

        public void saveElement(UML.Classes.Kernel.Element element)
        {
            ((Element)element).save();
        }

        public void saveDiagram(UML.Diagrams.Diagram diagram)
        {
            throw new NotImplementedException();
        }

        public ElementWrapper getElementWrapperByPackageID(int packageID)
        {
            try
            {
                return this.factory.createElement(this.wrappedModel.GetPackageByID(packageID)) as ElementWrapper;
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                if (e.Message.Contains("Can't find matching ID"))
                {
                    return null;
                }
                else
                {
                    throw e;
                }
            }
        }

        //returns a list of diagrams according to the given query.
        //the given query should return a list of diagram id's
        public List<Diagram> getDiagramsByQuery(string sqlGetDiagrams)
        {
            // get the nodes with the name "Diagram_ID"
            XmlDocument xmlDiagramIDs = this.SQLQuery(sqlGetDiagrams);
            XmlNodeList diagramIDNodes =
                xmlDiagramIDs.SelectNodes(formatXPath("//Diagram_ID"));
            List<Diagram> diagrams = new List<Diagram>();
            foreach (XmlNode diagramIDNode in diagramIDNodes)
            {
                int diagramID;
                if (int.TryParse(diagramIDNode.InnerText, out diagramID))
                {
                    Diagram diagram = this.getDiagramByID(diagramID);
                    diagrams.Add(diagram);
                }
            }
            return diagrams;
        }

        public Operation getOperationByGUID(string guid)
        {
            Operation operation = this.factory.createElement(this.wrappedModel.GetMethodByGuid(guid)) as Operation;
            if (operation == null)
            {
                List<OperationTag> tags = this.getOperationTagsWithValue(guid);

                foreach (OperationTag tag in tags)
                {
                    if (tag != null && tag.name == "ea_guid")
                    {
                        operation = tag.owner as Operation;
                    }
                }
            }
            return operation;

        }
        public Operation getOperationByID(int operationID)
        {
            return this.factory.createElement(this.wrappedModel.GetMethodByID(operationID)) as Operation;
        }


        public void executeSQL(string SQLString)
        {
            this.wrappedModel.Execute(SQLString);
        }

        public void selectDiagram(Diagram diagram)
        {
            this.wrappedModel.ShowInProjectView(diagram.wrappedDiagram);
        }
        /// <summary>
        /// finds the item with the given guid
        /// </summary>
        /// <param name="guidString">the string with the guid</param>
        /// <returns>the item that is identified by the given GUID</returns>
        public UML.Extended.UMLItem getItemFromGUID(string guidString)
        {
            try
            {
                if (guidString.Contains("_"))
                {
                    guidString = this.wrappedModel.GetProjectInterface().XMLtoGUID(guidString);
                }
                UML.Extended.UMLItem foundItem = null;
                if (Guid.TryParse(guidString, out var result))
                {
                    foundItem = this.getElementByGUID(guidString);
                    if (foundItem == null) foundItem = this.getDiagramByGUID(guidString);
                    if (foundItem == null) foundItem = this.getAttributeWrapperByGUID(guidString);
                    if (foundItem == null) foundItem = this.getOperationByGUID(guidString);
                    if (foundItem == null) foundItem = this.getRelationByGUID(guidString);
                }
                else if (long.TryParse(guidString, out var longresult))
                {
                    //try to find a diagramObject based on the instanceID (diagramObjects don't have an ea_guid)
                    //find diagram
                    var sqlGetData = $"select do.Diagram_ID from t_diagramobjects do where do.Instance_ID = {guidString}";
                    var diagram = this.getDiagramsByQuery(sqlGetData).FirstOrDefault();
                    if (diagram != null)
                    {
                        //get element
                        sqlGetData = $"select do.Object_ID from t_diagramobjects do where do.Instance_ID = {guidString}";
                        var element = this.getElementWrappersByQuery(sqlGetData).FirstOrDefault();
                        if (element != null)
                        {
                            //get the diagramObject for this element on this diagram
                            foundItem = diagram.getDiagramElement(element);
                        }
                    }
                }
                return foundItem;
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                if (e.Message.Contains("Can't find matching ID"))
                {
                    return null;
                }
                else
                {
                    throw e;
                }
            }
        }

        public UML.Extended.UMLItem getItemFromFQN(string FQN)
        {
            //split the FQN in the different parts
            UML.Extended.UMLItem foundItem = null;
            foreach (UML.Classes.Kernel.Package package in this.rootPackages)
            {

                foundItem = package.getItemFromRelativePath(FQN.Split('.').ToList<string>());
                if (foundItem != null)
                {
                    break;
                }
            }
            return foundItem;
        }

        public HashSet<UML.Classes.Kernel.Package> rootPackages
        {
            get
            {

                return new HashSet<UML.Classes.Kernel.Package>(this.factory.createElements(this.wrappedModel.Models).Cast<UML.Classes.Kernel.Package>());
            }
        }

        public UML.Diagrams.Diagram selectedDiagram
        {
            get
            {
                object item;
                try
                {
                    this.wrappedModel.GetTreeSelectedItem(out item);
                }
                catch (COMException)
                {
                    //something went wrong (Can't find matching ID)
                    return null;
                }
                global::EA.Diagram diagram = item as global::EA.Diagram;
                if (diagram != null)
                {
                    return this.factory.createDiagram(diagram);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                value.select();

            }
        }
        public TSF.UmlToolingFramework.UML.Extended.UMLItem selectedItem
        {
            get
            {
                UML.Extended.UMLItem item = this.selectedElement;
                if (item == null)
                {
                    item = this.selectedDiagram;
                }
                //get the owner in this thread, not for Connectors as there is a bug in the API that results in a stackoverflow in certain conditions
                if (item != null
                     && !(item is ConnectorWrapper))
                {
                    var owner = item.owner;
                }
                return item;
            }
            set
            {
                if (value is UML.Diagrams.Diagram)
                {
                    this.selectedDiagram = value as UML.Diagrams.Diagram;
                }
                else if (value is UML.Classes.Kernel.Element)
                {
                    this.selectedElement = value as UML.Classes.Kernel.Element;
                }
            }
        }

        public HashSet<UML.Profiles.TaggedValue> getTaggedValuesWithValue(string searchValue, bool includeWildCards = false)
        {
            HashSet<UML.Profiles.TaggedValue> taggedValues = new HashSet<TSF.UmlToolingFramework.UML.Profiles.TaggedValue>();
            if (includeWildCards) searchValue = "%" + searchValue + "%";
            //elements
            foreach (ElementTag elementTag in this.getElementTagsWithValue(searchValue))
            {
                taggedValues.Add(elementTag);
            }
            //attribute
            foreach (AttributeTag attributeTag in this.getAttributeTagsWithValue(searchValue))
            {
                taggedValues.Add(attributeTag);
            }
            //operations
            foreach (OperationTag operationTag in this.getOperationTagsWithValue(searchValue))
            {
                taggedValues.Add(operationTag);
            }
            //parameters
            foreach (ParameterTag parameterTag in this.getParameterTagsWithValue(searchValue))
            {
                taggedValues.Add(parameterTag);
            }
            //relations
            foreach (RelationTag relationTag in this.getRelationTagsWithValue(searchValue))
            {
                taggedValues.Add(relationTag);
            }
            return taggedValues;
        }
        public HashSet<ElementTag> getElementTagsWithValue(string searchValue)
        {
            HashSet<ElementTag> elementTags = new HashSet<ElementTag>();
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                string sqlFindGUIDS = @"select ea_guid from t_objectproperties ot
								where ot.[Value] like '" + searchValue + "'" +
                                        "or ot.[Notes]  like '" + searchValue + "'";
                // get the nodes with the name "ea_guid"
                XmlDocument xmlElementTagGUIDs = this.SQLQuery(sqlFindGUIDS);
                XmlNodeList tagGUIDNodes = xmlElementTagGUIDs.SelectNodes(formatXPath("//ea_guid"));
                foreach (XmlNode guidNode in tagGUIDNodes)
                {
                    ElementTag elementTag = this.getElementTagByGUID(guidNode.InnerText);
                    if (elementTag != null)
                    {
                        elementTags.Add(elementTag);
                    }
                }
            }
            return elementTags;
        }
        public ElementTag getElementTagByGUID(string GUID)
        {
            ElementTag elementTag = null;
            string getElementWrappers = @"select object_id from t_objectproperties
									where ea_guid like '" + GUID + "'";
            XmlDocument xmlElementIDs = this.SQLQuery(getElementWrappers);
            XmlNode elementNode = xmlElementIDs.SelectSingleNode(formatXPath("//object_id"));
            if (elementNode != null)
            {
                int objectID;
                if (int.TryParse(elementNode.InnerText, out objectID))
                {
                    ElementWrapper owner = this.getElementWrapperByID(objectID);
                    if (owner != null)
                    {
                        foreach (TaggedValue taggedValue in owner.taggedValues)
                        {
                            if (taggedValue.ea_guid.Equals(GUID, StringComparison.InvariantCultureIgnoreCase))
                            {
                                elementTag = taggedValue as ElementTag;
                            }
                        }
                    }
                }

            }
            return elementTag;

        }
        public List<AttributeTag> getAttributeTagsWithValue(string searchValue)
        {
            List<AttributeTag> attributeTags = new List<AttributeTag>();
            string sqlFindGUIDS = @"select ea_guid from t_attributetag att
								where att.[VALUE] like '" + searchValue + "'" +
                                    "or att.[NOTES]  like '" + searchValue + "'";
            // get the nodes with the name "ea_guid"
            XmlDocument xmlTagGUIDs = this.SQLQuery(sqlFindGUIDS);
            XmlNodeList tagGUIDNodes = xmlTagGUIDs.SelectNodes(formatXPath("//ea_guid"));
            foreach (XmlNode guidNode in tagGUIDNodes)
            {
                AttributeTag attributeTag = this.getAttributeTagByGUID(guidNode.InnerText);
                if (attributeTag != null)
                {
                    attributeTags.Add(attributeTag);
                }
            }
            return attributeTags;
        }
        public AttributeTag getAttributeTagByGUID(string GUID)
        {
            AttributeTag attributeTag = null;
            string getAttributes = @"select elementid from t_attributetag
									where ea_guid like '" + GUID + "'";
            XmlDocument xmlElementIDs = this.SQLQuery(getAttributes);
            XmlNode elementNode = xmlElementIDs.SelectSingleNode(formatXPath("//elementid"));
            if (elementNode != null)
            {
                int objectID;
                if (int.TryParse(elementNode.InnerText, out objectID))
                {
                    AttributeWrapper owner = this.getAttributeWrapperByID(objectID);
                    if (owner != null)
                    {
                        foreach (TaggedValue taggedValue in owner.taggedValues)
                        {
                            if (taggedValue.ea_guid.Equals(GUID, StringComparison.InvariantCultureIgnoreCase))
                            {
                                attributeTag = taggedValue as AttributeTag;
                            }
                        }
                    }
                }
            }
            return attributeTag;
        }

        public List<OperationTag> getOperationTagsWithValue(string searchValue)
        {
            List<OperationTag> operationTags = new List<OperationTag>();
            string sqlFindGUIDS = @"select ea_guid from t_operationtag opt
								where opt.[VALUE] like '" + searchValue + "'" +
                                    "or opt.[NOTES]  like '" + searchValue + "'";
            // get the nodes with the name "ea_guid"
            XmlDocument xmlTagGUIDs = this.SQLQuery(sqlFindGUIDS);
            XmlNodeList tagGUIDNodes = xmlTagGUIDs.SelectNodes(formatXPath("//ea_guid"));
            foreach (XmlNode guidNode in tagGUIDNodes)
            {
                OperationTag operationTag = this.getOperationTagByGUID(guidNode.InnerText);
                if (operationTag != null)
                {
                    operationTags.Add(operationTag);
                }
            }
            return operationTags;
        }
        public OperationTag getOperationTagByGUID(string GUID)
        {
            OperationTag operationTag = null;
            string getOperations = @"select elementid from t_operationtag
									where ea_guid like '" + GUID + "'";
            XmlDocument xmlElementIDs = this.SQLQuery(getOperations);
            XmlNode elementNode = xmlElementIDs.SelectSingleNode(formatXPath("//elementid"));
            if (elementNode != null)
            {
                int objectID;
                if (int.TryParse(elementNode.InnerText, out objectID))
                {
                    Operation owner = this.getOperationByID(objectID) as Operation;
                    if (owner != null)
                    {
                        foreach (TaggedValue taggedValue in owner.taggedValues)
                        {
                            if (taggedValue.ea_guid.Equals(GUID, StringComparison.InvariantCultureIgnoreCase))
                            {
                                operationTag = taggedValue as OperationTag;
                            }
                        }
                    }
                }
            }
            return operationTag;
        }
        public List<ParameterTag> getParameterTagsWithValue(string searchValue)
        {
            List<ParameterTag> parameterTags = new List<ParameterTag>();
            string sqlFindGUIDS = @"select propertyid from t_taggedvalue
								where notes like '" + searchValue + "'";
            // get the nodes with the name "ea_guid"
            XmlDocument xmlTagGUIDs = this.SQLQuery(sqlFindGUIDS);
            XmlNodeList tagGUIDNodes = xmlTagGUIDs.SelectNodes(formatXPath("//propertyid"));
            foreach (XmlNode guidNode in tagGUIDNodes)
            {
                ParameterTag parameterTag = this.getParameterTagByGUID(guidNode.InnerText);
                if (parameterTag != null)
                {
                    parameterTags.Add(parameterTag);
                }
            }
            return parameterTags;
        }
        public ParameterTag getParameterTagByGUID(string GUID)
        {
            ParameterTag parameterTag = null;
            string getParameters = @"select elementid from t_taggedvalue
									where propertyid like '" + GUID + "'";
            XmlDocument xmlElementIDs = this.SQLQuery(getParameters);
            XmlNode elementNode = xmlElementIDs.SelectSingleNode(formatXPath("//elementid"));
            if (elementNode != null)
            {

                if (elementNode.InnerText.Length > 0)
                {
                    Parameter owner = this.getParameterByGUID(elementNode.InnerText);
                    if (owner != null)
                    {
                        foreach (TaggedValue taggedValue in owner.taggedValues)
                        {
                            if (taggedValue.ea_guid.Equals(GUID, StringComparison.InvariantCultureIgnoreCase))
                            {
                                parameterTag = taggedValue as ParameterTag;
                            }
                        }
                    }
                }
            }
            return parameterTag;
        }
        public List<RelationTag> getRelationTagsWithValue(string searchValue)
        {
            List<RelationTag> relationTags = new List<RelationTag>();
            string sqlFindGUIDS = @"select ea_guid from t_connectortag ct
								where ct.[VALUE] like '" + searchValue + "'" +
                                    "or ct.[NOTES]  like '" + searchValue + "'";
            // get the nodes with the name "ea_guid"
            XmlDocument xmlTagGUIDs = this.SQLQuery(sqlFindGUIDS);
            XmlNodeList tagGUIDNodes = xmlTagGUIDs.SelectNodes(formatXPath("//ea_guid"));
            foreach (XmlNode guidNode in tagGUIDNodes)
            {
                RelationTag relationTag = this.getRelationTagByGUID(guidNode.InnerText);
                if (relationTag != null)
                {
                    relationTags.Add(relationTag);
                }
            }
            return relationTags;
        }
        public RelationTag getRelationTagByGUID(string GUID)
        {
            RelationTag relationTag = null;
            string getRelations = @"select elementid from t_connectortag
									where [ea_guid] like '" + GUID + "'";
            XmlDocument xmlElementIDs = this.SQLQuery(getRelations);
            XmlNode elementNode = xmlElementIDs.SelectSingleNode(formatXPath("//elementid"));
            if (elementNode != null)
            {
                int objectID;
                if (int.TryParse(elementNode.InnerText, out objectID))
                {
                    ConnectorWrapper owner = this.getRelationByID(objectID);
                    if (owner != null)
                    {
                        foreach (TaggedValue taggedValue in owner.taggedValues)
                        {
                            if (taggedValue.ea_guid.Equals(GUID, StringComparison.InvariantCultureIgnoreCase))
                            {
                                relationTag = taggedValue as RelationTag;
                            }
                        }
                    }
                }
            }
            return relationTag;
        }
        /// <summary>
        /// all users defined in this model
        /// </summary>
        public List<User> users
        {
            get
            {

                List<User> userList = new List<User>();
                if (this.isSecurityEnabled)
                {
                    string getUsers = "select u.UserLogin, u.FirstName, u.Surname from t_secuser u";
                    XmlDocument users = this.SQLQuery(getUsers);
                    foreach (XmlNode userNode in users.SelectNodes("//Row"))
                    {
                        string login = string.Empty;
                        string firstName = string.Empty;
                        string lastName = string.Empty;
                        foreach (XmlNode subNode in userNode.ChildNodes)
                        {
                            switch (subNode.Name.ToLower())
                            {
                                case "userlogin":
                                    login = subNode.InnerText;
                                    break;
                                case "firstname":
                                    firstName = subNode.InnerText;
                                    break;
                                case "surname":
                                    lastName = subNode.InnerText;
                                    break;
                            }
                        }
                        userList.Add(((Factory)this.factory).createUser(login, firstName, lastName));
                    }
                }
                else
                {
                    //security not enabled. List of all users is the list of all authors mentioned in the t_object table.
                    string getUsers = "select distinct o.author from t_object o";
                    XmlDocument users = this.SQLQuery(getUsers);
                    foreach (XmlNode authorNode in users.SelectNodes(formatXPath("//author")))
                    {
                        string login = authorNode.InnerText;
                        string firstName = string.Empty;
                        string lastName = string.Empty;
                        //add user	
                        userList.Add(((Factory)this.factory).createUser(login, firstName, lastName));
                    }
                }
                return userList;
            }
        }
        /// <summary>
        /// Contains the currently logged in user.
        /// Returns null is security not enabled.
        /// </summary>
        public User currentUser
        {
            get
            {
                string currentUserLogin = string.Empty;
                if (this.isSecurityEnabled)
                {
                    currentUserLogin = this.wrappedModel.GetCurrentLoginUser(false);
                }
                else
                {
                    currentUserLogin = Environment.UserName;
                }
                return this.users.Find(u => u.login.Equals(currentUserLogin, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        /// <summary>
        /// The working sets defined in this model
        /// </summary>
        public List<WorkingSet> workingSets
        {
            get
            {
                List<WorkingSet> workingSetList = new List<WorkingSet>();
                string getWorkingSets = "select d.docid, d.DocName,d.Author from t_document d where d.DocType = 'WorkItem' order by d.Author, d.DocName";
                XmlDocument workingSets = this.SQLQuery(getWorkingSets);
                foreach (XmlNode workingSetNode in workingSets.SelectNodes("//Row"))
                {
                    string name = string.Empty;
                    string id = string.Empty;
                    string ownerFullName = string.Empty;
                    foreach (XmlNode subNode in workingSetNode.ChildNodes)
                    {
                        switch (subNode.Name.ToLower())
                        {
                            case "docid":
                                id = subNode.InnerText;
                                break;
                            case "docname":
                                name = subNode.InnerText;
                                break;
                            case "author":
                                ownerFullName = subNode.InnerText;
                                break;
                        }
                    }
                    User owner = this.users.Find(u => u.fullName.Equals(ownerFullName, StringComparison.InvariantCultureIgnoreCase));
                    workingSetList.Add(((Factory)this.factory).createWorkingSet(name, id, owner));
                }
                return workingSetList;
            }
        }
        /// <summary>
        /// returns true if security is enabled in this model
        /// </summary>
        public bool isSecurityEnabled
        {
            get
            {
                try
                {
                    this.wrappedModel.GetCurrentLoginUser();
                    return true;
                }
                catch (System.Runtime.InteropServices.COMException e)
                {
                    if (e.Message == "Security not enabled")
                    {
                        return false;
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// opens the properties dialog for this item
        /// </summary>
        /// <param name="item">the item to open the properties dialog for</param>
        public void openProperties(UML.Extended.UMLItem item)
        {
            //get the type string
            string typeString = string.Empty;
            int itemID = 0;
            if (item is Package)
            {
                typeString = "PKG";
                itemID = ((Package)item).packageID;
            }
            else if (item is ElementWrapper)
            {
                typeString = "ELM";
                itemID = ((ElementWrapper)item).id;
            }
            else if (item is Attribute)
            {
                typeString = "ATT";
                itemID = ((Attribute)item).id;
            }
            else if (item is Operation)
            {
                typeString = "OP";
                itemID = ((Operation)item).id;
            }
            else if (item is Diagram)
            {
                typeString = "DGM";
                itemID = ((Diagram)item).DiagramID;
            }
            // TODO: figure out how to open the properties dialog for a connector.	
            //		else if (item is ConnectorWrapper)
            //		{
            //			//typeString = "CON";
            //			typeString = "MSG";
            //			itemID = ((ConnectorWrapper)item).id;
            //		}
            //open the actual dialog
            if (this.mainEAWindow != null
                && typeString != string.Empty
                && itemID != 0)
            {
                string ret = this.wrappedModel.CustomCommand("CFormCommandHelper", "ProcessCommand", "Dlg=" + typeString + ";id=" + itemID.ToString() + ";hwnd=" + this.mainEAWindow.Handle);
            }
        }
        /// <summary>
        /// returns the repository object
        /// </summary>
        /// <returns>the wrapped repository object</returns>
        public global::EA.Repository wrappedModel
        {
            get { return this._wrappedModel; }
        }


        /// <summary>
        /// Show a dialog to the user that allows him to select a package from the model
        /// </summary>
        /// <returns></returns>
        public UML.Classes.Kernel.Package getUserSelectedPackage()
        {
            var allowedtypes = new List<string>();
            allowedtypes.Add("Package");
            return this.getUserSelectedElement(allowedtypes) as UML.Classes.Kernel.Package;
        }
        /// <summary>
        /// Lets the user select an element from the model and return that
        /// </summary>
        /// <param name="allowedTypes">the subtypes of UML.Classes.Kernel.Element that should be used as a filter</param>
        /// <returns>the selected element</returns>
        public UML.Classes.Kernel.Element getUserSelectedElement(List<string> allowedTypes)
        {
            return this.getUserSelectedElement(allowedTypes, null);
        }

        /// <summary>
        /// Lets the user select an element from the model and return that
        /// </summary>
        /// <param name="allowedTypes">the subtypes of UML.Classes.Kernel.Element that should be used as a filter</param>
        /// <returns>the selected element</returns>
        /// <param name = "allowedStereotypes"> the list of stereotypes to filter on</param>
        public UML.Classes.Kernel.Element getUserSelectedElement(List<string> allowedTypes, List<string> allowedStereotypes, string defaultSelectionGUID = null)
        {
            //construct the include string
            string includeString = "IncludedTypes=";
            var eaTypeNames = new List<string>();
            foreach (var allowedType in allowedTypes)
            {
                eaTypeNames.Add(((Factory)this.factory).translateTypeName(allowedType));
            }
            includeString += string.Join(",", allowedTypes);
            //close section
            includeString += ";";
            if (allowedStereotypes != null && allowedStereotypes.Any())
            {
                //add stereotype filter
                includeString += "StereoType="
                                  + string.Join(",", allowedStereotypes)
                                + ";";
            }
            //default to the currently selected package is the default is not given
            if (string.IsNullOrEmpty(defaultSelectionGUID))
            {
                defaultSelectionGUID = this.wrappedModel.GetTreeSelectedPackage()?.PackageGUID;
            }
            if (!string.IsNullOrEmpty(defaultSelectionGUID))
            {
                includeString += "Selection="
                            + defaultSelectionGUID
                            + ";";
            }
            //currenlty only supported for ElementWrappers
            int EAElementID = this.wrappedModel.InvokeConstructPicker(includeString);
            return this.getElementWrapperByID(EAElementID);
        }
        /// <summary>
        /// checks if a tagged value type with the given name exists in the current model
        /// </summary>
        /// <param name="tagName">the name of the tagged value type</param>
        /// <returns>true if it exists</returns>
        public bool taggedValueTypeExists(string tagName)
        {
            this.wrappedModel.PropertyTypes.Refresh();
            foreach (global::EA.PropertyType taggedValueType in this.wrappedModel.PropertyTypes)
            {
                string tagTypeName = taggedValueType.Tag;
                //ignore case or EA will complain
                if (taggedValueType.Tag.Equals(tagName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public void addTaggedValueType(string tagName, string tagDescription, string tagDetail)
        {
            global::EA.PropertyType taggedValueType = (global::EA.PropertyType)this.wrappedModel.PropertyTypes.AddNew(tagName, "");
            taggedValueType.Description = tagDescription;
            taggedValueType.Detail = tagDetail;
            taggedValueType.Update();
        }
        /// <summary>
        /// reload the open diagrams
        /// </summary>
        public void reloadDiagrams()
        {
            this.wrappedModel.RefreshOpenDiagrams(true);
        }
        public UML.Classes.Kernel.Package getCurrentRootPackage()
        {
            return this.getRoootPackage(this.selectedItem);
        }
        public RootPackage getRoootPackage(UML.Extended.UMLItem item)
        {
            //if the item is a RootPackage or null we return the given item
            if (item is RootPackage || item == null) return item as RootPackage;
            //else go up in the hierarchy
            return (getRoootPackage(item.owner as UML.Classes.Kernel.Element));
        }
        /// <summary>
        /// Returns the Object_ID and the ea_guid for one single object
        /// </summary>
        /// <param name="sqlQuery">a select query that returns one single record</param>
        /// <returns>a KeyValuePair with two strings, Object_ID and ea_guid</returns>
        public KeyValuePair<string, string> getObjectIdAndGuid(string sqlQuery)
        {

            XmlDocument xmlIdGuid = this.SQLQuery(sqlQuery);

            //get the node with the name "Object_ID'
            XmlNode ObjectIDNode = xmlIdGuid.SelectSingleNode(formatXPath("//Object_ID"));
            //get the node with the name "ea_guid"
            XmlNode EAGuidNode = xmlIdGuid.SelectSingleNode(formatXPath("//ea_guid"));

            string Object_ID = ObjectIDNode != null ? ObjectIDNode.InnerText : string.Empty;
            string ea_guid = EAGuidNode != null ? EAGuidNode.InnerText : string.Empty;

            return new KeyValuePair<string, string>(Object_ID, ea_guid);
        }
    }
}
