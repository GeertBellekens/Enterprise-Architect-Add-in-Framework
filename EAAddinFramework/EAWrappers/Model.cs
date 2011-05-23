using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Model : UML.UMLModel {
    private global::EA.Repository wrappedModel;

    /// Creates a model connecting to the first running instance of EA
    public Model(){
      object obj = Marshal.GetActiveObject("EA.App");
      global::EA.App eaApp = obj as global::EA.App;
      wrappedModel = eaApp.Repository;
    }

    /// constructor creates EAModel based on the given repository
    public Model(global::EA.Repository eaRepository){
      wrappedModel = eaRepository;
    }

    /// the Element currently selected in EA
    public UML.Classes.Kernel.Element selectedElement {
      get {
        Object selectedItem;
        this.wrappedModel.GetContextItem(out selectedItem);
        return this.factory.createElement(selectedItem);
      }
    }
    
    /// returns the correct type of factory for this model
    public UML.UMLFactory factory {
      get { return Factory.getInstance(this); }
    }

    /// Finds the EA.Element with the given id and returns an EAElementwrapper 
    /// wrapping this element.
    public ElementWrapper getElementWrapperByID(int id){
      try{
        return this.factory.createElement
          (this.wrappedModel.GetElementByID(id)) as ElementWrapper;
      } catch( Exception )  {
        // element not found, return null
        return null;
      }
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
      XmlNodeList objectIDNodes = xmlObjectIDs.SelectNodes("//Object_ID");
      List<ElementWrapper> elements = new List<ElementWrapper>();
      
      foreach( XmlNode objectIDNode in objectIDNodes ) 
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
    /// gets the Attribute with the given GUID
    /// </summary>
    /// <param name="GUID">the attribute's GUID</param>
    /// <returns>the Attribute with the given GUID</returns>
    public Attribute getAttributeByGUID (string GUID)
    {
    	try
    	{
    		return this.factory.createElement(this.wrappedModel.GetAttributeByGuid(GUID)) as Attribute;
    	}catch (Exception)
    	{
    		// attribute not found, return null
    		return null;
    	}
    	
    }
    /// <summary>
    /// gets the parameter by its GUID.
    /// This is a tricky one since EA doesn't provide a getParameterByGUID operation
    /// we have to first get the operation, then loop the pamarameters to find the one
    /// with the GUID
    /// </summary>
    /// <param name="GUID">the parameter's GUID</param>
    /// <returns>the Parameter with the given GUID</returns>
    public ParameterWrapper getParameterByGUID (string GUID)
    {
    	
    		//first need to get the operation for the parameter
    		string getOperationSQL = @"select p.OperationID from t_operationparams p
    									where p.ea_guid = '" + GUID +"'";
    		//first get the operation id
    		List<Operation> operations = this.getOperationsByQuery(getOperationSQL);
    		if (operations.Count > 0)
    		{
    			// the list of operations should only contain one operation
    			Operation operation = operations[0];
    			foreach ( ParameterWrapper parameter in operation.ownedParameters) {
    				if (parameter.ID == GUID) 
    				{
    					return parameter;
    				}
    			}
    		}
    	//parameter not found, return null
    	return null;
    }
    
    public UML.Diagrams.Diagram selectedDiagram {
      get {
        return ((Factory)this.factory).createDiagram
          ( this.wrappedModel.GetCurrentDiagram() );
      }
      set { this.wrappedModel.OpenDiagram(((Diagram)value).DiagramID); }
    }
    
    internal Diagram getDiagramByID(int diagramID){
      return ((Factory)this.factory).createDiagram
        ( this.wrappedModel.GetDiagramByID(diagramID) ) as Diagram;
    }

    internal ConnectorWrapper getRelationByID(int relationID) {
      return ((Factory)this.factory).createElement
        ( this.wrappedModel.GetConnectorByID(relationID)) as ConnectorWrapper;
    }
    
    internal List<ConnectorWrapper> getRelationsByQuery(string SQLQuery){
      // get the nodes with the name "Connector_ID"
      XmlDocument xmlrelationIDs = this.SQLQuery(SQLQuery);
      XmlNodeList relationIDNodes = 
        xmlrelationIDs.SelectNodes("//Connector_ID");
      List<ConnectorWrapper> relations = new List<ConnectorWrapper>();
      foreach( XmlNode relationIDNode in relationIDNodes ) {
        int relationID;
        if (int.TryParse( relationIDNode.InnerText, out relationID)) {
          ConnectorWrapper relation = this.getRelationByID(relationID);
          relations.Add(relation);
        }
      }
      return relations;
    }
    
      internal List<Attribute> getAttributesByQuery(string SQLQuery){
      // get the nodes with the name "ea_guid"
      XmlDocument xmlAttributeIDs = this.SQLQuery(SQLQuery);
      XmlNodeList attributeIDNodes = xmlAttributeIDs.SelectNodes("//ea_guid");
      List<Attribute> attributes = new List<Attribute>();
      
      foreach( XmlNode attributeIDNode in attributeIDNodes ) 
      {
        Attribute attribute = this.getAttributeByGUID(attributeIDNode.InnerText);
        if (attribute != null)
        {
        	attributes.Add(attribute);
        }
      }
      return attributes;
    }
    internal List<Parameter>getParametersByQuery(string SQLQuery)
    {
      // get the nodes with the name "ea_guid"
      XmlDocument xmlParameterIDs = this.SQLQuery(SQLQuery);
      XmlNodeList parameterIDNodes = xmlParameterIDs.SelectNodes("//ea_guid");
      List<Parameter> parameters = new List<Parameter>();
      
      foreach( XmlNode parameterIDNode in parameterIDNodes ) 
      {
        Parameter parameter = this.getParameterByGUID(parameterIDNode.InnerText);
        if (parameter != null)
        {
        	parameters.Add(parameter);
        }
      }
      return parameters;
    }
    internal List<Operation>getOperationsByQuery(string SQLQuery)
    {
      // get the nodes with the name "OperationID"
      XmlDocument xmlOperationIDs = this.SQLQuery(SQLQuery);
      XmlNodeList operationIDNodes = xmlOperationIDs.SelectNodes("//OperationID");
      List<Operation> operations = new List<Operation>();
      
      foreach( XmlNode operationIDNode in operationIDNodes ) 
      {
      	int operationID;
      	if (int.TryParse(operationIDNode.InnerText,out operationID))
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

    /// generic query operation on the model.
    /// Returns results in an xml format
    public XmlDocument SQLQuery(string sqlQuery){
      XmlDocument results = new XmlDocument();
      results.LoadXml(this.wrappedModel.SQLQuery(sqlQuery));
      return results;
    }

    public void saveElement(UML.Classes.Kernel.Element element){
      ((Element)element).save();
    }

    public void saveDiagram(UML.Diagrams.Diagram diagram){
      throw new NotImplementedException();
    }

    internal UML.Classes.Kernel.Element getElementWrapperByPackageID
      (int packageID)
    {
      return this.factory.createElement
        ( this.wrappedModel.GetPackageByID(packageID).Element );
    }

    //returns a list of diagrams according to the given query.
    //the given query should return a list of diagram id's
    internal List<Diagram> getDiagramsByQuery(string sqlGetDiagrams)
    {
        // get the nodes with the name "Diagram_ID"
        XmlDocument xmlDiagramIDs = this.SQLQuery(sqlGetDiagrams);
        XmlNodeList diagramIDNodes =
          xmlDiagramIDs.SelectNodes("//Diagram_ID");
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

    internal UML.Classes.Kernel.Operation getOperationByGUID(string guid)
    {
        return this.factory.createElement(this.wrappedModel.GetMethodByGuid(guid)) as UML.Classes.Kernel.Operation;
    }
    internal UML.Classes.Kernel.Operation getOperationByID(int operationID)
    {
        return this.factory.createElement(this.wrappedModel.GetMethodByID(operationID)) as UML.Classes.Kernel.Operation;
    }

    public void selectElement(UML.Classes.Kernel.Element element)
    {
        if (element is ElementWrapper){
            this.wrappedModel.ShowInProjectView(((ElementWrapper)element).wrappedElement);
        }
        else if (element is Operation)
        {
            this.wrappedModel.ShowInProjectView(((Operation)element).wrappedOperation);
        }
        else if (element is Attribute)
        {
            this.wrappedModel.ShowInProjectView(((Attribute)element).wrappedAttribute);
        }
        else if (element is Diagram)
        {
            this.wrappedModel.ShowInProjectView(((Diagram)element).wrappedDiagram);
        }else if (element is Parameter)
        {
        	Operation operation = (Operation)((Parameter)element).operation;
        	this.wrappedModel.ShowInProjectView(operation.wrappedOperation);
        }
    }
    internal void executeSQL(string SQLString)
    {
    	this.wrappedModel.Execute(SQLString);
    }
  }
}
