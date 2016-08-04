
using System;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of ForeignKey.
	/// </summary>
	public class ForeignKey:Constraint, DB.ForeignKey
	{
		private Association _wrappedAssociation;
		internal Table _foreignTable;
		
		public ForeignKey(Table owner,Operation operation):base(owner,operation)
		{
		}
		
		internal Association wrappedAssociation
		{
			get
			{
				if (_wrappedAssociation == null)
				{
					this.getWrappedAssociation();
				}
				return _wrappedAssociation;
			}
			
		}
		private void getWrappedAssociation()
		{
			if (_wrappedOperation != null)
			{
				string getAssociationQuery = @"select c.Connector_ID from t_connector c 
											where c.Start_Object_ID = " + this._owner._wrappedClass.id.ToString() +
					" and c.SourceRole = '"+ _wrappedOperation.name +"'";
				_wrappedAssociation = this._wrappedOperation.model.getRelationsByQuery(getAssociationQuery).FirstOrDefault() as Association;
			}
		}
		public override string properties {
			get 
			{
				string _properties = base.properties;
				if (foreignTable != null)
				{
					_properties += " => " + foreignTable.name;
				}
				return _properties;
			}
		}
		public override string itemType {
			get {return "Foreign Key";}
		}
		#region ForeignKey implementation
		public DB.Table foreignTable {
			get 
			{
				if (_foreignTable == null)
				{
					this.getForeignTable();
				}
				return _foreignTable;
			}
			set {
				throw new NotImplementedException();
			}
		}
		#endregion
		private void getForeignTable()
		{
			if (wrappedAssociation != null)
			{
				var targetClass = _wrappedAssociation.target as Class;
				if (targetClass != null)
				{
					_foreignTable = new Table(this._owner._owner, targetClass);
				}
			}
		}

	}
}
