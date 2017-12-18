using System.Collections.Generic;
using System.Linq;
using System;
using EAAddinFramework.Databases.Strategy;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML=TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of CheckConstraint.
	/// </summary>
	public class CheckConstraint:Constraint,DB.CheckConstraint
	{
		private Column _column;
		public CheckConstraint(string name, Column column , string rule)
			:base(column._ownerTable,new List<Column>{column},column.strategy.getStrategy<CheckConstraint>())
		{
			this._column = column;
			this.rule = rule;
			this.name = name;
		}
		public CheckConstraint(Table ownerTable, Operation wrappedOperation)
			:base(ownerTable,wrappedOperation,ownerTable.strategy.getStrategy<CheckConstraint>())
		{
		}
		private string _rule;
		public string rule 
		{
			get 
			{
				if (_rule == null)
				{
					if (this._wrappedOperation != null)
					{
						_rule = this._wrappedOperation.code;
					}
				}
				return _rule;
			}
			set 
			{
				_rule = value;
				if (this._wrappedOperation != null)
				{
					this._wrappedOperation.code = _rule;
				}
			}
		}
		protected override void saveMe()
		{
			base.saveMe();
			if (this._wrappedOperation != null)
			{
				this.rule = this.rule;
				this._wrappedOperation.save();
			}
		}

		#region implemented abstract members of DatabaseItem

		protected override DatabaseItem createAsNew(DatabaseItem owner, bool save = true)
		{
			throw new NotImplementedException();
		}

		public override List<UML.Classes.Kernel.Element> logicalElements {
			get 
			{
				//there is no logical element for an index
				return new List<UML.Classes.Kernel.Element>();
			}
			set
			{
				//do nothing as there is not logical element for an index
			}
		}

		#endregion

		#region implemented abstract members of Constraint

		protected override string getStereotype()
		{
			return  "check";
		}

		internal override void createTraceTaggedValue()
		{
			throw new NotImplementedException();
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
	}
}
