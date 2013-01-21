﻿using System;

namespace EAAddinFramework
{
	/// <summary>
	/// This abstract class can be used as a base class for creating add-ins for Enterprise Architec
	/// It contains all supported operations for EA v 8.0.864
	/// </summary>
	public abstract class EAAddinBase
	{

 	    // define menu constants
//        private string _menuHeader = "-&MenuHeader";
//        private string[] _menuOptions = {"&MenuOption1","&MenuOption2","&MenuOption3"};       
        private string _menuHeader = string.Empty;
        private string[] _menuOptions = {string.Empty};       
        
		        
        /// <summary>
        /// the name of the menu. 
        /// Set this name in the constructor of the derived add-in class
        /// </summary>
        protected virtual string menuHeader {
       	    get { return _menuHeader; }
       	    set { _menuHeader = value; }
        }
        /// <summary>
        /// the different options of the menu.
        /// Set these options in the constructor of the derived add-in class
        /// </summary>
        protected virtual string[] menuOptions{
        	get { return _menuOptions; }
        	set { _menuOptions = value; }
        }
        
        /// <summary>
        /// returns true if a project is currently opened
        /// </summary>
        /// <param name="Repository">the repository</param>
        /// <returns>true if a project is opened in EA</returns>
        protected virtual bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        #region EA Add-In Events
        
        /// <summary>
        /// EA_Connect events enable Add-Ins to identify their type and to respond to Enterprise Architect start up.
        /// This event occurs when Enterprise Architect first loads your Add-In. Enterprise Architect itself is loading at this time so that while a Repository object is supplied, there is limited information that you can extract from it.
        /// The chief uses for EA_Connect are in initializing global Add-In data and for identifying the Add-In as an MDG Add-In.
        /// Also look at EA_Disconnect.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <returns>String identifying a specialized type of Add-In: 
        /// - "MDG" : MDG Add-Ins receive MDG Events and extra menu options.
        /// - "" : None-specialized Add-In.</returns>
        public virtual string EA_Connect(EA.Repository Repository)
        {
            //No special processing required.
            return "a string";
        }

        /// <summary>
        /// The EA_GetMenuItems event enables the Add-In to provide the Enterprise Architect user interface with additional Add-In menu options in various context and main menus. When a user selects an Add-In menu option, an event is raised and passed back to the Add-In that originally defined that menu option.
        /// This event is raised just before Enterprise Architect has to show particular menu options to the user, and its use is described in the Define Menu Items topic.
        /// Also look at:
        /// - EA_MenuClick
        /// - EA_GetMenuState.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="MenuLocation">String representing the part of the user interface that brought up the menu. 
        /// Can be TreeView, MainMenu or Diagram.</param>
        /// <param name="MenuName">The name of the parent menu for which sub-items are to be defined. In the case of the top-level menu it is an empty string.</param>
        /// <returns>One of the following types:
        /// - A string indicating the label for a single menu option.
        /// - An array of strings indicating a multiple menu options.
        /// - Empty (Visual Basic/VB.NET) or null (C#) to indicate that no menu should be displayed.
        /// In the case of the top-level menu it should be a single string or an array containing only one item, or Empty/null.</returns>
        public virtual object EA_GetMenuItems(EA.Repository Repository, string MenuLocation, string MenuName)
        {
        	if (MenuName == string.Empty) {
        		//return top level menu option
        		return this.menuHeader;
        	} else if (MenuName == this.menuHeader) {
        		// return submenu options
        		return this.menuOptions;
        	} else {
        		return string.Empty;
        	}
            
        }

        /// <summary>
        /// The EA_GetMenuState event enables the Add-In to set a particular menu option to either enabled or disabled. This is useful when dealing with locked packages and other situations where it is convenient to show a menu option, but not enable it for use.
        /// This event is raised just before Enterprise Architect has to show particular menu options to the user. Its use is described in the Define Menu Items topic.
        /// Also look at EA_GetMenuItems.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="MenuLocation">String representing the part of the user interface that brought up the menu. 
        /// Can be TreeView, MainMenu or Diagram.</param>
        /// <param name="MenuName">The name of the parent menu for which sub-items must be defined. In the case of the top-level menu it is an empty string.</param>
        /// <param name="ItemName">The name of the option actually clicked, for example, Create a New Invoice.</param>
        /// <param name="IsEnabled">Boolean. Set to False to disable this particular menu option.</param>
        /// <param name="IsChecked">Boolean. Set to True to check this particular menu option.</param>
        public virtual void EA_GetMenuState(EA.Repository Repository, string MenuLocation, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(Repository))
            {
				IsEnabled = true;
            }
            else
            {
                // If no open project, disable all menu options
                IsEnabled = false;
            }
        }

        /// <summary>
        /// EA_MenuClick events are received by an Add-In in response to user selection of a menu option.
        /// The event is raised when the user clicks on a particular menu option. When a user clicks on one of your non-parent menu options, your Add-In receives a MenuClick event, defined as follows:
        /// Sub EA_MenuClick(Repository As EA.Repository, ByVal MenuName As String, ByVal ItemName As String)
        /// Notice that your code can directly access Enterprise Architect data and UI elements using Repository methods.
        /// Also look at EA_GetMenuItems.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="MenuLocation">String representing the part of the user interface that brought up the menu. 
        /// Can be TreeView, MainMenu or Diagram.</param>
        /// <param name="MenuName">The name of the parent menu for which sub-items must be defined. In the case of the top-level menu it is an empty string.</param>
        /// <param name="ItemName">The name of the option actually clicked, for example, Create a New Invoice.</param>
        public virtual void EA_MenuClick(EA.Repository Repository, string MenuLocation, string MenuName, string ItemName){}
    
        /// <summary>
        /// The EA_Disconnect event enables the Add-In to respond to user requests to disconnect the model branch from an external project.
        /// This function is called when the Enterprise Architect closes. If you have stored references to Enterprise Architect objects (not particularly recommended anyway), you must release them here.
        /// In addition, .NET users must call memory management functions as shown below:
        /// GC.Collect();
        /// GC.WaitForPendingFinalizers();
        /// Also look at EA_Connect.
        /// </summary>
        public virtual void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        /// <summary>
        /// EA_OnOutputItemClicked events inform Add-Ins that the user has clicked on a list entry in the system tab or one of the user defined output tabs.
        /// Usually an Add-In responds to this event in order to capture activity on an output tab they had previously created through a call to Repository.AddTab().
        /// Note that every loaded Add-In receives this event for every click on an output tab in Enterprise Architect - irrespective of whether the Add-In created that tab. Add-Ins should therefore check the TabName parameter supplied by this event to ensure that they are not responding to other Add-Ins' events.
        /// Also look at EA_OnOutputItemDoubleClicked.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="TabName">The name of the tab that the click occurred in. 
        /// Usually this would have been created through Repository.AddTab().</param>
        /// <param name="LineText">The text that had been supplied as the String parameter in the original call to Repository.WriteOutput().</param>
        /// <param name="ID">The ID value specified in the original call to Repository.WriteOutput().</param>
        public virtual void EA_OnOutputItemClicked(EA.Repository Repository ,string TabName , string LineText ,long ID){}
		
        /// <summary>
        /// EA_OnOutputItemDoubleClicked events informs Add-Ins that the user has used the mouse to double-click on a list entry in one of the user-defined output tabs.
        /// Usually an Add-In responds to this event in order to capture activity on an output tab they had previously created through a call to Repository.AddTab().
        /// Note that every loaded Add-In receives this event for every double-click on an output tab in Enterprise Architect - irrespective of whether the Add-In created that tab. Add-Ins should therefore check the TabName parameter supplied by this event to ensure that they are not responding to other Add-Ins' events.
        /// Also look at EA_OnOutputItemClicked.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="TabName">The name of the tab that the click occurred in. 
        /// Usually this would have been created through Repository.AddTab().</param>
        /// <param name="LineText">The text that had been supplied as the String parameter in the original call to Repository.WriteOutput().</param>
        /// <param name="ID">The ID value specified in the original call to Repository.WriteOutput().</param>
        public virtual void EA_OnOutputItemDoubleClicked(EA.Repository Repository ,string TabName , string LineText ,long ID){}
        
        /// <summary>
        /// The EA_ShowHelp event enables the Add-In to show a help topic for a particular menu option. When the user has an Add-In menu option selected, pressing [F1] can be delegated to the required Help topic by the Add-In and a suitable help message shown.
        /// This event is raised when the user presses [F1] on a menu option that is not a parent menu.
        /// Also look at EA_GetMenuItems.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="MenuLocation">String representing the part of the user interface that brought up the menu. 
        /// Can be Treeview, MainMenu or Diagram.</param>
        /// <param name="MenuName">The name of the parent menu for which sub-items are to be defined. In the case of the top-level menu it is an empty string.</param>
        /// <param name="ItemName">The name of the option actually clicked, for example, Create a New Invoice.</param>
        public virtual void EA_ShowHelp(EA.Repository Repository ,string MenuLocation ,string MenuName , string ItemName){}


		#endregion EA Add-In Events
		        
        #region EA Broadcast Events
        /// <summary>
        /// The EA_FileOpen event enables the Add-In to respond to a File Open event. When Enterprise Architect opens a new model file, this event is raised and passed to all Add-Ins implementing this method.
        /// The event occurs when the model being viewed by the Enterprise Architect user changes, for whatever reason (through user interaction or Add-In activity).
        /// Also look at EA_FileClose and EA_FileNew.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        public virtual void EA_FileOpen(EA.Repository Repository){}
        
        /// <summary>
        /// The EA_FileClose event enables the Add-In to respond to a File Close event. When Enterprise Architect closes an opened Model file, this event is raised and passed to all Add-Ins implementing this method.
        /// This event occurs when the model currently opened within Enterprise Architect is about to be closed (when another model is about to be opened or when Enterprise Architect is about to shutdown).
        /// Also look at EA_FileOpen and EA_FileNew.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        public virtual void EA_FileClose(EA.Repository Repository){}

        /// <summary>
        /// The EA_FileNew event enables the Add-In to respond to a File New event. When Enterprise Architect creates a new model file, this event is raised and passed to all Add-Ins implementing this method.
        /// The event occurs when the model being viewed by the Enterprise Architect user changes, for whatever reason (through user interaction or Add-In activity).
        /// Also look at EA_FileClose and EA_FileOpen.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        public virtual void EA_FileNew(EA.Repository Repository){}
        
        /// <summary>
        /// EA_OnPostCloseDiagram notifies Add-Ins that a diagram has been closed.
        /// Also look at EA_OnPostOpenDiagram.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="DiagramID">Contains the Diagram ID of the diagram that was closed.</param>
        public virtual void EA_OnPostCloseDiagram(EA.Repository Repository ,int DiagramID){}
		
        /// <summary>
        /// EA_OnPostOpenDiagram notifies Add-Ins that a diagram has been opened.
        /// Also look at EA_OnPostCloseDiagram.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="DiagramID">Contains the Diagram ID of the diagram that was opened.</param>
        public virtual void EA_OnPostOpenDiagram(EA.Repository Repository ,int DiagramID){}
		
		#region EA Pre-Deletion Events
		
		/// <summary>
		/// EA_OnPreDeleteElement notifies Add-Ins that an element is to be deleted from the model. It enables Add-Ins to permit or deny deletion of the element.
		/// This event occurs when a user deletes an element from the Project Browser or on a diagram. 
		/// The notification is provided immediately before the element is deleted, so that the Add-In can disable deletion of the element.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the element to be deleted:
		/// - ElementID: A long value corresponding to Element.ElementID.</param>	
		/// <returns>Return True to enable deletion of the element from the model. Return False to disable deletion of the element.</returns>
		public virtual bool EA_OnPreDeleteElement(EA.Repository Repository, EA.EventProperties Info){return true;}
		
		/// <summary>
		/// EA_OnPreDeleteAttribute notifies Add-Ins that an attribute is to be deleted from the model. It enables Add-Ins to permit or deny deletion of the attribute.
		/// This event occurs when a user deletes an attribute from the Project Browser or on a diagram. 
		/// The notification is provided immediately before the attribute is deleted, so that the Add-In can disable deletion of the attribute.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the Attribute to be deleted:
		/// - AttributeID: A long value corresponding to Attribute.AttributeID.</param>	
		/// <returns>Return True to enable deletion of the attribute from the model. Return False to disable deletion of the attribute.</returns>
		public virtual bool EA_OnPreDeleteAttribute(EA.Repository Repository, EA.EventProperties Info){return true;}
		
		/// <summary>
		/// EA_OnPreDeleteMethod notifies Add-Ins that an method is to be deleted from the model. It enables Add-Ins to permit or deny deletion of the method.
		/// This event occurs when a user deletes an method from the Project Browser or on a diagram. 
		/// The notification is provided immediately before the method is deleted, so that the Add-In can disable deletion of the method.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the Method to be deleted:
		/// - MethodID: A long value corresponding to Method.MethodID.</param>	
		/// <returns>Return True to enable deletion of the method from the model. Return False to disable deletion of the method.</returns>
		public virtual bool EA_OnPreDeleteMethod(EA.Repository Repository, EA.EventProperties Info){return true;}
		
		/// <summary>
		/// EA_OnPreDeleteConnector notifies Add-Ins that an connector is to be deleted from the model. It enables Add-Ins to permit or deny deletion of the connector.
		/// This event occurs when a user attempts to permanently delete a connector on a diagram.
		/// The notification is provided immediately before the connector is deleted, so that the Add-In can disable deletion of the connector.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the connector to be deleted:
		/// - ConnectorID: A long value corresponding to Connector.ConnectorID.</param>	
		/// <returns>Return True to enable deletion of the connector from the model. Return False to disable deletion of the connector.</returns>
		public virtual bool EA_OnPreDeleteConnector(EA.Repository Repository, EA.EventProperties Info){return true;}
		
		/// <summary>
		/// EA_OnPreDeleteDiagram notifies Add-Ins that an diagram is to be deleted from the model. It enables Add-Ins to permit or deny deletion of the diagram.
		/// This event occurs when a user attempts to permanently delete a diagram from the Project Browser.
		/// The notification is provided immediately before the diagram is deleted, so that the Add-In can disable deletion of the diagram.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the diagram to be deleted:
		/// - DiagramID: A long value corresponding to Diagram.DiagramID.</param>	
		/// <returns>Return True to enable deletion of the diagram from the model. Return False to disable deletion of the diagram.</returns>
		public virtual bool EA_OnPreDeleteDiagram(EA.Repository Repository, EA.EventProperties Info){return true;}
		
		/// <summary>
		/// EA_OnPreDeletePackage notifies Add-Ins that an package is to be deleted from the model. It enables Add-Ins to permit or deny deletion of the package.
		/// This event occurs when a user attempts to permanently delete a package from the Project Browser.
		/// The notification is provided immediately before the package is deleted, so that the Add-In can disable deletion of the package.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the package to be deleted:
		/// - PackageID: A long value corresponding to Package.PackageID.</param>	
		/// <returns>Return True to enable deletion of the package from the model. Return False to disable deletion of the package.</returns>
		public virtual bool EA_OnPreDeletePackage(EA.Repository Repository, EA.EventProperties Info){return true;}
		
		/// <summary>
		/// EA_OnPreDeleteGlossaryTerm notifies Add-Ins that a glossary term is to be deleted from the model. It enables Add-Ins to permit or deny deletion of the glossary term.
		/// The notification is provided immediately before the glossary term is deleted, so that the Add-In can disable deletion of the glossary term.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty objects for the glossary term to be deleted:
		/// TermID: A long value corresponding to Term.TermID.</param>
		/// <returns>Return True to enable deletion of the glossary term from the model. Return False to disable deletion of the glossary term.</returns>
		public virtual bool EA_OnPreDeleteGlossaryTerm (EA.Repository Repository, EA.EventProperties Info){return true;}
		
		
		#endregion EA Pre-Deletion Events
		
		#region EA Pre-New Events
		
		/// <summary>
		/// EA_OnPreNewElement notifies Add-Ins that a new element is about to be created on a diagram. It enables Add-Ins to permit or deny creation of the new element.
		/// This event occurs when a user drags a new element from the Toolbox or Resources window onto a diagram. 
		/// The notification is provided immediately before the element is created, so that the Add-In can disable addition of the element.
		/// Also look at EA_OnPostNewElement.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty objects for the element to be created:
		/// - Type: A string value corresponding to Element.Type
		/// - Stereotype: A string value corresponding to Element.Stereotype
		/// - ParentID: A long value corresponding to Element.ParentID
		/// - DiagramID: A long value corresponding to the ID of the diagram to which the element is being added. </param>
		/// <returns>Return True to enable addition of the new element to the model. Return False to disable addition of the new element.</returns>
		public virtual bool EA_OnPreNewElement(EA.Repository Repository, EA.EventProperties Info){return true;}
		
		/// <summary>
		/// EA_OnPreNewConnector notifies Add-Ins that a new connector is about to be created on a diagram. It enables Add-Ins to permit or deny creation of a new connector.
		/// This event occurs when a user drags a new connector from the Toolbox or Resources window, onto a diagram. The notification is provided immediately before the connector is created, so that the Add-In can disable addition of the connector.
		/// Also look at EA_OnPostNewConnector.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty objects for the connector to be created:
		/// - Type: A string value corresponding to Connector.Type
		/// - Subtype: A string value corresponding to Connector.Subtype
		/// - Stereotype: A string value corresponding to Connector.Stereotype
		/// - ClientID: A long value corresponding to Connector.ClientID
		/// - SupplierID: A long value corresponding to Connector.SupplierID
		/// - DiagramID: A long value corresponding to Connector.DiagramID.
		/// </param>
		/// <returns>Return True to enable addition of the new connector to the model. Return False to disable addition of the new connector.</returns>
		public virtual bool EA_OnPreNewConnector(EA.Repository Repository, EA.EventProperties Info){return true;}
		
		/// <summary>
		/// EA_OnPreNewDiagram notifies Add-Ins that a new diagram is about to be created. It enables Add-Ins to permit or deny creation of the new diagram.
		/// The notification is provided immediately before the diagram is created, so that the Add-In can disable addition of the diagram.
		/// Also look at EA_OnPostNewDiagram.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty objects for the diagram to be created:
		/// - Type: A string value corresponding to Diagram.Type
		/// - ParentID: A long value corresponding to Diagram.ParentID
		/// - PackageID: A long value corresponding to Diagram.PackageID. </param>
		/// <returns>Return True to enable addition of the new diagram to the model. Return False to disable addition of the new diagram.</returns>
		public virtual bool EA_OnPreNewDiagram(EA.Repository Repository, EA.EventProperties Info){return true;}
				
		/// <summary>
		/// EA_OnPreNewDiagramObject notifies Add-Ins that a new diagram object is about to be dropped on a diagram. It enables Add-Ins to permit or deny creation of the new object.
		/// This event occurs when a user drags an object from the Enterprise Architect Project Browser or Resources window onto a diagram. The notification is provided immediately before the object is created, so that the Add-In can disable addition of the object.
		/// Also look at EA_OnPostNewDiagramObject.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty objects for the object to be created:
		/// - Type: A string value corresponding to Object.Type
		/// - Stereotype: A string value corresponding to Object.Stereotype
		/// - ParentID: A long value corresponding to Object.ParentID
		/// - DiagramID: A long value corresponding to the ID of the diagram to which the object is being added. </param>
		/// <returns>Return True to enable addition of the object to the model. Return False to disable addition of the object.</returns>
		public virtual bool EA_OnPreNewDiagramObject(EA.Repository Repository, EA.EventProperties Info){return true;}
		
		/// <summary>
		/// EA_OnPreNewAttribute notifies Add-Ins that a new attribute is about to be created on an element. It enables Add-Ins to permit or deny creation of the new attribute.
		/// This event occurs when a user creates a new attribute on an element by either drag-dropping from the Project Browser, using the Attributes Properties dialog, or using the in-place editor on the diagram. The notification is provided immediately before the attribute is created, so that the Add-In can disable addition of the attribute.
		/// Also look at EA_OnPostNewAttribute.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty objects for the attribute to be created:
		/// - Type: A string value corresponding to Attribute.Type
		/// - Stereotype: A string value corresponding to Attribute.Stereotype
		/// - ParentID: A long value corresponding to Attribute.ParentID
		/// - ClassifierID: A long value corresponding to Attribute.ClassifierID. </param>
		/// <returns>Return True to enable addition of the new attribute to the model. Return False to disable addition of the new attribute.</returns>
		public virtual bool EA_OnPreNewAttribute(EA.Repository Repository, EA.EventProperties Info){return true;}
		
		/// <summary>
		/// EA_OnPreNewMethod notifies Add-Ins that a new method is about to be created on an element. It enables Add-Ins to permit or deny creation of the new method.
		/// This event occurs when a user creates a new method on an element by either drag-dropping from the Project Browser, using the method Properties dialog, or using the in-place editor on the diagram. The notification is provided immediately before the method is created, so that the Add-In can disable addition of the method.
		/// Also look at EA_OnPostNewMethod.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty objects for the method to be created:
		/// - ReturnType: A string value corresponding to Method.ReturnType
		/// - Stereotype: A string value corresponding to Method.Stereotype
		/// - ParentID: A long value corresponding to Method.ParentID
		/// - ClassifierID: A long value corresponding to Method.ClassifierID. </param>
		/// <returns>Return True to enable addition of the new method to the model. Return False to disable addition of the new method.</returns>
		public virtual bool EA_OnPreNewMethod(EA.Repository Repository, EA.EventProperties Info){return true;}
					
		/// <summary>
		/// EA_OnPreNewPackage notifies Add-Ins that a new package is about to be created in the model. It enables Add-Ins to permit or deny creation of the new package.
		/// This event occurs when a user drags a new package from the Toolbox or Resources window onto a diagram, or by selecting the New Package icon from the Project Browser. The notification is provided immediately before the package is created, so that the Add-In can disable addition of the package.
		/// Also look at EA_OnPostNewPackage.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty objects for the package to be created:
		/// Stereotype: A string value corresponding to Package.Stereotype
		/// ParentID: A long value corresponding to Package.ParentID
		/// DiagramID: A long value corresponding to the ID of the diagram to which the package is being added. </param>
		/// <returns>Return True to enable addition of the new package to the model. Return False to disable addition of the new package.</returns>
		public virtual bool EA_OnPreNewPackage(EA.Repository Repository, EA.EventProperties Info){return true;}
		
		/// <summary>
		/// EA_OnPreNewGlossaryTerm notifies Add-Ins that a new glossary term is about to be created. It enables Add-Ins to permit or deny creation of the new glossary term.
		/// The notification is provided immediately before the glossary term is created, so that the Add-In can disable addition of the element.
		/// Also look at EA_OnPostNewGlossaryTerm.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty objects for the glossary term to be deleted:
		/// TermID: A long value corresponding to Term.TermID.</param>
		/// <returns>Return True to enable addition of the new glossary term to the model. Return False to disable addition of the new glossary term.</returns>
		public virtual bool EA_OnPreNewGlossaryTerm (EA.Repository Repository, EA.EventProperties Info){return true;}
				
		#endregion EA Pre-New Events
		
		/// <summary>
		/// EA_OnPreExitInstance is not currently used.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		public virtual void EA_OnPreExitInstance(EA.Repository Repository){}
		
		#region EA Post-New Events
		
		/// <summary>
		/// EA_OnPostNewElement notifies Add-Ins that a new element has been created on a diagram. It enables Add-Ins to modify the element upon creation.
		/// This event occurs after a user has dragged a new element from the Toolbox or Resources window onto a diagram. The notification is provided immediately after the element is added to the model. Set Repository.SuppressEADialogs to true to suppress Enterprise Architect from showing its default dialogs.
		/// Also look at EA_OnPreNewElement.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the new element:
		/// - ElementID: A long value corresponding to Element.ElementID. </param>
		/// <returns>Return True if the element has been updated during this notification. Return False otherwise.</returns>
		public virtual bool EA_OnPostNewElement(EA.Repository Repository, EA.EventProperties Info){return false;}
		
		/// <summary>
		/// EA_OnPostNewConnector notifies Add-Ins that a new connector has been created on a diagram. It enables Add-Ins to modify the connector upon creation.
		/// This event occurs after a user has dragged a new connector from the Toolbox or Resources window onto a diagram. The notification is provided immediately after the connector is added to the model. Set Repository.SuppressEADialogs to true to suppress Enterprise Architect from showing its default dialogs.
		/// Also look at EA_OnPreNewConnector.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the new connector:
		/// - ConnectorID: A long value corresponding to Connector.ConnectorID.
		/// </param>
		/// <returns>Return True if the connector has been updated during this notification. Return False otherwise.</returns>
		public virtual bool EA_OnPostNewConnector(EA.Repository Repository, EA.EventProperties Info){return false;}
		
		/// <summary>
		/// EA_OnPostNewDiagram notifies Add-Ins that a new diagram has been created. It enables Add-Ins to modify the diagram upon creation.
		/// Set Repository.SuppressEADialogs to true to suppress Enterprise Architect from showing its default dialogs.
		/// Also look at EA_OnPreNewDiagram.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the new diagram:
		/// - DiagramID: A long value corresponding to Diagram.DiagramID.</param>
		/// <returns>Return True if the diagram has been updated during this notification. Return False otherwise.</returns>
		public virtual bool EA_OnPostNewDiagram(EA.Repository Repository, EA.EventProperties Info){return false;}
				
		/// <summary>
		/// EA_OnPostNewDiagramObject notifies Add-Ins that a new object has been created on a diagram. It enables Add-Ins to modify the object upon creation.
		/// This event occurs after a user has dragged a new object from the Project Browser or Resources window onto a diagram. The notification is provided immediately after the object is added to the diagram. Set Repository.SuppressEADialogs to true to suppress Enterprise Architect from showing its default dialogs.
		/// Also look at EA_OnPreNewDiagramObject.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the new element:
		/// - ObjectID: A long value corresponding to Object.ObjectID.</param>
		/// <returns>Return True if the element has been updated during this notification. Return False otherwise.</returns>
		public virtual bool EA_OnPostNewDiagramObject(EA.Repository Repository, EA.EventProperties Info){return false;}
		
		/// <summary>
		/// EA_OnPostNewAttribute notifies Add-Ins that a new attribute has been created on a diagram. It enables Add-Ins to modify the attribute upon creation.
		/// This event occurs when a user creates a new attribute on an element by either drag-dropping from the Project Browser, using the Attributes Properties dialog, or using the in-place editor on the diagram. The notification is provided immediately after the attribute is created. Set Repository.SuppressEADialogs to true to suppress Enterprise Architect from showing its default dialogs.
		/// Also look at EA_OnPreNewAttribute.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the new attribute:
		/// - AttributeID: A long value corresponding to Attribute.AttributeID.</param>
		/// <returns>Return True if the attribute has been updated during this notification. Return False otherwise.</returns>
		public virtual bool EA_OnPostNewAttribute(EA.Repository Repository, EA.EventProperties Info){return false;}
		
		/// <summary>
		/// EA_OnPostNewMethod notifies Add-Ins that a new method has been created on a diagram. It enables Add-Ins to modify the method upon creation.
		/// This event occurs when a user creates a new method on an element by either drag-dropping from the Project Browser, using the method's Properties dialog, or using the in-place editor on the diagram. The notification is provided immediately after the method is created. Set Repository.SuppressEADialogs to true to suppress Enterprise Architect from showing its default dialogs.
		/// Also look at EA_OnPreNewMethod.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the new method:
		/// - MethodID: A long value corresponding to Method.MethodID.</param>
		/// <returns>Return True if the method has been updated during this notification. Return False otherwise.</returns>
		public virtual bool EA_OnPostNewMethod(EA.Repository Repository, EA.EventProperties Info){return false;}
					
		/// <summary>
		/// EA_OnPostNewPackage notifies Add-Ins that a new package has been created on a diagram. It enables Add-Ins to modify the package upon creation.
		/// This event occurs when a user drags a new package from the Toolbox or Resources window onto a diagram, or by selecting the New Package icon from the Project Browser. Set Repository.SuppressEADialogs to true to suppress Enterprise Architect from showing its default dialogs.
		/// Also look at EA_OnPreNewPackage.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the new package:
		/// - PackageID: A long value corresponding to Package.PackageID.</param>
		/// <returns>Return True if the package has been updated during this notification. Return False otherwise.</returns>
		public virtual bool EA_OnPostNewPackage(EA.Repository Repository, EA.EventProperties Info){return false;}
		
		/// <summary>
		/// EA_OnPostNewGlossaryTerm notifies Add-Ins that a new glossary term has been created. It enables Add-Ins to modify the glossary term upon creation.
		/// The notification is provided immediately after the glossary term is added to the model. Set Repository.SuppressEADialogs to true to suppress Enterprise Architect from showing its default dialogs.
		/// Also look at EA_OnPreNewGlossaryTerm.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty objects for the glossary term to be deleted:
		/// TermID: A long value corresponding to Term.TermID.</param>
		/// <returns>Return True if the glossary term has been updated during this notification. Return False otherwise.</returns>
		public virtual bool EA_OnPostNewGlossaryTerm (EA.Repository Repository, EA.EventProperties Info){return false;}
		
		#endregion EA Post-New Events
		/// <summary>
		/// EA_OnPostInitialized notifies Add-Ins that the Repository object has finished loading and any necessary initialization steps can now be performed on the object.
		/// For example, the Add-In can create an Output tab using Repository.CreateOutputTab.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		public virtual void EA_OnPostInitialized(EA.Repository Repository){}
		
		/// <summary>
		/// EA_OnPostTransform notifies Add-Ins that an MDG transformation has taken place with the output in the specified target package.
		/// This event occurs when a user runs an MDG transform on one or more target packages. The notification is provided for each transform/target package immediately after all transform processes have completed.
		/// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty objects for the transform performed:
		/// - Transform: A string value corresponding to the name of the transform used
		/// - PackageID: A long value corresponding to Package.PackageID of the destination package. </param>
		/// <returns>Reserved for future use.</returns>
        public virtual bool EA_OnPostTransform(EA.Repository Repository, EA.EventProperties Info){return true;}
		
        #region EA Technology Events
        
        /// <summary>
        /// EA_OnInitializeTechnologies requests that an Add-In pass an MDG Technology to Enterprise Architect for loading.
        /// This event occurs on Enterprise Architect startup. Return your technology XML to this function and Enterprise Architect loads and enables it.
        /// </summary>
        /// <example>
        /// public object EA_OnInitializeTechnologies(EA.Repository Repository){
        /// 	return My.Resources.MyTechnology;}
        /// </example>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <returns>Return the MDG Technology as a single XML string.</returns>
        public virtual object EA_OnInitializeTechnologies(EA.Repository Repository){return null;}
        
        /// <summary>
        /// EA_OnPreActivateTechnology notifies Add-Ins that an MDG Technology resource is about to be activated in the model. This event occurs when a user selects to activate an MDG Technology resource in the model (by clicking on the Set Active button on the MDG Technologies dialog or by selecting the technology in the list box in the Default Tools toolbar).
        /// The notification is provided immediately after the user attempts to activate the MDG Technology, so that the Add-In can permit or disable activation of the Technology.
        /// Also look at EA_OnPostActivateTechnology.
        /// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the MDG Technology to be activated:
		/// - TechnologyID: A string value corresponding to the MDG Technology ID.</param>
		/// <returns>Return True to enable activation of the MDG Technology resource in the model. Return False to disable activation of the MDG Technology resource.</returns>
        public virtual bool EA_OnPreActivateTechnology(EA.Repository Repository, EA.EventProperties Info){return true;}
        
        /// <summary>
        /// EA_OnPostActivateTechnology notifies Add-Ins that an MDG Technology resource has been activated in the model. This event occurs when a user activates an MDG Technology resource in the model (by clicking on the Set Active button on the MDG Technologies dialog or by selecting the technology in the list box in the Default Tools toolbar). The notification is provided immediately after the user succeeds in activating the MDG Technology, so that the Add-In can update the Technology if necessary.
        /// Also look at EA_OnPreActivateTechnology.
        /// </summary>
		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="Info">Contains the following EventProperty object for the MDG Technology to be activated:
		/// - TechnologyID: A string value corresponding to the MDG Technology ID.</param>
		/// <returns>Return True if the MDG Technology resource is updated during this notification. Return False otherwise.</returns>
        public virtual bool EA_OnPostActivateTechnology(EA.Repository Repository, EA.EventProperties Info){return true;}
                
        #endregion EA Technology Events
        
        #region EA Context Item Events
        
        /// <summary>
        /// EA_OnContextItemChanged notifies Add-Ins that a different item is now in context.
        /// This event occurs after a user has selected an item anywhere in the Enterprise Architect GUI. Add-Ins that require knowledge of the current item in context can subscribe to this broadcast function. If ot = otRepository, then this function behaves the same as EA_FileOpen.
        /// Also look at EA_OnContextItemDoubleClicked and EA_OnNotifyContextItemModified.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="GUID">Contains the GUID of the new context item. 
        /// This value corresponds to the following properties, depending on the value of the ot parameter:
        /// ot (ObjectType)	- GUID value
        /// otElement  		- Element.ElementGUID
        /// otPackage 		- Package.PackageGUID
        /// otDiagram		- Diagram.DiagramGUID
        /// otAttribute		- Attribute.AttributeGUID
        /// otMethod		- Method.MethodGUID
        /// otConnector		- Connector.ConnectorGUID
        /// otRepository	- NOT APPLICABLE, GUID is an empty string
        /// </param>
        /// <param name="ot">Specifies the type of the new context item.</param>
        public virtual void EA_OnContextItemChanged(EA.Repository Repository,string GUID, EA.ObjectType ot){}
        
        /// <summary>
        /// EA_OnContextItemDoubleClicked notifies Add-Ins that the user has double-clicked the item currently in context.
        /// This event occurs when a user has double-clicked (or pressed [Enter]) on the item in context, either in a diagram or in the Project Browser. Add-Ins to handle events can subscribe to this broadcast function.
        /// Also look at EA_OnContextItemChanged and EA_OnNotifyContextItemModified.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="GUID">Contains the GUID of the new context item. 
        /// This value corresponds to the following properties, depending on the value of the ot parameter:
        /// ot (ObjectType)	- GUID value
        /// otElement  		- Element.ElementGUID
        /// otPackage 		- Package.PackageGUID
        /// otDiagram		- Diagram.DiagramGUID
        /// otAttribute		- Attribute.AttributeGUID
        /// otMethod		- Method.MethodGUID
        /// otConnector		- Connector.ConnectorGUID
        /// </param>
        /// <param name="ot">Specifies the type of the new context item.</param>
        /// <returns>Return True to notify Enterprise Architect that the double-click event has been handled by an Add-In. Return False to enable Enterprise Architect to continue processing the event.</returns>
        public virtual bool EA_OnContextItemDoubleClicked(EA.Repository Repository,string GUID, EA.ObjectType ot){return false;}
        
        /// <summary>
        /// EA_OnNotifyContextItemModified notifies Add-Ins that the current context item has been modified.
        /// This event occurs when a user has modified the context item. Add-Ins that require knowledge of when an item has been modified can subscribe to this broadcast function.
        /// Also look at EA_OnContextItemChanged and EA_OnContextItemDoubleClicked.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="GUID">Contains the GUID of the new context item. 
        /// This value corresponds to the following properties, depending on the value of the ot parameter:
        /// ot (ObjectType)	- GUID value
        /// otElement  		- Element.ElementGUID
        /// otPackage 		- Package.PackageGUID
        /// otDiagram		- Diagram.DiagramGUID
        /// otAttribute		- Attribute.AttributeGUID
        /// otMethod		- Method.MethodGUID
        /// otConnector		- Connector.ConnectorGUID
        /// </param>
        /// <param name="ot">Specifies the type of the new context item.</param>
        public virtual void EA_OnNotifyContextItemModified(EA.Repository Repository,string GUID, EA.ObjectType ot){}                                                
        
        #endregion EA Context Item Events
        
        #region EA Compartment Events
        
        /// <summary>
        /// This event occurs when Enterprise Architect's diagrams are refreshed. It is a request for the Add-In to provide a list of user-defined compartments. The EA_GetCompartmentData event then queries each object for the data to display in each user-defined compartment.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <returns>A String containing a comma-separated list of user-defined compartments.</returns>
         public virtual object EA_QueryAvailableCompartments(EA.Repository Repository){return null;}
         
        /// <summary>
        /// This event occurs when Enterprise Architect is instructed to redraw an element. It requests that the Add-In provide the data to populate the element's compartment.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="sCompartment">The name of the compartment for which data is being requested.</param>
        /// <param name="sGUID">The GUID of the element for which data is being requested.</param>
        /// <param name="oType">The type of the element for which data is being requested.</param>
        /// <returns>Variant containing a formatted string. See the example in the EA Help file to understand the format.</returns>
        public virtual object EA_GetCompartmentData(EA.Repository Repository ,string sCompartment ,string sGUID , EA.ObjectType oType ) {return null;}
        
        #endregion EA Compartment Events
        
        #region EA Model Validation Broadcasts
        
        /// <summary>
        /// EA_OnInitializeUserRules is called on Enterprise Architect start-up and requests that the Add-In provide Enterprise Architect with a rule category and list of rule IDs for model validation.
        /// This function must be implemented by any Add-In that is to perform its own model validation. It must call Project.DefineRuleCategory once and Project.DefineRule for each rule; these functions are described in the Project Interface section.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        public virtual void EA_OnInitializeUserRules(EA.Repository Repository){}
        
        /// <summary>
        /// EA_OnStartValidation notifies Add-Ins that a user has invoked the model validation command from Enterprise Architect.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="Args">Contains a (Variant) list of Rule Categories that are active for the current invocation of model validation.</param>
        public virtual void EA_OnStartValidation(EA.Repository Repository,object Args){}
        
        /// <summary>
        /// EA_OnEndValidation notifies Add-Ins that model validation has completed. Use this event to arrange any clean-up operations arising from the validation.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="Args">Contains a (Variant) list of Rule Categories that were active for the invocation of model validation that has just completed.</param>
        public virtual void EA_OnEndValidation(EA.Repository Repository, object Args){}
        
        /// <summary>
        /// This event is triggered once for each rule defined in EA_OnInitializeUserRules to be performed on each element in the selection being validated. If you don't want to perform the rule defined by RuleID on the given element, then simply return without performing any action. On performing any validation, if a validation error is found, use the Repository.ProjectInterface.PublishResult method to notify Enterprise Architect.
        /// Also look at EA_OnInitializeUserRules.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="RuleID">The ID that was passed into the Project.DefineRule command.</param>
        /// <param name="Element">The element to potentially perform validation on.</param>
        public virtual void EA_OnRunElementRule(EA.Repository Repository,string RuleID, EA.Element Element){}
        
        /// <summary>
        /// This event is triggered once for each rule defined in EA_OnInitializeUserRules to be performed on each package in the selection being validated.
		/// If you don't want to perform the rule defined by RuleID on the given package, then simply return without performing any action. 
		/// On performing any validation, if a validation error is found, use the Repository.ProjectInterface.PublishResult method to notify Enterprise Architect.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="RuleID">The ID that was passed into the Project.DefineRule command.</param>
        /// <param name="PackageID">The ID of the package to potentially perform validation on. Use the Repository.GetPackageByID method to retrieve the package object.</param>
        public virtual void EA_OnRunPackageRule(EA.Repository Repository,string RuleID, long PackageID){}
        
        /// <summary>
        /// This event is triggered once for each rule defined in EA_OnInitializeUserRules to be performed on each diagram in the selection being validated.
		/// If you don't want to perform the rule defined by RuleID on the given diagram, then simply return without performing any action. 
		/// On performing any validation, if a validation error is found, use the Repository.ProjectInterface.PublishResult method to notify Enterprise Architect.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="RuleID">The ID that was passed into the Project.DefineRule command.</param>
        /// <param name="DiagramID">The ID of the diagram to potentially perform validation on. Use the Repository.GetDiagramByID method to retrieve the diagram object.</param>
        public virtual void EA_OnRunDiagramRule(EA.Repository Repository,string RuleID, long DiagramID){}
        
        /// <summary>
        /// This event is triggered once for each rule defined in EA_OnInitializeUserRules to be performed on each connector in the selection being validated.
		/// If you don't want to perform the rule defined by RuleID on the given connector, then simply return without performing any action. 
		/// On performing any validation, if a validation error is found, use the Repository.ProjectInterface.PublishResult method to notify Enterprise Architect.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="RuleID">The ID that was passed into the Project.DefineRule command.</param>
        /// <param name="ConnectorID">The ID of the connector to potentially perform validation on. Use the Repository.GetConnectorByID method to retrieve the connector object.</param>
        public virtual void EA_OnRunConnectorRule(EA.Repository Repository,string RuleID, long ConnectorID){}
        
        /// <summary>
        /// This event is triggered once for each rule defined in EA_OnInitializeUserRules to be performed on each attribute in the selection being validated.
		/// If you don't want to perform the rule defined by RuleID on the given attribute, then simply return without performing any action. 
		/// On performing any validation, if a validation error is found, use the Repository.ProjectInterface.PublishResult method to notify Enterprise Architect.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="RuleID">The ID that was passed into the Project.DefineRule command.</param>
        /// <param name="AttributeGUID">The GUID of the attribute to potentially perform validation on. Use the Repository.GetAttributeByGUID method to retrieve the attribute object.</param>
        /// <param name="ObjectID">The ID of the object that owns the given attribute. Use the Repository.GetObjectByID method to retrieve the object.</param>
        public virtual void EA_OnRunAttributeRule(EA.Repository Repository,string RuleID, string AttributeGUID,long ObjectID){}
        
        /// <summary>
        /// This event is triggered once for each rule defined in EA_OnInitializeUserRules to be performed on each method in the selection being validated.
		/// If you don't want to perform the rule defined by RuleID on the given method, then simply return without performing any action. 
		/// On performing any validation, if a validation error is found, use the Repository.ProjectInterface.PublishResult method to notify Enterprise Architect.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="RuleID">The ID that was passed into the Project.DefineRule command.</param>
        /// <param name="MethodGUID">The GUID of the method to potentially perform validation on. Use the Repository.GetMethodByGUID method to retrieve the method object.</param>
        /// <param name="ObjectID">The ID of the object that owns the given method. Use the Repository.GetObjectByID method to retrieve the object.</param>
        public virtual void EA_OnRunMethodRule(EA.Repository Repository,string RuleID, string MethodGUID,long ObjectID){}
        
        /// <summary>
        /// This event is triggered once for each rule defined in EA_OnInitializeUserRules to be performed on each parameter in the selection being validated.
		/// If you don't want to perform the rule defined by RuleID on the given parameter, then simply return without performing any action. 
		/// On performing any validation, if a validation error is found, use the Repository.ProjectInterface.PublishResult parameter to notify Enterprise Architect.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="RuleID">The ID that was passed into the Project.DefineRule command.</param>
        /// <param name="ParameterGUID">The GUID of the parameter to potentially perform validation on. Use the Repository.GetParameterByGUID parameter to retrieve the parameter object.</param>
        /// <param name="MethodGUID">The GUID of the method that owns the given parameter. Use the Repository.GetMethodByGuid method to retrieve the method object.</param> 
        /// <param name="ObjectID">The ID of the object that owns the given parameter. Use the Repository.GetObjectByID parameter to retrieve the object.</param>
        public virtual void EA_OnRunParameterRule(EA.Repository Repository,string RuleID, string ParameterGUID,string MethodGUID, long ObjectID){}
                
        #endregion EA Model Validation Broadcasts
        
        #region EA Tagged Value Broadcasts
        /// <summary>
        /// EA_OnAttributeTagEdit is called when the user clicks the ellipsis ( ... ) button 
        /// for a Tagged Value of type AddinBroadcast on an attribute.
        /// The Add-In displays fields to show and change the value and notes; this function 
        /// provides the initial values for the Tagged Value notes and value, and takes on any 
        /// changes on exit of the function.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open 
        /// Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="AttributeID">The ID of the attribute that this Tagged Value is on</param>
        /// <param name="TagName">The name of the Tagged Value to edit.</param>
        /// <param name="TagValue">The current value of the tag; if the value is updated, 
        /// the new value is stored in the repository on exit of the function.</param>
        /// <param name="TagNotes">The current value of the Tagged Value notes; if the value 
        /// is updated, the new value is stored in the repository on exit of the function.</param>
        public virtual void EA_OnAttributeTagEdit(EA.Repository Repository, long AttributeID, ref string TagName, ref string TagValue, ref string TagNotes) {}
		
        /// <summary>
        /// EA_OnConnectorTagEdit is called when the user clicks the ellipsis ( ... ) button 
        /// for a Tagged Value of type AddinBroadcast on a connector.The Add-In displays fields
		/// to show and change the value and notes; this function provides the initial values 
		/// for the Tagged Value notes and value, and takes on any changes on exit of the function
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open 
        /// Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="ConnectorID">The ID of the connector that this Tagged Value is on.</param>
        /// <param name="TagName">The name of the Tagged Value to edit.</param>
        /// <param name="TagValue">The current value of the tag; if the value is updated, 
        /// the new value is stored in the repository on exit of the function.</param>
        /// <param name="TagNotes">The current value of the Tagged Value notes; if the value 
        public virtual void EA_OnConnectorTagEdit(EA.Repository Repository, long ConnectorID, ref string TagName, ref string TagValue, ref string TagNotes) {}

        /// <summary>
        /// EA_OnElementTagEdit is called when the user clicks the ellipsis ( ... ) button 
        /// for a Tagged Value of type AddinBroadcast on a element.The Add-In displays fields
		/// to show and change the value and notes; this function provides the initial values 
		/// for the Tagged Value notes and value, and takes on any changes on exit of the function
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open 
        /// Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="ObjectID">The ID of the object (element) that this Tagged Value is on.</param>
        /// <param name="TagName">The name of the Tagged Value to edit.</param>
        /// <param name="TagValue">The current value of the tag; if the value is updated, 
        /// the new value is stored in the repository on exit of the function.</param>
        /// <param name="TagNotes">The current value of the Tagged Value notes; if the value 
        public virtual void EA_OnElementTagEdit(EA.Repository Repository, long ObjectID, ref string TagName, ref string TagValue, ref string TagNotes) {}

        /// <summary>
        /// EA_OnMethodTagEdit is called when the user clicks the ellipsis ( ... ) button 
        /// for a Tagged Value of type AddinBroadcast on a method.The Add-In displays fields
		/// to show and change the value and notes; this function provides the initial values 
		/// for the Tagged Value notes and value, and takes on any changes on exit of the function
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open 
        /// Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="MethodID">The ID of the method that this Tagged Value is on.</param>
        /// <param name="TagName">The name of the Tagged Value to edit.</param>
        /// <param name="TagValue">The current value of the tag; if the value is updated, 
        /// the new value is stored in the repository on exit of the function.</param>
        /// <param name="TagNotes">The current value of the Tagged Value notes; if the value 
        public virtual void EA_OnMethodTagEdit(EA.Repository Repository, long MethodID, ref string TagName, ref string TagValue, ref string TagNotes) {}
        #endregion
        
        /// <summary>
        /// EA_OnRetrieveModelTemplate requests that an Add-In pass a model template to Enterprise Architect.
        /// This event occurs when a user executes the Add a New Model Using Wizard command to add a model that has been defined by an MDG Technology. See the Incorporate Model Templates topic for details of how to define such model templates.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <param name="sLocation">The name of the template requested. This should match the location attribute in the [ModelTemplates] section of an MDG Technology File. For more information, see the Incorporate Model Templates in a Technology topic.</param>	
        /// <returns>Return a string containing the XMI export of the model that is being used as a template.</returns>
        public virtual string EA_OnRetrieveModelTemplate(EA.Repository Repository,string sLocation){return string.Empty;}
        
		#endregion EA Broadcast Events   
		
		#region EA MDG Events
		
		/// <summary>
		/// MDG_BuildProject enables the Add-In to handle file changes caused by generation. This function is called in response to a user selecting the Add-Ins | Build Project menu option.
		/// Respond to this event by compiling the project source files into a running application.
		/// Also look at MDG_RunExe.
		/// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		public virtual void MDG_BuildProject(EA.Repository Repository, string PackageGuid){}
		
		/// <summary>
		/// MDG_Connect enables the Add-In to handle user driven request to connect a model branch to an external application. This function is called when the user attempts to connect a particular Enterprise Architect package to an as yet unspecified external project. This event enables the Add-In to interact with the user to specify such a project.
		/// The Add-In is responsible for retaining the connection details, which should be stored on a per-user or per-workstation basis. That is, users who share a common Enterprise Architect model over a network should be able to connect and disconnect to external projects independently of one another.
		/// The Add-In should therefore not store connection details in an Enterprise Architect repository. A suitable place to store such details would be:
		/// SHGetFolderPath(..CSIDL_APPDATA..)\AddinName.
		/// The PackageGuid parameter is the same identifier as required for most events relating to the MDG Add-In. Therefore it is recommended that the connection details be indexed using the PackageGuid value.
		/// The PackageID parameter is provided to aid fast retrieval of package details from Enterprise Architect, should this be required.
		/// Also look at MDG_Disconnect.
		/// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageID">The PackageID of the Enterprise Architect package the user has requested to have connected to an external project.</param>
		/// <param name="PackageGuid">The unique ID identifying the project provided by the Add-In when a connection to a project branch of an Enterprise Architect model was first established.</param>
		/// <returns>Returns a non-zero to indicate that a connection has been made; a zero indicates that the user has not nominated a project and connection should not proceed.</returns>
		public virtual long MDG_Connect(EA.Repository Repository,long PackageID,string PackageGuid){return 0;}
		
		/// <summary>
		/// MDG_Disconnect enables the Add-In to respond to user requests to disconnect the model branch from an external project. This function is called when the user attempts to disconnect an associated external project. The Add-In is required to delete the details of the connection.
		/// Also look at MDG_Connect.
		/// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		/// <returns>Returns a non-zero to indicate that a disconnection has occurred enabling Enterprise Architect to update the user interface. A zero indicates that the user has not disconnected from an external project.</returns>
		public virtual long MDG_Disconnect(EA.Repository Repository ,string PackageGuid ){return 0;}
		
		/// <summary>
		/// MDG_GetConnectedPackages enables the Add-In to return a list of current connection between Enterprise Architect and an external application. This function is called when the Add-In is first loaded, and is expected to return a list of the available connections to external projects for this Add-In.
		/// Also look at MDG_Connect
		/// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <returns>Returns an array of GUID strings representing individual Enterprise Architect packages.</returns>
		public virtual object MDG_GetConnectedPackages(EA.Repository Repository){return null;}
		
		/// <summary>
		/// MDG_GetProperty provides miscellaneous Add-In details to Enterprise Architect. This function is called by Enterprise Architect to poll the Add-In for information relating to the PropertyName. This event should occur in as short a duration as possible as Enterprise Architect does not cache the information provided by the function.
		/// Values corresponding to the following PropertyNames must be provided:
		/// - IconID - Return the name of a DLL and a resource identifier in the format #ResID, where the resource ID indicates an Icon; for example, c:\program files\myapp\myapp.dlll#101
		/// - Language - Return the default language that Classes should be assigned when they are created in Enterprise Architect
		/// - HiddenMenus - Return one or more values from the MDGMenus enumeration to hide menus that do not apply to your Add-In. For example:
		/// if( PropertyName == "HiddenMenus" )
		/// return mgBuildProject + mgRun;
		/// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		/// <param name="PropertyName">The name of the property that is used by Enterprise Architect. See Details for the possible values.</param>
		/// <returns>see summary above</returns>
		public virtual object MDG_GetProperty(EA.Repository Repository,string PackageGuid,string PropertyName){return null;}
		
		/// <summary>
		/// MDG_Merge enables the Add-In to jointly handle changes to both the model branch and the code project that the model branch is connected to. This event should be called whenever the user has asked to merge their model branch with its connected code project, or whenever the user has established a new connection to a code project. The purpose of this event is to enable the Add-In to interact with the user to perform a merge between the model branch and the connected project.
		/// Also look at MDG_Connect, MDG_PreMerge and MDG_PostMerge.
		/// Merge
		/// A merge consists of three major operations:
		/// - Export: Where newly created model objects are exported into code and made available to the code project.
		/// - Import: Where newly created code objects, Classes and such things are imported into the model.
		/// - Synchronize: Where objects available both to the model and in code are jointly updated to reflect changes made in either the model, code project or both.
		/// - Synchronize Type
		/// The Synchronize operation can take place in one of four different ways. Each of these ways corresponds to a value returned by SynchType:
		/// - None: (SynchType = 0) No synchronization is to be performed
		/// - Forward: (SynchType = 1) Forward synchronization, between the model branch and the code project is to occur
		/// - Reverse: (SynchType = 2) Reverse synchronization, between the code project and the model branch is to occur
		/// - Both: (SynchType = 3) Reverse, then Forward synchronization's are to occur.
		/// Object ID Format
		/// Each of the Object IDs listed in the string arrays described above should be composed in the following format:
		/// (@namespace)*(#class)*($attribute|%operation|:property)*
		/// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		/// <param name="SynchObjects">A string array containing a list of objects (Object ID format) to be jointly synchronized between the model branch and the project. 
		/// See summary for the format of the Object IDs.</param>
		/// <param name="SynchType">The value determining the user-selected type of synchronization to take place. 
		/// See summary for a list of valid values.</param>
		/// <param name="ExportObjects">The string array containing the list of new model objects (in Object ID format) to be exported by Enterprise Architect to the code project.</param>
		/// <param name="ExportFiles">A string array containing the list of files for each model object chosen for export by the Add-In. 
		/// Each entry in this array must have a corresponding entry in the ExportObjects parameter at the same array index, 
		/// so ExportFiles(2) must contain the filename of the object by ExportObjects(2).</param>
		/// <param name="ImportFiles">A string array containing the list of code files made available to the code project to be newly imported to the model. 
		/// Enterprise Architect imports each file listed in this array for import into the connected model branch.</param>
		/// <param name="IgnoreLocked">A value indicating whether to ignore any files locked by the code project (that is, "TRUE" or "FALSE".</param>
		/// <param name="Language">The string value containing the name of the code language supported by the code project connected to the model branch.</param>
		/// <returns>Return a non-zero if the merge operation completed successfully and a zero value when the operation has been unsuccessful.</returns>
		public virtual long MDG_Merge(EA.Repository Repository,string PackageGuid ,ref object SynchObjects,
		                              ref string SynchType,ref object ExportObjects ,ref object ExportFiles , 
		                              ref object ImportFiles ,ref string IgnoreLocked ,ref string Language ){return 0;}
		
		/// <summary>
		/// MDG_NewClass enables the Add-In to alter details of a Class before it is created.
		/// This method is called when Enterprise Architect generates a new Class, and requires information relating to assigning the language and file path. The file path should be passed back as a return value and the language should be passed back via the language parameter.
		/// Also look at MDG_PreGenerate
		/// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		/// <param name="CodeID">A string used to identify the code element before it is created, for more information see MDG_View.</param>
		/// <param name="Language">A string used to identify the programming language for the new Class. The language must be supported by Enterprise Architect.</param>
		/// <returns>Returns a string containing the file path that should be assigned to the Class.</returns>
		public virtual string MDG_NewClass(EA.Repository Repository,string PackageGuid,string CodeID,ref string Language ){return string.Empty;}
		
		/// <summary>
		/// MDG_PostGenerate enables the Add-In to handle file changes caused by generation.
		/// This event is called after Enterprise Architect has prepared text to replace the existing contents of a file. Responding to this event enables the Add-In to write to the linked application's user interface rather than modify the file directly.
		/// When the contents of a file are changed, Enterprise Architect passes FileContents as a non-empty string. New files created as a result of code generation are also sent through this mechanism, enabling Add-Ins to add new files to the linked project's file list.
		/// When new files are created Enterprise Architect passes FileContents as an empty string. When a non-zero is returned by this function, the Add-In has successfully written the contents of the file. A zero value for the return indicates to Enterprise Architect that the file must be saved.
		/// Also look at MDG_PreGenerate.
		/// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		/// <param name="FilePath">The path of the file Enterprise Architect intends to overwrite.</param>
		/// <param name="FileContents">A string containing the proposed contents of the file.</param>
		/// <returns>Return value depends on the type of event that this function is responding to (see summary). 
		/// This function is required to handle two separate and distinct cases.</returns>
		public virtual long MDG_PostGenerate(EA.Repository Repository,string PackageGuid,string FilePath,string FileContents ){return 0;}
		
		/// <summary>
		/// MDG_PostMerge is called after a merge process has been completed.
		/// This function is called by Enterprise Architect after the merge process has been completed.
		/// Note:
		/// File save checking should not be performed with this function, but should be handled by MDG_PreGenerate, MDG_PostGenerate and MDG_PreReverse.
		/// Also look at MDG_PreMerge and MDG_Merge.
		/// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		/// <returns>Return a zero value if the post-merge process has failed, a non-zero return indicates that the post-merge has been successful. Enterprise Architect assumes a non-zero return if this method is not implemented</returns>
		public virtual long MDG_PostMerge(EA.Repository Repository,string PackageGuid){return 1;}
		
		/// <summary>
		/// MDG_PreGenerate enables the Add-In to deal with unsaved changes. 
		/// This function is called immediately before Enterprise Architect attempts to generate files from the model. 
		/// A possible use of this function would be to prompt the user to save unsaved source files.
		/// Also look at MDG_PostGenerate.
		/// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		/// <returns>Return a zero value to abort generation. Any other value enables the generation to continue.</returns>
		public virtual long MDG_PreGenerate(EA.Repository Repository,string PackageGuid){return 1;}
		
		/// <summary>
		/// MDG_PreMerge is called after a merge process has been initiated by the user and before Enterprise Architect performs the merge process.
		/// This event is called after a user has performed their interactions with the merge screen and has confirmed the merge with the OK button, but before Enterprise Architect performs the merge process using the data provided by the MDG_Merge call, before any changes have been made to the model or the connected project.
		/// This event is made available to provide the Add-In with the opportunity to generally set internal Add-In flags to augment the MDG_PreGenerate, MDG_PostGenerate and MDG_PreReverse events.
		/// Note:
		/// File save checking should not be performed with this function, but should be handled by MDG_PreGenerate, MDG_PostGenerate and MDG_PreReverse.
		/// Also look at MDG_Merge and MDG_PostMerge.
		/// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		/// <returns>A return value of zero indicates that the merge process will not occur. If the value is not zero the merge process will proceed. If this method is not implemented then it is assumed that a merge process is used.</returns>
		public virtual long MDG_PreMerge(EA.Repository Repository, string PackageGuid){return 1;}
		
		/// <summary>
		/// MDG_PreReverse enables the Add-In to save file changes before being imported into Enterprise Architect.
		/// This function operates on a list of files that are about to be reverse-engineered into Enterprise Architect. If the user is working on unsaved versions of these files in an editor, you could either prompt the user or save automatically.
		/// Also look at MDG_PostGenerate and MDG_PreGenerate.
		/// </summary>
 		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		/// <param name="FilePaths">A string array of filepaths pointed to the files that are to be reverse engineered.</param>
		public virtual void MDG_PreReverse(EA.Repository Repository, string PackageGuid,object FilePaths){}
		
		/// <summary>
		/// MDG_RunExe enables the Add-In to run the target application. This function is called when the user selects the Add-Ins | Run Exe menu option. Respond to this event by launching the compiled application.
		/// Also look at MDG_BuildProject.
		/// </summary>
 		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		public virtual void MDG_RunExe(EA.Repository Repository, string PackageGuid){}
		
		/// <summary>
		/// MDG_View enables the Add-In to display user specified code elements. This function is called by Enterprise Architect when the user asks to view a particular code element. This enables the Add-In to present that element in its own way, usually in a code editor.
		/// </summary>
 		/// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
		/// <param name="PackageGuid">The GUID identifying the Enterprise Architect package sub-tree that is controlled by the Add-In.</param>
		/// <param name="CodeID">Identifies the code element in the following format:
		/// [type]ElementPart[type]ElementPart...
		/// where each element is proceeded with a token identifying its type:
		/// @ -namespace
		/// # - Class
		/// $ - attribute
		/// % - operation
		/// For example if a user has selected the m_Name attribute of Class1 located in namespace Name1, the class ID would be passed through in the following format:
		/// @Name1#Class1%m_Name</param>
		/// <returns>Return a non-zero value to indicate that the Add-In has processed the request. Returning a zero value results in Enterprise Architect employing the standard viewing process which is to launch the associated source file.</returns>
		public virtual long MDG_View(EA.Repository Repository, string PackageGuid, string CodeID){return 0;}
	
		#endregion EA MDG Events
		
	}
}
