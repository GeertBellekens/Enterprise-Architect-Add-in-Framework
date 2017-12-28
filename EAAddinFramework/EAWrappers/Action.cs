using System;
using System.Collections.Generic;
using System.Linq;

using System.Xml;
using EAAddinFramework.Utilities;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
	/// <summary>
	/// Description of Action.
	/// </summary>
	public class Action: ElementWrapper,UML.Actions.BasicActions.Action
	{
		public Action(Model model, global::EA.Element wrappedElement)
      	: base(model,wrappedElement)
    	{
    	}
		 /// <summary>
		 /// The classifier that owns the behavior of which this action is a part.
		 /// </summary>
		 public UML.Classes.Kernel.Classifier context
		 {
        	get{throw new NotImplementedException();}
        	set{throw new NotImplementedException();} 
         }
        
        /// <summary>
        /// The ordered set of input pins connected to the Action. These are among the total set of inputs. {Specializes
        /// Element::ownedElement}
        /// </summary>
         public HashSet<UML.Actions.BasicActions.InputPin> input
         {
        	get{throw new NotImplementedException();}
        	set{throw new NotImplementedException();} 
         }
		
        /// <summary>
        ///The ordered set of output pins connected to the Action. The action places its results onto pins in this set.
        ///{Specializes Element::ownedElement}
        /// </summary>
		 public HashSet<UML.Actions.BasicActions.OutputPin> output
		 {
        	get{throw new NotImplementedException();}
        	set{throw new NotImplementedException();} 
         }
		 private ActionKind getActionKindFromWrappedElement()
		 {
		 	foreach (global::EA.CustomProperty property in this.wrappedElement.CustomProperties)
	 		{
	 			if (property.Name == "kind")
	 			{
	 				string actionKindName = property.Value;
	 				ActionKind foundActionKind;
	 				if (Enum.TryParse<ActionKind>(actionKindName, out foundActionKind))
	 				{
	 					return foundActionKind;
	 				}
		 		}
		 	}
		 	//default = Atomic
		 	return ActionKind.Atomic;
		 }
		 public ActionKind kind
		 {
		 	get
		 	{
		 		return (ActionKind)this.getProperty(getPropertyNameName(),this.getActionKindFromWrappedElement());
		 	}
		 	set
		 	{
		 		this.setProperty(getPropertyNameName(),value,this.getActionKindFromWrappedElement());
		 		
		 	}
		 }
		internal override void saveElement()
		{
			base.saveElement();
			//check if the kind has been changed. In that case we need to save it directly in the database
			if (this.isPropertyDirty("kind"))
			{
				//get the name of the new ActionKind
				string newActionKindString = Enum.GetName(typeof(ActionKind),this.kind);
		 		// there is no API method available so we will have to do it the hard way
		 		// first try to get the t_xref record details
		 		string sqlGetActionKind = @"select x.Description from t_object o
				inner join t_xref x on x.Client = o.ea_guid
				where x.Name = 'CustomProperties'
				and x.Type = 'element property'
				and o.Object_ID = "+ this.wrappedElement.ElementID;
		 		XmlDocument descriptionXml  = this.EAModel.SQLQuery(sqlGetActionKind);
				XmlNodeList descriptionNodes = descriptionXml.SelectNodes(this.EAModel.formatXPath("//Description"));
				foreach (XmlNode descriptionNode in descriptionNodes) 
				{
					string description = descriptionNode.InnerText;
					string [] descriptionParts = description.Split(new String[]{"@PROP="},StringSplitOptions.RemoveEmptyEntries);
					
					for (int i = 0; i < descriptionParts.Count(); i++)
					{
						string descriptionPart = descriptionParts[i];
						bool kindFound = false;
						string existingActionKindString = string.Empty;
						foreach (var propPart in descriptionPart.Split('@')) 
						{
							var keyValuePair = propPart.Split('=');
							if (keyValuePair.Count() == 2)
							{
								if (keyValuePair[0].Contains("TYPE") 
								    && keyValuePair[1] == "ActionKind") 
									kindFound = true;
								if (keyValuePair[0].Contains("VALU"))
									existingActionKindString = keyValuePair[1];
							}
						}
						if (kindFound)
						{
							
							description = description.Replace("@TYPE=ActionKind@ENDTYPE;@VALU="+existingActionKindString+"@ENDVALU;"
							                                  ,"@TYPE=ActionKind@ENDTYPE;@VALU="+newActionKindString+"@ENDVALU;" );
							//now update the txref record
							string updateXref = @"update t_xref set Description = '"+description+@"' 
													where Name = 'CustomProperties'
													and Type = 'element property'
													and Client = '"+this.uniqueID+"'";
							this.EAModel.executeSQL(updateXref);
							// no need to continue for loop
							break;
						}
					}
				}
				if (descriptionNodes.Count == 0)
				{
					//setting a new action kind when none is defined yet is troublesome.
					// It works, but we gan an error box popping up in EA.
					// Gor some reason EA is executing the command "SELECT * FROM t_xref WHERE" right after we execute the insert statement
					// setting this between try/catch isn't helping either.
					
//					//need to do an insert
//					string insertXref = "insert into t_xref values ('" + Guid.NewGuid().ToString("B") + "','CustomProperties','element property','Public',null,null,null,null,0,"
//										+ "'@PROP=@NAME=kind@ENDNAME;@TYPE=ActionKind@ENDTYPE;@VALU="
//										+ newActionKindString
//										+"@ENDVALU;@PRMT=@ENDPRMT;@ENDPROP;@PROP=@NAME=value@ENDNAME;@TYPE=String@ENDTYPE;@VALU=blablaValue@ENDVALU;@PRMT=@ENDPRMT;@ENDPROP;'"
//										+",'" + this.guid + "','<none>',null)";
//					try
//					{
//						this.model.executeSQL(insertXref);
//					}
//					catch(Exception e)
//					{
//						Logger.log(e.Message);
//					}
				}
			}
		}
		 
		

	}
}
