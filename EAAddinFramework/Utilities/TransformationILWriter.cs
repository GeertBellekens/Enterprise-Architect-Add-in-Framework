using System;
using System.Collections.Generic;
using System.Collections.Specialized;
// using System.Diagnostics.PerformanceData;
using System.Drawing.Text;
using System.IO;
using TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.UML.StateMachines.BehaviorStateMachines;
using UML = TSF.UmlToolingFramework.UML;
using UML_SM = TSF.UmlToolingFramework.UML.StateMachines.BehaviorStateMachines;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Utilities
{

	/// <summary>
	/// The connector types available for Enterprise Architect transformation IL.
	/// </summary>
	public enum ConnectorType
	{
		/// <summary>
		/// Aggregation
		/// </summary>
		Aggregation ,
		/// <summary>
		/// Assembly
		/// </summary>
		Assembly ,
		/// <summary>
		/// Association
		/// </summary>
		Association ,
		/// <summary>
		/// Collaboration
		/// </summary>
		Collaboration ,
		/// <summary>
		/// ControlFlow
		/// </summary>
		ControlFlow ,
		/// <summary>
		/// Connector
		/// </summary>
		Connector ,
		/// <summary>
		/// Delegate
		/// </summary>
		Delegate ,
		/// <summary>
		/// Dependency
		/// </summary>
		Dependency ,
		/// <summary>
		/// Deployment
		/// </summary>
		Deployment ,
		/// <summary>
		/// ForeignKey
		/// </summary>
		ForeignKey ,
		/// <summary>
		/// Generalization
		/// </summary>
		Generalization ,
		/// <summary>
		/// InformationFlow
		/// </summary>
		InformationFlow ,
		/// <summary>
		/// Instantiation
		/// </summary>
		Instantiation ,
		/// <summary>
		/// Interface
		/// </summary>
		Interface ,
		/// <summary>
		/// InterruptFlow
		/// </summary>
		InterruptFlow ,
		/// <summary>
		/// Manifest
		/// </summary>
		Manifest ,
		/// <summary>
		/// Nesting
		/// </summary>
		Nesting ,
		/// <summary>
		/// NoteLink
		/// </summary>
		NoteLink ,
		/// <summary>
		/// ObjectFlow
		/// </summary>
		ObjectFlow ,
		/// <summary>
		/// Package
		/// </summary>
		Package ,
		/// <summary>
		/// Realization
		/// </summary>
		Realization ,
		/// <summary>
		/// Sequence
		/// </summary>
		Sequence ,
		/// <summary>
		/// Transition
		/// </summary>
		Transition ,
		/// <summary>
		/// UseCase
		/// </summary>
		UseCase ,
		/// <summary>
		/// Uses
		/// </summary>
		Uses ,
	};		

	/// <summary>
	/// The TransformationILWriter to write common EA transformation intermediate language elements to any underlying 
	/// System.IO.TextWriter. In fact it implements kind of a little DSL for Enterprise Architects transformation language.
	/// </summary>
	public class TransformationILWriter
	{
		System.IO.TextWriter writer;
		int levelIndent;
		string indentString;
		
		/// <summary>
		/// Creates a new TransformationILWTextWriter instance.
		/// </summary>
		/// <param name="writer">The System.IO.TextWriter to use for output.</param>
		/// <param name="initialIndent">The initial indent level.</param>
		public TransformationILWriter(System.IO.TextWriter writer,int initialIndent = 0)
		{
			this.writer = writer;
			levelIndent = initialIndent;
			indentString = GetLevelIndent();
		}

		/// <summary>
		/// Increases the current indent level.
		/// </summary>
		/// <param name="increment">The number of indent levels to insert.</param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter Indent(int increment = 1)
		{
			levelIndent += increment;
			indentString = GetLevelIndent();
			return this;
		}
		
		/// <summary>
		/// Opens an IL 'Interface' declaration at the current indent level and increases the indent level.
		/// <code>
		/// Interface
		/// {
		/// </code>
		/// </summary>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter OpenILInterface()
		{
			WriteIndented("Interface");
			WriteIndented("{");
			Indent();
			return this;
		}

		/// <summary>
		/// Opens an IL 'Class' declaration at the current indent level and increases the indent level.
		/// <code>
		/// Class
		/// {
		/// </code>
		/// </summary>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter OpenILClass()
		{
			WriteIndented("Class");
			WriteIndented("{");
			Indent();
			return this;
		}

		/// <summary>
		/// Opens an IL 'Package' declaration at the current indent level and increases the indent level.
		/// <code>
		/// Package
		/// {
		/// </code>
		/// </summary>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter OpenILPackage()
		{
			WriteIndented("Package");
			WriteIndented("{");
			Indent();
			return this;
		}
		
		/// <summary>
		/// Opens an IL 'XRef' declaration at the current indent level and increases the indent level.
		/// <code>
		/// XRef
		/// {
		/// </code>
		/// </summary>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter OpenILXRef( )
		{
			WriteIndented("XRef");
			WriteIndented("{");
			Indent();
			return this;
		}
		
		/// <summary>
		/// Opens an IL connector of type connectorType declaration at the current indent level and increases the indent level.
		/// <code>
		/// [connectorType]
		/// {
		/// </code>
		/// </summary>
		/// <param name="connectorType">The IL connector type to declare</param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter OpenILConnector(ConnectorType connectorType)
		{
			return OpenILConnectorRaw(connectorType.ToString());
		}

		/// <summary>
		/// Opens an IL connector of type connectorType declaration at the current indent level and increases the indent level.
		/// <code>
		/// [connectorType]
		/// {
		/// </code>
		/// </summary>
		/// <param name="connectorType">The IL connector type to declare</param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter OpenILConnectorRaw(string connectorType)
		{
			WriteIndented("{0}",connectorType);
			WriteIndented("{");
			Indent();
			return this;
		}
		
		/// <summary>
		/// Opens an IL 'Operation' declaration at the current indent level and increases the indent level.
		/// <code>
		/// Operation
		/// {
		/// </code>
		/// </summary>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter OpenILOperation( )
		{
			WriteIndented("Operation");
			WriteIndented("{");
			Indent();
			return this;
		}

		/// <summary>
		/// Opens an IL 'Parameter' declaration at the current indent level and increases the indent level.
		/// <code>
		/// Parameter
		/// {
		/// </code>
		/// </summary>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter OpenILParameter( )
		{
			WriteIndented("Parameter");
			WriteIndented("{");
			Indent();
			return this;
		}

		/// <summary>
		/// Opens an IL 'Attribute' declaration at the current indent level and increases the indent level.
		/// <code>
		/// Attribute
		/// {
		/// </code>
		/// </summary>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter OpenILAttribute( )
		{
			WriteIndented("Attribute");
			WriteIndented("{");
			Indent();
			return this;
		}

		/// <summary>
		/// Decreases the indent level and closes the actual IL declaration scope.
		/// </summary>
		/// <code>
		/// }
		/// </code>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter CloseIL( )
		{
			Indent(-1);
			WriteIndented("}");
			writer.WriteLine();
			return this;
		}

		/// <summary>
		/// Writes an IL tagged value declaration at the current indent level.
		/// <code>
		/// Tag
		/// {
		///     name = "[tagName]"
		///     value = "[tagValue]"
		/// }
		/// </code>
		/// </summary>
		/// <param name="tagName">The tagged value name.</param>
		/// <param name="tagValue">The tagged values value.</param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter WriteILTag(string tagName, string tagValue)
		{
			WriteIndented("Tag");
			WriteIndented("{");
				Indent();
				WriteILProperty("name",tagName);
				if(!string.IsNullOrWhiteSpace(tagValue)) {
					WriteILProperty("value",tagValue);
				}
				Indent(-1);
			WriteIndented("}");
			return this;
		}
		
		/// <summary>
		/// Writes the IL declarations for a packages standard properties.
		/// </summary>
		/// <param name="package">The UML model package instance that declares the properties to generate.</param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter WriteStdPackageProperties(UML.Classes.Kernel.Package package)
		{
			WriteILProperty("name",package.name);
			//WriteILProperty( levelIndent,"language","C++");
			return this;
		}

		/// <summary>
		/// Writes the IL declarations for a UML model transitions standard properties.
		/// </summary>
		/// <param name="relationship"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter WriteStdConnectorProperties(UML_SM.Transition relationship)
		{
			WriteILProperty("direction","Source -> Destination");
			GenerateXRef("Connector",relationship as UTF_EA.ConnectorWrapper);
			return this;
		}

		/// <summary>
		/// Writes a connector source at the current indent level.
		/// </summary>
		/// <param name="connectorWrapper"></param>
		/// <param name="addProperties"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter WriteILConnectorSource(UTF_EA.ConnectorWrapper connectorWrapper, StringDictionary addProperties = null)
		{
			if(connectorWrapper != null) {
				WriteIndented("source");
				WriteIndented("{");
					Indent();
					if(addProperties != null) {
						foreach(string propertyName in addProperties.Keys) {
							WriteILProperty(propertyName,addProperties[propertyName]);
						}
					}									
					GenerateXRef(connectorWrapper.source);
					Indent(-1);
				WriteIndented("}");
			}
			return this;
		}

		/// <summary>
		/// Writes a relationships connector source from using an UML model element.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="sourceRef"></param>
		/// <param name="addProperties"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter WriteILConnectorSource(string type, UML.Classes.Kernel.Element sourceRef, StringDictionary addProperties = null)
		{
			WriteIndented("source");
			WriteIndented("{");
				Indent();
				if(addProperties != null) {
					foreach(string propertyName in addProperties.Keys) {
						WriteILProperty(propertyName,addProperties[propertyName]);
					}
				}
				GenerateXRef(type,sourceRef);
				Indent(-1);
			WriteIndented("}");
			return this;
		}

		/// <summary>
		/// Writes a transitions connector source at the current indent level.
		/// </summary>
		/// <param name="connectorWrapper"></param>
		/// <param name="addProperties"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter WriteILConnectorTarget(UTF_EA.ConnectorWrapper connectorWrapper, StringDictionary addProperties = null)
		{
			if(connectorWrapper != null) {
				WriteIndented("target");
				WriteIndented("{");
					Indent();
					if(addProperties != null) {
						foreach(string propertyName in addProperties.Keys) {
							WriteILProperty(propertyName,addProperties[propertyName]);
						}
					}
					GenerateXRef(connectorWrapper.supplier);
					Indent(-1);
				WriteIndented("}");
			}
			return this;
		}
		
		/// <summary>
		/// Writes a relationships connector target from using an UML model element.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="targetRef"></param>
		/// <param name="addProperties"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter WriteILConnectorTarget(string type, UML.Classes.Kernel.Element targetRef, StringDictionary addProperties = null)
		{
			WriteIndented("target");
			WriteIndented("{");
				Indent();
				if(addProperties != null) {
					foreach(string propertyName in addProperties.Keys) {
						WriteILProperty(propertyName,addProperties[propertyName]);
					}
				}
				GenerateXRef(type, targetRef);
				Indent(-1);
			WriteIndented("}");
			return this;
		}

		/// <summary>
		/// Writes an IL property declaration at the current indentation level.
		/// </summary>
		/// <code>
		/// [propertyName] = "[propertyValue]"
		/// </code>
		/// <param name="propertyName"></param>
		/// <param name="propertyValue"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter WriteILProperty(string propertyName, string propertyValue)
		{
			WriteIndented("{0} = \"{1}\"",propertyName,propertyValue);
			return this;
		}
		
		/// <summary>
		/// Writes an IL 'Stereotype' declaration at the current indentation level.
		/// </summary>
		/// <code>
		/// Stereotype = "[propertyValue]"
		/// </code>
		/// <param name="stereotype"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter WriteILStereotype(string stereotype)
		{
			WriteIndented("Stereotype = \"{0}\"",stereotype);
			return this;
		}

		/// <summary>
		/// Writes an IL comment.
		/// </summary>
		/// <param name="comment"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		/// <remarks>
		/// The Enterprise Architect transformation IL doesn't seem to support comments that will
		/// be ignored on transformations. So this method does actually nothing.
		/// </remarks>
		public TransformationILWriter WriteILComment(string comment)
		{
			return WriteILComment(comment,null);
		}
		
		/// <summary>
		/// Writes an IL comment from a fomat specification.
		/// </summary>
		/// <param name="comment"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		/// <remarks>
		/// The Enterprise Architect transformation IL doesn't seem to support comments that will
		/// be ignored on transformations. So this method does actually nothing.
		/// </remarks>
		public TransformationILWriter WriteILComment(string comment, params object[] args)
		{
			// TODO: Check out if representation as note might be useful.
			return this;
		}
		
		/// <summary>
		/// Writes an error message to the current transformation IL output. This will stop the transformation at this point
		/// and can be inspected in a saved transformation IL file.
		/// </summary>
		/// <param name="error"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter WriteILError(string error)
		{
			return WriteILError(error,null);
		}
		
		/// <summary>
		/// Writes a formatted error message to the current transformation IL output. This will stop the transformation at this point
		/// and can be inspected in a saved transformation IL file.
		/// </summary>
		/// <param name="error"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter WriteILError(string error, params object[] args)
		{
			try
			{
				writer.Write("*** ERROR: ");
				if(args != null) {
					writer.Write(error, args);
				}
				else {
					writer.Write(error);
				}
				writer.WriteLine();
			}
			catch
			{
			}

			return this;
		}
		
		/// <summary>
		/// Writes an IL 'XRef' declaration for element at the current indentation level.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="elementWrapper"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter GenerateXRef(UML.Classes.Kernel.Element element)
		{
			UTF_EA.ElementWrapper elementWrapper = element as UTF_EA.ElementWrapper;
			if(elementWrapper != null) {
				string sourceGuid = GetElementGuid(elementWrapper);
				return GenerateXRef(element.name,sourceGuid);
			}
			return this;
		}
		
		/// <summary>
		/// Writes an IL 'XRef' declaration for element at the current indentation level.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="elementWrapper"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter GenerateXRef(string name, UML.Classes.Kernel.Element element)
		{
			UTF_EA.ElementWrapper elementWrapper = element as UTF_EA.ElementWrapper;
			if(elementWrapper != null) {
				string sourceGuid = GetElementGuid(elementWrapper);
				return GenerateXRef(name,sourceGuid);
			}
			return this;
		}
		
		/// <summary>
		/// Writes an IL 'XRef' declaration for refName and sourceGuid at the current indentation level.
		/// </summary>
		/// <param name="refName"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter GenerateXRef(string refName, string sourceGuid)
		{
			OpenILXRef()
				.WriteILProperty("namespace","C++STTCL")
				.WriteILProperty("name",refName)
				.WriteILProperty("source",sourceGuid)
			.CloseIL();
			return this;
		}
		
		/// <summary>
		/// Writes an IL 'XRef' declaration for connectorWrapper at the current indentation level.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="connectorWrapper"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter GenerateXRef(string name, UTF_EA.ConnectorWrapper connectorWrapper)
		{
			string sourceGuid = GetConnectorGuid(connectorWrapper);
			OpenILXRef()
				.WriteILProperty("namespace","C++STTCL")
				.WriteILProperty("name",name)
				.WriteILProperty("source",sourceGuid)
			.CloseIL();
			return this;
		}
		
		/// <summary>
		/// Writes the standard IL 'Class' properties at the current indentation level.
		/// </summary>
		/// <param name="umlItem"></param>
		/// <returns>The TransformationILWriter instance for use in concatenated output.</returns>
		public TransformationILWriter GenerateClassProperties(UML.Extended.UMLItem umlItem)
		{
			WriteILProperty("name",umlItem.name);
			//WriteILProperty(levelIndent,"language","C++");
			return this;
		}
						
		private string GetElementGuid(UTF_EA.ElementWrapper elementWrapper)
		{
			return elementWrapper.WrappedElement.ElementGUID;
		}
 
		private string GetConnectorGuid(UTF_EA.ConnectorWrapper connectorWrapper)
		{
			return connectorWrapper.WrappedConnector.ConnectorGUID;
		}

		private string GetLevelIndent()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for(int i = 0; i < levelIndent; ++i) {
				sb.Append("\t");
			}
			return sb.ToString();
		}
		
		private void WriteIndented(string s)
		{
			WriteIndented("{0}",s);
		}

		private void WriteIndented(string format, params object[] args)
		{
			writer.Write(indentString);
			writer.WriteLine(format,args);
		}
	}
}
