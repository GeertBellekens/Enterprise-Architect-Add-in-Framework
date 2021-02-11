using System;
using System.Collections.Generic;
using System.Linq;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class Attribute : AttributeWrapper, UML.Classes.Kernel.Property
    {


        public Attribute(Model model, global::EA.Attribute wrappedAttribute)
          : base(model, wrappedAttribute)
        { }
        public bool isID
        {
            get => (bool)this.getProperty(getPropertyNameName(), this.wrappedAttribute.IsID);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedAttribute.IsID);
        }
        public bool allowDuplicates
        {
            get => (bool)this.getProperty(getPropertyNameName(), this.wrappedAttribute.AllowDuplicates);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedAttribute.AllowDuplicates);
        }
        public UML.Classes.Kernel.ValueSpecification defaultValue
        {
            get => this.EAModel.factory.createValueSpecificationFromString((string)this.getProperty(getPropertyNameName(), this.wrappedAttribute.Default));
            set => this.setProperty(getPropertyNameName(), value.ToString(), this.wrappedAttribute.Default);
        }
        /// the isStatic property defines context of the attribute.
        /// If true then the context is the class
        /// If false then the context is the instance.
        public bool isStatic
        {
            get => (bool)this.getProperty(getPropertyNameName(), this.wrappedAttribute.IsStatic);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedAttribute.IsStatic);
        }
        public UML.Classes.Kernel.VisibilityKind visibility
        {
            get => VisibilityKind.getUMLVisibilityKind
                  ((string)this.getProperty(getPropertyNameName(), this.wrappedAttribute.Visibility),
                  UML.Classes.Kernel.VisibilityKind._private);
            set => this.setProperty(getPropertyNameName(), VisibilityKind.getEAVisibility(value), this.wrappedAttribute.Visibility);
        }

        public Multiplicity EAMultiplicity
        {
            get => (Multiplicity)this.getProperty(getPropertyNameName(), this.getInitialMultiplicity());
            set => this.setProperty(getPropertyNameName(), value, this.getInitialMultiplicity());
        }
        public int length
        {
            get
            {
                int returnedLength = 0;
                int.TryParse((string)this.getProperty(getPropertyNameName(), this.wrappedAttribute.Length), out returnedLength);
                return returnedLength;
            }
            set => this.setProperty(getPropertyNameName(), value.ToString(), this.wrappedAttribute.Length);
        }
        public int precision
        {
            get
            {
                int returnedprecision = 0;
                int.TryParse((string)this.getProperty(getPropertyNameName(), this.wrappedAttribute.Precision), out returnedprecision);
                return returnedprecision;
            }
            set => this.setProperty(getPropertyNameName(), value.ToString(), this.wrappedAttribute.Precision);
        }
        public int scale
        {
            get
            {
                int returnedScale = 0;
                int.TryParse((string)this.getProperty(getPropertyNameName(), this.wrappedAttribute.Scale), out returnedScale);
                return returnedScale;
            }
            set => this.setProperty(getPropertyNameName(), value.ToString(), this.wrappedAttribute.Scale);
        }

        internal override void saveElement()
        {
            if (this.getProperty("isID") != null)
            {
                this.wrappedAttribute.IsID = (bool)this.getProperty("isID");
            }

            if (this.getProperty("allowDuplicates") != null)
            {
                this.wrappedAttribute.AllowDuplicates = (bool)this.getProperty("allowDuplicates");
            }

            if (this.getProperty("defaultValue") != null)
            {
                this.wrappedAttribute.Default = (string)this.getProperty("defaultValue");
            }

            if (this.getProperty("isStatic") != null)
            {
                this.wrappedAttribute.IsStatic = (bool)this.getProperty("isStatic");
            }

            if (this.getProperty("visibility") != null)
            {
                this.wrappedAttribute.Visibility = (string)this.getProperty("visibility");
            }

            if (this.getProperty("length") != null)
            {
                this.wrappedAttribute.Length = (string)this.getProperty("length");
            }

            if (this.getProperty("precision") != null)
            {
                this.wrappedAttribute.Precision = (string)this.getProperty("precision");
            }
            if (this.getProperty("scale") != null)
            {
                this.wrappedAttribute.Scale = (string)this.getProperty("scale");
            }
            if (this.getProperty("isDerived") != null)
            {
                this.wrappedAttribute.IsDerived = (bool)this.getProperty("isDerived");
            }

            //multiplicity is a bit of an exception. We only save the info if it actually has changed
            var multiplicityProperty = this.getPropertyInfo("EAMultiplicity");
            if (multiplicityProperty != null && multiplicityProperty.isDirty)
            {
                this.WrappedAttribute.LowerBound = ((Multiplicity)multiplicityProperty.propertyValue).lower.ToString();
                this.WrappedAttribute.UpperBound = ((Multiplicity)multiplicityProperty.propertyValue).upper.ToString();
            }
            base.saveElement();
        }
        public UML.Classes.Kernel.UnlimitedNatural upper
        {
            get => this.EAMultiplicity.upper;
            set => this.EAMultiplicity.upper = value;
        }

        public uint lower
        {
            get => this.EAMultiplicity.lower;
            set => this.EAMultiplicity.lower = value;
        }
        public UML.Classes.Kernel.Multiplicity multiplicity
        {
            get => this.EAMultiplicity;
            set => this.EAMultiplicity = (Multiplicity)value;
        }

        public static Multiplicity defaultMultiplicity()
        {
            //default for attributes is 1..1
            string lowerString = "1";
            string upperString = "1";
            return new Multiplicity(lowerString, upperString);
        }

        private Multiplicity getInitialMultiplicity()
        {
            //default for attributes is 1..1
            string lowerString = "1";
            string upperString = "1";
            //debug
            if (this.WrappedAttribute.LowerBound.Length > 0)
            {
                lowerString = this.wrappedAttribute.LowerBound;
            }
            if (this.WrappedAttribute.UpperBound.Length > 0)
            {
                upperString = this.wrappedAttribute.UpperBound;
            }
            return new Multiplicity(lowerString, upperString);
        }
        public override List<UML.Classes.Kernel.Relationship> relationships
        {
            get
            {
                string selectRelationsSQL = @"select c.Connector_ID from t_connector c
							,t_attribute a where a.ea_guid = '" + this.wrappedAttribute.AttributeGUID + @"' 
							and c.StyleEx like '%LF_P=" + this.wrappedAttribute.AttributeGUID + "%'"
                                + @" and ((c.Start_Object_ID = a.Object_ID and c.End_Object_ID <> a.Object_ID)
							    or (c.Start_Object_ID <> a.Object_ID and c.End_Object_ID = a.Object_ID))";
                return this.EAModel.getRelationsByQuery(selectRelationsSQL).Cast<UML.Classes.Kernel.Relationship>().ToList();
            }
            set => throw new NotImplementedException();
        }
        public bool isDerived
        {
            get => (bool)this.getProperty(getPropertyNameName(), this.wrappedAttribute.IsDerived);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedAttribute.IsDerived);
        }

        public bool isDerivedUnion
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool isComposite
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string _default
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.AggregationKind aggregation
        {
            get => UML.Classes.Kernel.AggregationKind.none;
            set { /* do nothing */ }
        }


        public void setDefaultValue(string defaultStringValue)
        {
            this.defaultValue = this.EAModel.factory.createValueSpecificationFromString(defaultStringValue);
        }


        public HashSet<UML.Classes.Kernel.Property> redefinedProperties
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public HashSet<UML.Classes.Kernel.Property> subsettedProperties
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.Property opposite
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.Classifier classifier
        {
            get => this.type as UML.Classes.Kernel.Classifier;
            set => this.type = value;
        }

        public UML.Classes.Kernel.Class _class
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.Association owningAssociation
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.Association association
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.DataType datatype
        {
            get => this.type as UML.Classes.Kernel.DataType;
            set => throw new NotImplementedException();
        }

        public bool isUMLReadOnly
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }



        public HashSet<UML.Classes.Kernel.Classifier> featuringClassifiers
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool isLeaf
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public HashSet<UML.Classes.Kernel.RedefinableElement> redefinedElements
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public HashSet<UML.Classes.Kernel.Classifier> redefinitionContexts
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public String qualifiedName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.Namespace owningNamespace
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }



        public bool isOrdered
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool isUnique
        {
            get => !this.wrappedAttribute.AllowDuplicates;
            set => this.wrappedAttribute.AllowDuplicates = !value;
        }
        public UML.Classes.Kernel.ValueSpecification upperValue
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public UML.Classes.Kernel.ValueSpecification lowerValue
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public bool getIsNavigable() { throw new NotImplementedException(); }

        public List<UML.Classes.Dependencies.Dependency> clientDependencies
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public List<UML.Classes.Dependencies.Dependency> supplierDependencies
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool isNavigable
        {
            get => true;
            set { /* do nothing */ }
        }



    }
}
