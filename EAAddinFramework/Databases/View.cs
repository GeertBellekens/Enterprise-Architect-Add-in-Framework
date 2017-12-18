
using System;
using System.Collections.Generic;
using EAAddinFramework.Databases.Strategy;
using EAAddinFramework.Utilities;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using UML = TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of View.
	/// </summary>
	public class View:DatabaseItem, DB.View
	{
		
		internal Class _wrappedClass;
		internal List<Column> _columns;
		internal List<Constraint> _constraints;
		internal List<Class> _logicalClasses;
		private Database _owner;
		private string _name;
		
		public View(Database owner,Class wrappedClass, DatabaseItemStrategy strategy):base(strategy)
		{
			this._wrappedClass = wrappedClass;
			this._owner = owner;
			this.viewOwner = _owner.defaultOwner;
		}
		public View(Database owner, string name, DatabaseItemStrategy strategy):base(strategy)
		{
			this._name = name;
			this._owner = owner;
			this.databaseOwner.addView(this);
			this.viewOwner = _owner.defaultOwner;
		}

		internal override Element wrappedElement 
		{
			get 
			{
				return _wrappedClass;
			}
			set 
			{
				this._wrappedClass = (Class)value;
			}
		}
		#region DatabaseItem implementation

		protected override void saveMe()
		{
			if (_wrappedClass == null) 
			{
				var databasePackage = this._owner._wrappedPackage;
				
				if (databasePackage != null)
				{
					Package viewPackage = databasePackage.ownedElements.OfType<Package>()
						.FirstOrDefault(x => x.name.Equals("Views",StringComparison.InvariantCultureIgnoreCase));
					Package ownerPackage = databasePackage;
					if (viewPackage != null) ownerPackage = viewPackage;
					_wrappedClass = this._factory._modelFactory.createNewElement<Class>(ownerPackage, this.name);
					//TODO: provide wrapper function for gentype?
					_wrappedClass.wrappedElement.Gentype = this.factory.databaseName;
					_wrappedClass.setStereotype("EAUML::view");
					_wrappedClass.save();
					this.viewOwner = this.viewOwner;
					this.definition = definition;
				}
				else
				{
					throw new Exception(string.Format("cannot save {0} because wrapped package for database {1} does not exist",this.name, this.databaseOwner.name));
				}
			}
			else
			{
				//save the class to save the changes to the existing table
				_wrappedClass.save();
			}
			
		}


		public override string name 
		{
			get 
			{
				if (this._wrappedClass != null) this._name = this._wrappedClass.name;
				return this._name;
			}
			set 
			{
				if (!string.IsNullOrEmpty(_name) && _name != value) this.isRenamed = true;
				this._name = value;
				if (this._wrappedClass != null) this._wrappedClass.name = this._name;
			}
		}

		

		private int _position;
		public override int position 
		{
			get 
			{
				if (_wrappedClass != null)
				{
					_position = _wrappedClass.position;
				}
				return _position;
			}
			set 
			{
				_position = value;
				if (_wrappedClass != null)
				{
					_wrappedClass.position =_position;
				}
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

		public string renamedName 
		{
			get { throw new NotImplementedException();}
			set { throw new NotImplementedException();}
		}

		public bool isNotRealized 
		{
			get { throw new NotImplementedException();}
			set { throw new NotImplementedException();}
		}

		public bool isEqualDirty 
		{
			get { throw new NotImplementedException();}
			set { throw new NotImplementedException();}
		}

		#endregion

		#region View implementation

		public DB.Database databaseOwner 
		{
			get 
			{
				return _owner;
			}
			set 
			{
				_owner = (Database)value;
			}
		}

		private string _definition = null;
		public string definition 
		{
			get 
			{
				if (_definition == null)
				{
					//get the tagged value if it exists
					if (this.wrappedElement != null)
					{
						var ownerTag = this.wrappedElement.taggedValues
							.FirstOrDefault( x => x.name.Equals("viewdef",StringComparison.InvariantCultureIgnoreCase));
						_definition = ownerTag != null ? ownerTag.tagValue.ToString() : null;
					}
				}
				return _definition ?? string.Empty;
			}
			set
			{
				this._definition = value;
				//create tagged value if needed
				if (this.wrappedElement != null)
				{
					wrappedElement.addTaggedValue("viewdef",value);
				}
			}
		}

		private string _viewOwner = null;
		public string viewOwner 
		{
			get 
			{
				if (_viewOwner == null)
				{
					//get the tagged value if it exists
					if (this.wrappedElement != null)
					{
						var ownerTag = this.wrappedElement.taggedValues
							.FirstOrDefault( x => x.name.Equals("Owner",StringComparison.InvariantCultureIgnoreCase));
						_viewOwner = ownerTag != null ? ownerTag.tagValue.ToString() : null;
					}
				}
				return _viewOwner ?? string.Empty;
			}
			set
			{
				this._viewOwner = value;
				//create tagged value if needed
				if (this.wrappedElement != null)
				{
					wrappedElement.addTaggedValue("Owner",value);
				}
			}
		}

		#endregion

		#region implemented abstract members of DatabaseItem

		internal override void createTraceTaggedValue()
		{
			throw new NotImplementedException();
		}

		protected override DatabaseItem createAsNew(DatabaseItem owner, bool save = true)
		{
			throw new NotImplementedException();
		}

		protected override void updateDetails(DB.DatabaseItem newDatabaseItem)
		{
			//no specifics
		}

		protected override void deleteMe()
		{
			if (this.wrappedElement != null)
				this.wrappedElement.delete();
			this._owner.removeView(this);
		}

		internal override TaggedValue traceTaggedValue {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		#endregion

		#region implemented abstract members of DatabaseItem

		public override List<UML.Classes.Kernel.Element> logicalElements {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public override bool isValid 
		{
			get {return true;} //always valid for now
		}

		public override DB.DatabaseItem owner 
		{
			get {return this._owner;}
		}

		public override string itemType 
		{
			get { return "View";}
		}

		public override string properties 
		{
			get 
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
