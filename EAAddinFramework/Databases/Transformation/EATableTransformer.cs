using System;
using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA=TSF.UmlToolingFramework.Wrappers.EA;
using DB=DatabaseFramework;
using DB_EA = EAAddinFramework.Databases;
using EAAddinFramework.Utilities;

namespace EAAddinFramework.Databases.Transformation
{
	/// <summary>
	/// Description of EATableTransformer.
	/// </summary>
	public abstract class EATableTransformer:DB.Transformation.TableTransformer
	{

		#region TableTransformer implementation
		
		public DB.Table table {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public List<UML.Classes.Kernel.Class> logicalClasses {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public DB.Table transformLogicalClasses(List<UML.Classes.Kernel.Class> logicalClasses)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
