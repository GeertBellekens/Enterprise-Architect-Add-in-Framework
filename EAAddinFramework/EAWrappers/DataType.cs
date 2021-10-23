using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// Description of DataType.
	/// </summary>
	public class DataType: ElementWrapper,UML.Classes.Kernel.DataType
	{
		public DataType(Model model, global::EA.Element elementToWrap): base(model, elementToWrap)
		{}
		
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Feature> features {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Dependencies.Substitution> substitutions {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool isLeaf {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.RedefinableElement> redefinedElements {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Classifier> redefinitionContexts {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
