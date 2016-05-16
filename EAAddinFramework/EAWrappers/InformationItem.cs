using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of Action.
	/// </summary>
	public class InformationItem: ElementWrapper,UML.InfomationFlows.InformationItem
	{
		public InformationItem(Model model, global::EA.Element wrappedElement)
			: base(model,wrappedElement){}

	
 		public HashSet<UML.Classes.Kernel.Classifier> represented 
		{
			get 
			{
				throw new NotImplementedException();
			}
			set 
			{
				throw new NotImplementedException();
			}
		}

		public HashSet<UML.Classes.Kernel.Feature> features {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public HashSet<UML.Classes.Dependencies.Substitution> substitutions {
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

		public HashSet<UML.Classes.Kernel.RedefinableElement> redefinedElements {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public HashSet<UML.Classes.Kernel.Classifier> redefinitionContexts {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
