
using System;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// List of databses supported as backend for an EA repository
	/// 0 - MYSQL
	///	1 - SQLSVR
	/// 2 - ADOJET
	/// 3 - ORACLE
	/// 4 - POSTGRES
	/// 5 - ASA
	/// 8 - ACCESS2007
	/// 9 - FireBird
	/// </summary>
	public enum RepositoryType
	{
		MYSQL,
		SQLSVR,
		ADOJET,
		ORACLE,
		POSTGRES,
		ASA,
		ACCESS2007,
		FIREBIRD
	}
}
