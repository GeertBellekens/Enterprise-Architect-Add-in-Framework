using System;
using System.Collections.Generic;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    /// <summary>
    /// Description of ValueSpecification.
    /// </summary>
    public class ValueSpecification : UML.Classes.Kernel.ValueSpecification
    {
        #region Element implementation
        public void addStereotype(UML.Profiles.Stereotype stereotype)
        {
            throw new NotImplementedException();
        }
        public List<T> getRelationships<T>(bool outgoing = true, bool incoming = true) where T : UML.Classes.Kernel.Relationship
        {
            throw new NotImplementedException();
        }
        public HashSet<T> getUsingDiagrams<T>() where T : class, UML.Diagrams.Diagram
        {
            throw new NotImplementedException();
        }
        public UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath)
        {
            throw new NotImplementedException();
        }
        public HashSet<UML.Profiles.TaggedValue> getReferencingTaggedValues()
        {
            throw new NotImplementedException();
        }
        public HashSet<UML.Classes.Kernel.Element> ownedElements
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public HashSet<UML.Classes.Kernel.Comment> ownedComments
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public UML.Classes.Kernel.Element owner
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public HashSet<UML.Profiles.Stereotype> stereotypes
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public List<UML.Classes.Kernel.Relationship> relationships
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public HashSet<UML.Profiles.TaggedValue> taggedValues
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public UML.Diagrams.Diagram compositeDiagram
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public int position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        #endregion
        #region UMLItem implementation
        public void select()
        {
            throw new NotImplementedException();
        }
        public void open()
        {
            throw new NotImplementedException();
        }
        public void openProperties()
        {
            throw new NotImplementedException();
        }
        public void addToCurrentDiagram()
        {
            throw new NotImplementedException();
        }
        public void selectInCurrentDiagram()
        {
            throw new NotImplementedException();
        }
        public void delete()
        {
            throw new NotImplementedException();
        }
        public void save()
        {
            throw new NotImplementedException();
        }
        public List<UML.Diagrams.Diagram> getDependentDiagrams()
        {
            throw new NotImplementedException();
        }
        public bool makeWritable(bool overrideLocks)
        {
            throw new NotImplementedException();
        }

        public List<UML.Classes.Kernel.Element> getAllOwners()
        {
            throw new NotImplementedException();
        }

        public string name
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public string fqn => throw new NotImplementedException();
        public string uniqueID => throw new NotImplementedException();
        public bool isReadOnly => throw new NotImplementedException();
        #endregion
        #region TypedElement implementation
        public UML.Classes.Kernel.Type type
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        #endregion
        #region PackageableElement implementation
        public UML.Classes.Kernel.Package owningPackage
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        #endregion
        #region NamedElement implementation
        public UML.Classes.Kernel.VisibilityKind visibility
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public string qualifiedName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public UML.Classes.Kernel.Namespace owningNamespace
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
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

        public HashSet<UML.Classes.Kernel.Constraint> constraints
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }


        #endregion

    }
}
