
using System;
using WT=WorkTrackingFramework;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using TSF.UmlToolingFramework.Wrappers.EA;


namespace EAAddinFramework.WorkTracking
{
	/// <summary>
	/// Description of WorkItem.
	/// </summary>
	public abstract class WorkItem:WT.Workitem
	{
		ElementWrapper _wrappedElement;
		protected string _type = string.Empty;
		protected string _ID = string.Empty;
		protected string _description = string.Empty;
		internal ElementWrapper wrappedElement {
			get {
				return _wrappedElement;
			}
			set {
				_wrappedElement = value;
				this.resetProperties();
			}
		}
		/// <summary>
		/// default constructor
		/// </summary>
		protected WorkItem(WT.Project ownerProject)
		{
			this.ownerProject = ownerProject;
		}
		/// <summary>
		/// constructuro with ElementWrapper
		/// </summary>
		/// <param name="wrappedElement"></param>
		protected WorkItem(WT.Project ownerProject,ElementWrapper wrappedElement):this(ownerProject)
		{
			this._wrappedElement = wrappedElement;
		}

		void resetProperties()
		{
			this.area = _area;
			this.assignedTo = _assignedTo;
			this.description = _description;
			this.ID = _ID;
			this.iteration = _iteration;
			this.state = _state;
			this.type = _type;
		}

		public void save()
		{
			if (this.wrappedElement != null)
			{
				this.wrappedElement.save();
			}
		}
		/// <summary>
		/// if it doesn't exist yet then we create it new, if it already exists then we update it
		/// </summary>
		/// <param name="ownerPackage">the package where the new items should be created</param>
		/// <param name = "elementType">the type of element to create for new items</param>
		public virtual bool synchronizeToEA(Package ownerPackage,string elementType)
		{
			//first check if it exists already
			string sqlGetExistingElement = @"select o.Object_ID from (t_object o 
											inner join t_objectproperties tv on o.Object_ID = tv.Object_ID)
											where tv.[Property] = 'TFS_ID'
											and tv.[Value] = '"+ this.ID +"'";
			//TODO: get elements by query
			var elementToWrap = ownerPackage.EAModel.getElementWrappersByQuery(sqlGetExistingElement).FirstOrDefault();
			if (elementToWrap == null)
			{
				//element does not exist, create a new one
				elementToWrap = ownerPackage.addOwnedElement<ElementWrapper>(this.title,elementType);
			}
			if (elementToWrap != null)
			{
				this.wrappedElement = elementToWrap;
				this.save();
			}
			//return if this worked
			return elementToWrap != null;
		}

		internal Project _ownerProject;
		public virtual WT.Project ownerProject 
		{
			get 
			{
				return _ownerProject;
			}
			set 
			{
				_ownerProject = (Project)value;
			}
		}

		#region Workitem implementation
		
		string _title = string.Empty;
		/// <summary>
		/// mapped to name of wrapped element
		/// </summary>
		public string title 
		{
			get 
			{
				if (this.wrappedElement != null)
				{
					_title = this.wrappedElement.name;
				}
				return _title;
			}
			set 
			{
				if (this.wrappedElement != null)
				{
					this.wrappedElement.name = value;
				}
				_title = value;
			}
		}

		public abstract string ID {get;set;}
		public abstract string type{get;set;}
		
		string _state = string.Empty;
		/// <summary>
		/// mapped to status of wrapped element
		/// </summary>
		public string state 
		{
			get 
			{
				if (this.wrappedElement != null)
				{
					_state = this.wrappedElement.status;
				}
				return _state;
			}
			set 
			{
				if (this.wrappedElement != null)
				{
					this.wrappedElement.status = value;
				}
				_state = value;
			}
		}

		
		public abstract string description {get;set;}
		string _assignedTo = string.Empty;
		/// <summary>
		/// mapped to Tagged Value TFS_AssignedTo
		/// </summary>
		public string assignedTo 
		{
			get 
			{
				if (this.wrappedElement != null)
				{
					var typeTag = this.wrappedElement.getTaggedValue("TFS_AssignedTo");
					if (typeTag != null)
					{
						_assignedTo = typeTag.tagValue.ToString();
					}
				}
				return _assignedTo;
			}
			set 
			{
				if (this.wrappedElement != null)
				{
					this.wrappedElement.addTaggedValue("TFS_AssignedTo",value);
				}
				_assignedTo = value;
			}
		}

		string _area = string.Empty;
		/// <summary>
		/// mapped to Tagged Value TFS_Area
		/// </summary>
		public string area 
		{
			get 
			{
				if (this.wrappedElement != null)
				{
					var typeTag = this.wrappedElement.getTaggedValue("TFS_Area");
					if (typeTag != null)
					{
						_area = typeTag.tagValue.ToString();
					}
				}
				return _area;
			}
			set 
			{
				if (this.wrappedElement != null)
				{
					this.wrappedElement.addTaggedValue("TFS_Area",value);
				}
				_area = value;
			}
		}

		string _iteration = string.Empty;
		/// <summary>
		/// mapped to phase of wrapped element
		/// </summary>
		public string iteration 
		{
			get 
			{
				if (this.wrappedElement != null)
				{
					_iteration = this.wrappedElement.phase;
				}
				return _iteration;
			}
			set 
			{
				if (this.wrappedElement != null)
				{
					this.wrappedElement.phase = value;
				}
				_iteration = value;
			}
		}

		#endregion
	}
}
