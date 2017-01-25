using System.Collections.Generic;
using System.Linq;
using System;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of MappingSettings.
	/// </summary>
	public interface MappingSettings
	{
		
		/// <summary>
		/// use tagged values as a way to map elements.
		/// If false we use relations with "link to element feature"
		/// </summary>
		bool useTaggedValues {get;set;}
	    
        /// <summary>
        /// the tagged value to use for attributes when using tagged values for the links (only for newly created items)
        /// </summary>
		string linkedAttributeTagName {get;set;}
		
		/// <summary>
		/// the tagge value to use for associations when using tagged values for the links (only for newly created items)
		/// </summary>
        string linkedAssociationTagName {get;set;}
        
        /// <summary>
		/// indicates that we use inline mapping logic (only description) in the comments of the tagged value when adding mapping logic.
		/// this only applies when using tagged values for mapping. (only for newly created items)
		/// </summary>
        bool useInlineMappingLogic {get;set;}
	    
       	/// <summary>
		/// the (EA) type of element to use for the mapping logic (only for newly created items)
		/// </summary>
        string mappingLogicType {get;set;}
        
        /// <summary>
		/// the name of the output log
		/// </summary>
        string outputName {get;set;}
        
    }
		
}
