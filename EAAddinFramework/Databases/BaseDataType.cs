
using System;
using System.Collections.Generic;
using EAAddinFramework.Utilities;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;
namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of BaseDataType.
	/// </summary>
	public class BaseDataType:DB.BaseDataType
	{
        
		public BaseDataType(string name, bool hasLength, bool hasPrecision)
		{
			this.name = name;
			this.hasLength = hasLength;
			this.hasPrecision = hasPrecision;
            this.databaseProduct = string.Empty;
		}
		public BaseDataType(global::EA.Datatype eaDatatype)
		{
			this.name = eaDatatype.Name;
			this.hasLength = eaDatatype.Size > 0;
			this.hasPrecision = eaDatatype.Size == 2;
            this.databaseProduct = eaDatatype.Product;
		}
        public string databaseProduct { get; private set; }


		public List<DB.DatabaseItem> mergedEquivalents {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public bool isNotRealized {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		#region BaseDataType implementation
		public void save()
		{
			//do nothing. deleting or adding of base datatypes should happen manually in the EA GUI
		}



		public void delete()
		{
			//do nothing. deleting or adding of base datatypes should happen manually in the EA GUI
		}

		public void Select()
		{
			//you cannot select a Base Datatype
		}
		public List<UML.Classes.Kernel.Element> logicalElements {
			get 
			{
				//base datatypes don't have logical elements
				return new List<UML.Classes.Kernel.Element>();
			}
		}
		public bool isValid {
			get 
			{
				//base data type is valid if it has a name
				return ! string.IsNullOrEmpty(this.name);
			}
		}
		public DB.DatabaseItem createAsNewItem(DB.DatabaseItem owner,bool save = true)
		{
			//do nothing. deleting or adding of base datatypes should happen manually in the EA GUI
			return null;
		}

		public int position {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public DB.DatabaseItem derivedFromItem {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public void update(DB.DatabaseItem newDatabaseItem, bool save = true)
		{
			//don't think we ever need this one
			throw new NotImplementedException();
		}
		public DB.DatabaseItem owner {
			get {
				//don't think we ever need this one
				throw new NotImplementedException();
			}
		}
		public DB.DataBaseFactory factory {
			get {
				//TODO:figure out if we need it here
				throw new NotImplementedException();
			}
		}
		public string name {get;set;}

		public bool hasLength {get;set;}

		public bool hasPrecision {get;set;}

		public string itemType 
		{
			get {return "BaseDatatype";}
		}
		


		public string properties 
		{
			get { return this.name;}
		}

		#endregion
		//base datatypes can't be overriden
		public bool isOverridden 
		{
			get 
			{
				return false;
			}
			set {
				//do nothing
			}
		}

		public bool isRenamed {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public DB.DatabaseItem createAsNewItem(DB.DatabaseItem owner, string newName, bool save = true)
		{
			throw new NotImplementedException();
		}

		public string renamedName {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public bool isEqualDirty {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public string logicalName {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
