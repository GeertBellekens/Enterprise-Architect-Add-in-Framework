using System;
using System.Collections.Generic;
using System.Linq;

using System.Xml;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of EnumerationLiteral.
	/// </summary>
	public class EnumerationLiteral : AttributeWrapper, UML.Classes.Kernel.EnumerationLiteral
	{
		public EnumerationLiteral(Model model, global::EA.Attribute wrappedAttribute) 
      : base(model, wrappedAttribute)
		{
			if (!this.wrappedAttribute.StyleEx.Contains("IsLiteral="))
		    {
				this.wrappedAttribute.StyleEx = "IsLiteral=1;" + this.wrappedAttribute.StyleEx;
		    }else
			{
				this.wrappedAttribute.StyleEx = this.wrappedAttribute.StyleEx.Replace("IsLiteral=0;","IsLiteral=1;");
			}
		}

		public static bool isLiteralValue(Model model, global::EA.Attribute wrappedAttribute)
		{
			//if the field StyleEx contains "IsLiteral=1" then it is a literal value
			return (wrappedAttribute.StyleEx.Contains("IsLiteral=1"));
		}
		public bool? booleanValue()
		{
			throw new NotImplementedException();
		}
		public int? integerValue()
		{
			throw new NotImplementedException();
		}
		public bool isComputable()
		{
			throw new NotImplementedException();
		}
		public bool isNull()
		{
			throw new NotImplementedException();
		}
		public double? realValue()
		{
			throw new NotImplementedException();
		}
		public string stringValue()
		{
			return this.name;
		}
		public UML.Classes.Kernel.UnlimitedNatural unlimitedValue()
		{
			throw new NotImplementedException();
		}
		public UML.Classes.Kernel.ValueSpecification specification {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public HashSet<UML.Classes.Kernel.Slot> slots {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public List<UML.Classes.Kernel.InstanceValue> instanceValues {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public HashSet<UML.Classes.Kernel.Classifier> classifiers {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public UML.Classes.Kernel.VisibilityKind visibility {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public string qualifiedName {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public UML.Classes.Kernel.Namespace owningNamespace {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public List<UML.Classes.Dependencies.Dependency> clientDependencies {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public List<UML.Classes.Dependencies.Dependency> supplierDependencies {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
