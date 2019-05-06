
using System;
using EAAddinFramework.Databases.Strategy;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using TSF_EA=TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;
using System.Collections.Generic;
using System.Linq;
namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of Column.
	/// </summary>
	public class Column:DatabaseItem, DB.Column
	{
		internal Table _ownerTable;
		protected TSF_EA.Attribute _wrappedattribute;
        public TSF_EA.Attribute wrappedattribute { get { return _wrappedattribute; } }
        internal DataType _type;
		private TSF_EA.TaggedValue _traceTaggedValue;
		private TSF_EA.Attribute _logicalAttribute;
		private List<TSF_EA.Attribute> _logicalAttributes;
		private string _name;
        private bool? _isNotNullable;
        internal string _initialValue;

        public Column(Table owner, TSF_EA.Attribute attribute):base(owner.strategy.getStrategy<Column>())
		{
			this._ownerTable = owner;
			this._wrappedattribute = attribute;
		}
		public Column(Table owner, string name):base(owner.strategy.getStrategy<Column>())
		{
			this._ownerTable = owner;
			this.name = name;
			this.ownerTable.addColumn(this);
		}
        /// <summary>
        /// reloads the data from the dataabse
        /// </summary>
        public void reload()
        {
            if (this.wrappedattribute != null)
            {
                this.wrappedattribute.reload();
                this._name = null;
                this._type = null;
                this._isNotNullable = null;
                this._initialValue = null;
            }
        }
		
		internal bool initialOverride = false;
		internal bool getInitialOverride()
		{
			initialOverride = base.isOverridden;
			return initialOverride;
		}
		public override bool isOverridden 
		{
			get 
			{
				return base.isOverridden;
			}
			set 
			{
				if (! isRemote)
				{
					base.isOverridden = value;
				}
				else
				{
					this._isOverridden = value;
					//create tagged value if needed if not overridden then we don't need the tagged value;
					if (this.wrappedElement != null)
					{
						wrappedElement.addTaggedValue("dboverride",value.ToString().ToLower());
					}
				}
			}
		}
		/// <summary>
		/// a column is remote if the logical attribute for this column has a different owner then the logical element of the owner table
		/// </summary>
		public bool isRemote 
		{
			get { return _logicalAttribute != null && _logicalAttribute.owner != this._ownerTable.logicalElement;}
		}

		private int? _position;
		public override int position 
		{
			get 
			{
				if (! _position.HasValue)
				{
					if (_wrappedattribute != null)
					{
						this._position = _wrappedattribute.position;
					}
				}
				return _position.GetValueOrDefault();
			}
			set 
			{
				this._position = value;
				if (_wrappedattribute != null)
				{
					_wrappedattribute.position = _position.Value;
				}
			}
		}

		#region implemented abstract members of DatabaseItem
		protected override void saveMe()
		{
			//create the _wrapped attribute if needed
			if (_wrappedattribute == null)
			{
				if (this._ownerTable.wrappedClass == null)
				{
					this.ownerTable.save();
				}
				//now the wrappedClass should exist. if not then we have a problem
				this._wrappedattribute = this.factory.modelFactory.createNewElement<TSF_EA.Attribute>(this._ownerTable.wrappedClass,this.name);

				if (this.logicalAttribute!= null)
				{
					var attributeType = logicalAttribute.type as TSF_EA.ElementWrapper;
					if (attributeType != null)
					{
						//check if attributeType as any constraints
						var typeConstraint = attributeType.constraints.OfType<TSF_EA.Constraint>().FirstOrDefault();
						if (typeConstraint != null)
						{
							//add a check constraint
							var checkConstraint = new CheckConstraint(this.name, this, typeConstraint.convertFromEANotes("TXT").Replace("<Column>", this.name));
							//save?
							checkConstraint.save();
						}
					}
				}					    
			}
			if (_wrappedattribute != null)
			{
				//set steretotype
				this._wrappedattribute.setStereotype("column");
				//set datatype;
				_wrappedattribute.type = this.factory.modelFactory.createPrimitiveType(this.type.name);
				if (this.type.type != null && this.type.type.hasPrecision)
				{
					_wrappedattribute.precision = this.type.length;
					_wrappedattribute.scale = this.type.precision;
				}
				else
				{
					_wrappedattribute.length = this.type.length;
				}
				//is not nullable
				this.isNotNullable = this.isNotNullable;
				//set position
				this.position = this.position;
				// InitialValue
				this.initialValue = this.initialValue;
        		//save
				_wrappedattribute.save();
				//following properties are saved as tagged values so need to be set after saving the wrapped atttribute
				//set isOverridden
				this.isOverridden = this.isOverridden;
				//set renamed
				this.isRenamed = this.isRenamed;
				//logical attribute tag value
				if (traceTaggedValue == null) createTraceTaggedValue();
			}
			//save the columnn name in the alias
			if (logicalAttribute != null) logicalAttribute.save(); 
				
		}
		
		public override List<UML.Classes.Kernel.Element> logicalElements 
		{
			get 
			{
				return logicalAttributes.OfType<UML.Classes.Kernel.Element>().ToList();
			}
			set
			{
				this._logicalAttributes = value.OfType<TSF_EA.Attribute>().ToList();
			}
		}
		#region implemented abstract members of DatabaseItem
		private bool? _isValid;
		public override bool isValid 
		{
			get 
			{
				if (! _isValid.HasValue)
				{
					_isValid = ! string.IsNullOrEmpty(this.name)
				        && this.type != null
						&& this.type.isValid;
					if (_isValid.Value)
					{
						var sameColumns = ownerTable.columns.Where (x => x.name == this.name);
						if (sameColumns.Count() > 1)
						{
							_isValid = sameColumns.Count(x => ! x.isNotRealized) <= 1;
						}
					}
				}
				// a column is valid if it has a name, a type, and if there's no other column in the table with the same except if that column is not realized
				return _isValid.Value;
			}
		}
		#endregion
		#region implemented abstract members of DatabaseItem
		protected override DatabaseItem createAsNew(DatabaseItem owner, bool save = true)
		{
			Table newTable = owner as Table;
			Database existingDatabase = owner as Database;
			if (newTable == null)
			{
				//look for corresponding table in existingDatabase
				newTable = (Table)existingDatabase.tables.FirstOrDefault(x => x.name == this.ownerTable.name);
			}
			if (newTable != null)
			{
				var newColumn = new Column(newTable,this.name);
				newColumn.isNew = true;
				newColumn.isNotNullable = isNotNullable;
				newColumn.type = type;
				newColumn.logicalAttribute = logicalAttribute;
				newColumn.logicalElements = logicalElements;
				newColumn.isOverridden = isOverridden;
				newColumn.isRenamed = isRenamed;
				newColumn.position = position;
				newColumn.initialValue = initialValue;
				newColumn.derivedFromItem = this;
				if (save) newColumn.save();
				return newColumn;
			}
			return null;
		}
		#endregion
		#region implemented abstract members of DatabaseItem
		protected override void updateDetails(DB.DatabaseItem newDatabaseItem)
		{
			var newColumn = (Column)newDatabaseItem;
			this._isNotNullable = newColumn.isNotNullable;
			this._logicalAttribute = newColumn.logicalAttribute;
			this._type = newColumn._type;
			this.isOverridden = newColumn.isOverridden;
			this.isRenamed = newColumn.isRenamed;
			this.position = newColumn.position;
			this.initialValue = newColumn.initialValue;
		}
		#endregion		
		
		protected override void deleteMe()
		{
			if (_wrappedattribute != null) _wrappedattribute.delete();
			this.ownerTable.removeColumn(this);
		}


		internal override void createTraceTaggedValue()
		{
			if (this._wrappedattribute != null)
			{
				string tagValue = string.Empty;
				if (_logicalAttribute != null) tagValue = _logicalAttribute.guid;
				 this._wrappedattribute.addTaggedValue("sourceAttribute",tagValue);
			}
			
		}
		internal override Element wrappedElement 
		{
			get 
			{
				return _wrappedattribute;
			}
			set 
			{
				this._wrappedattribute = (TSF_EA.Attribute)value;
			}
		}
		internal override TaggedValue traceTaggedValue 
		{
			get 
			{
				if (_traceTaggedValue == null)
				{
					if (_wrappedattribute != null)
					{
						_traceTaggedValue = _wrappedattribute.taggedValues.OfType<TaggedValue>().FirstOrDefault(x => x.name.Equals("sourceAttribute",StringComparison.InvariantCultureIgnoreCase));
					}
				}
				//no wrapped attribute so retur null
				return _traceTaggedValue;
			}
			set 
			{
				_traceTaggedValue = value;
				if (_wrappedattribute != null
				   && value != null)
				{
					_traceTaggedValue = _wrappedattribute.addTaggedValue(value.name, value.eaStringValue,value.comment);
					_traceTaggedValue.save();
				}
			}
		}
		#endregion		
		public TSF_EA.Attribute logicalAttribute
		{
			get
			{
				if (_logicalAttribute == null
				   && _wrappedattribute != null)
				{
					_logicalAttribute = _wrappedattribute.taggedValues
						.Where(x => x.name.Equals("sourceAttribute",StringComparison.InvariantCultureIgnoreCase)
						             && x.tagValue is TSF_EA.Attribute)
						.Select(y => y.tagValue as TSF_EA.Attribute).FirstOrDefault();
				}
				return _logicalAttribute;
			}
			set
			{
				_logicalAttribute = value;
				//add the logical attribute to the list of logical attributes
				if (_logicalAttributes == null)
				{
					_logicalAttributes = new List<TSF_EA.Attribute>();
					_logicalAttributes.Add(_logicalAttribute);
				}
				else if (!_logicalAttributes.Contains(_logicalAttribute))
				{
					_logicalAttributes.Add(_logicalAttribute);
				}
				this.createTraceTaggedValue();
			}
		}
		public List<TSF_EA.Attribute> logicalAttributes
		{
			get
			{
				if (_logicalAttributes == null)
				{
				 	if ( _wrappedattribute != null)
					{
						_logicalAttributes = _wrappedattribute.taggedValues
							.Where(x => x.name.Equals("sourceAttribute",StringComparison.InvariantCultureIgnoreCase)
							             && x.tagValue is TSF_EA.Attribute)
							.Select(y => y.tagValue as TSF_EA.Attribute).ToList();
					}
				}
				return _logicalAttributes ?? new List<TSF_EA.Attribute>();
			}
		}
		
		#region Column implementation

		public DB.Table ownerTable {
			get {return this._ownerTable;}
			set {this._ownerTable = (Table)value;}
		}

		#region implemented abstract members of DatabaseItem
		public override DB.DatabaseItem owner {
			get {
				return ownerTable;
			}
		}
		#endregion
		public override string itemType {
			get {return "Column";}
		}
		public override string properties {
			get 
			{
				
				string _properties = string.Empty;
				if (this.type != null ) _properties += this.type.properties;
				if (this.isNotNullable)
				{
					_properties += " Not Null";
				}
				if( ! string.IsNullOrEmpty(this.initialValue)
				   && ! this.initialValue.Equals("DEFAULT")) //TODO: find a better way then hardcoding this value here.
				{
					_properties += " DEFAULT " + this.initialValue;
				}
				return _properties;
			}
		}
		public override string name {
			get 
			{
				if(_wrappedattribute != null) _name = _wrappedattribute.name;
				return _name;
			}
			set 
			{
				if (this.isRemote && !string.IsNullOrEmpty(_name) && _name != value) this.isRenamed = true;
				_name = value;
				if (_wrappedattribute != null) this._wrappedattribute.name = _name;
				if (this.logicalAttribute != null && !this.isRemote) this.logicalAttribute.alias = value;
				resetIsValid();
			}
		}
		private void resetIsValid()
		{
			foreach (Column column in this.ownerTable.columns)
			{
				column._isValid = null;
			}
		}

		public DB.DataType type 
		{
			get 
			{
				if (_type == null)
				{
					_type = this.getDataType();
				}
				return _type;
			}
			set 
			{
				_type = (DataType)value;
				this.setDataType();
			}
		}

		
		public bool isNotNullable
		{
			get 
			{
				if (! _isNotNullable.HasValue)
				{
					if(_wrappedattribute != null) _isNotNullable = _wrappedattribute.allowDuplicates;
				}
				return _isNotNullable.GetValueOrDefault();
			}
			set 
			{
				_isNotNullable = value;
				if (_wrappedattribute != null) this._wrappedattribute.allowDuplicates = _isNotNullable.Value;
			}
		}


		private DataType getDataType()
		{
			if (this._wrappedattribute != null)
			{
				var basetype = this.ownerTable.databaseOwner.databaseFactory.baseDataTypes.FirstOrDefault(x => x.name == this._wrappedattribute.type.name);
				if (basetype != null)
				{
					if (basetype.hasPrecision)
					{
						return  new DataType((BaseDataType)basetype, this._wrappedattribute.precision, this._wrappedattribute.scale);
					}
					else
					{
						return  new DataType((BaseDataType)basetype, this._wrappedattribute.length, 0);
					}
				}
			}
			return null;
		}
		private void setDataType()
		{
			if (this._wrappedattribute != null)
			{
				this._wrappedattribute.type = this._wrappedattribute.EAModel.factory.createPrimitiveType(this.type.type.name);
				if (this.type.type.hasPrecision)
				{
					this._wrappedattribute.precision = this.type.length;
					this._wrappedattribute.scale = this.type.precision;
				}
				else
				{
					this._wrappedattribute.length = this.type.length;
				}
			}
		}

    
    public string initialValue 
    {
		get 
		{
			if( this._initialValue == null
			&& this._wrappedattribute != null )
			{
				this._initialValue = this._wrappedattribute.defaultValue.ToString();
			}
			return this._initialValue;
		}
		set 
		{
			var newInitial = value ?? string.Empty;
			this._initialValue = newInitial;
			if (this._wrappedattribute != null)
			{
				_wrappedattribute.setDefaultValue(newInitial);
			}
		}
    }

		#endregion

	}
}
