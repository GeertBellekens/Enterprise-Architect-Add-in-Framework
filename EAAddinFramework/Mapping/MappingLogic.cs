
using System;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of MappingLogic.
	/// </summary>
	public class MappingLogic:MP.MappingLogic
	{
		private string _description;
		private ElementWrapper _wrappedElement;
		internal ElementWrapper wrappedElement
		{
			get{return _wrappedElement;}
			set{this._wrappedElement = value;}
		}
		public MappingLogic(string logicDescription)
		{
			_description = logicDescription;
		}
		public MappingLogic(ElementWrapper wrappedElement)
		{
			this.wrappedElement = wrappedElement;
		}

		#region MappingLogic implementation

		public string description {
			get 
			{
				if (string.IsNullOrEmpty(_description)
				   && this.wrappedElement != null)
				{
					_description = wrappedElement.notes;
				}
				return _description;
			}
			set 
			{
				if (wrappedElement != null)
				{
					wrappedElement.notes = value;
				}
				_description = value;
			}
		}

		#endregion
	}
}
