
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
	public class WorkItem:WT.Workitem
	{
		ElementWrapper _wrappedElement;
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
		public void synchronizeToEA(Package ownerPackage,string elementType)
		{
			//first check if it exists already
			string sqlGetExistingElement = @"select o.Object_ID from (t_object o 
											inner join t_objectproperties tv on o.Object_ID = tv.Object_ID)
											where tv.[Property] = 'TFS_ID'
											and tv.[Value] = '"+ this.ID +"'";
			//TODO: get elements by query
			var elementToWrap = ownerPackage.model.getElementWrappersByQuery(sqlGetExistingElement).FirstOrDefault();
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
		string _ID;
		/// <summary>
		/// mapped to tagged value TFS_ID
		/// </summary>
		public string ID 
		{
			get 
			{
				if (this.wrappedElement != null)
				{
					
					var IDTag = this.wrappedElement.getTaggedValue("TFS_ID");
					if (IDTag != null)
					{
						_ID = IDTag.tagValue.ToString();
					}
				}
				return _ID;
			}
			set 
			{
				if (this.wrappedElement != null)
				{
					this.wrappedElement.addTaggedValue("TFS_ID",value);
				}
				_ID = value;
			}
		}
		string _type;
		/// <summary>
		/// mapped to tagged value TFS_type
		/// </summary>
		public string type 
		{
			get 
			{
				if (this.wrappedElement != null)
				{
					var typeTag = this.wrappedElement.getTaggedValue("TFS_type");
					if (typeTag != null)
					{
						_type = typeTag.tagValue.ToString();
					}
				}
				return _type;
			}
			set 
			{
				if (this.wrappedElement != null)
				{
					this.wrappedElement.addTaggedValue("TFS_type",value);
				}
				_type = value;
			}
		}
		string _title;
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

		string _state;
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

		string _description;
		/// <summary>
		/// mapped to notes of wrapped element
		/// </summary>
		public string description 
		{
			get 
			{
				if (this.wrappedElement != null)
				{
					_description = this.wrappedElement.notes;
				}
				return _description;
			}
			set 
			{
				if (this.wrappedElement != null)
				{
					this.wrappedElement.notes = value;
				}
				_description = value;
			}
		}
		string _assignedTo;
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

		string _area;
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

		string _iteration;
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
				return _description;
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
