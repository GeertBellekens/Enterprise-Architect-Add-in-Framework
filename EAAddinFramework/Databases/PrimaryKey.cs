
using System;
using System.Collections.Generic;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of PrimaryKey.
	/// </summary>
	public class PrimaryKey:Constraint, DB.PrimaryKey
	{

		
		public PrimaryKey(Table owner,Operation operation) : base(owner, operation)
		{
			
		}

	}
}
