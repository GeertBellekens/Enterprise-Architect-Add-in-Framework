using System;
using System.Collections.Generic;
using System.Linq;

using EAAddinFramework.Utilities;
using UML = TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    /// <summary>
    /// Description of Part.
    /// </summary>
    public class Property : ElementWrapper, UML.CompositeStructures.InternalStructures.Property
    {
        public Property(Model model, global::EA.Element wrappedElement)
          : base(model, wrappedElement)
        {
        }

        public bool getIsNavigable()
        {
            throw new NotImplementedException();
        }

        public bool isDerived
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

        public bool isDerivedUnion
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

        public bool isComposite
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

        public string _default
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

        public UML.Classes.Kernel.AggregationKind aggregation
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

        public UML.Classes.Kernel.ValueSpecification defaultValue
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

        public HashSet<UML.Classes.Kernel.Property> redefinedProperties
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

        public HashSet<UML.Classes.Kernel.Property> subsettedProperties
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

        public UML.Classes.Kernel.Property opposite
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

        public UML.Classes.Kernel.Classifier classifier
        {
            get
            {
                return this.EAModel.getElementWrapperByGUID(this.wrappedElement.MiscData[0].ToString()) as UML.Classes.Kernel.Classifier;

            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public UML.Classes.Kernel.Class _class
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

        public UML.Classes.Kernel.Association owningAssociation
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

        public UML.Classes.Kernel.Association association
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

        public UML.Classes.Kernel.DataType datatype
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

        public bool isID
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

        public bool isNavigable
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

        public UML.CompositeStructures.InternalStructures.ConnectorEnd end
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

        public bool isUMLReadOnly
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

        public bool isOrdered
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

        public bool isUnique
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

        public UML.Classes.Kernel.UnlimitedNatural upper
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

        public uint lower
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

        public UML.Classes.Kernel.ValueSpecification upperValue
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

        public UML.Classes.Kernel.ValueSpecification lowerValue
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

        public UML.Classes.Kernel.Multiplicity multiplicity
        {
            get
            {
                if (string.IsNullOrEmpty(this.wrappedElement.Multiplicity))
                {
                    return Attribute.defaultMultiplicity();
                }

                string multiplicityString = this.wrappedElement.Multiplicity;

                if (multiplicityString.StartsWith(".."))
                {
                    multiplicityString = "1" + multiplicityString;
                }
                if (multiplicityString.EndsWith(".."))
                {
                    multiplicityString = multiplicityString + "1";
                }
                return new Multiplicity(multiplicityString);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public UML.Classes.Kernel.Type type
        {
            get
            {
                return null;
            }
            set
            {
                //do nothing
            }
        }

        public bool isStatic
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

        public HashSet<UML.Classes.Kernel.Classifier> featuringClassifiers
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

        public bool isLeaf
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

        public HashSet<UML.Classes.Kernel.RedefinableElement> redefinedElements
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

        public HashSet<UML.Classes.Kernel.Classifier> redefinitionContexts
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
    }
}
