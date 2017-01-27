
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
		internal ElementWrapper _wrappedElement;

		public UML.Classes.Kernel.Element mappingElement 
		{
			get{return _wrappedElement;}
			set{this._wrappedElement = value as ElementWrapper;}
		}
		public MappingLogic(string logicDescription)
		{
			_description = logicDescription;
		}
		public MappingLogic(ElementWrapper wrappedElement)
		{
			this.mappingElement = wrappedElement;
		}

		#region MappingLogic implementation

		public string description {
			get 
			{
				if (string.IsNullOrEmpty(_description)
				   && this.mappingElement != null)
				{
					_description = _wrappedElement.notes;
				}
				return _description;
			}
			set 
			{
				if (mappingElement != null)
				{
					_wrappedElement.notes = value;
				}
				_description = value;
			}
		}

		#endregion
	}
}
