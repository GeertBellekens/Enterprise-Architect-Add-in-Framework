using System;
using System.Collections.Generic;
using UML = TSF.UmlToolingFramework.UML;



namespace TSF.UmlToolingFramework.Wrappers.EA
{
    /// <summary>
    /// Description of DataType.
    /// </summary>
    public class InstanceSpecification : ElementWrapper, UML.Classes.Kernel.InstanceSpecification
    {

        public InstanceSpecification(Model model, global::EA.Element elementToWrap)
      : base(model, elementToWrap)
        { }

        //TODO: implement this to represent the runstate
        public HashSet<UML.Classes.Kernel.Slot> slots { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<UML.Classes.Kernel.InstanceValue> instanceValues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // This will help in getting to the properties of the authorization object.
        public HashSet<UML.Classes.Kernel.Classifier> classifiers
        {
            get
            {
                return new HashSet<UML.Classes.Kernel.Classifier> {
                    this.classifier as UML.Classes.Kernel.Classifier
                };
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        UML.Classes.Kernel.ValueSpecification UML.Classes.Kernel.InstanceSpecification.specification { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
            throw new NotImplementedException();
        }

        public UML.Classes.Kernel.UnlimitedNatural unlimitedValue()
        {
            throw new NotImplementedException();
        }


    }
}
