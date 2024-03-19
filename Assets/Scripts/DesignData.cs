using MemoryPack;
using System;
using UnityEngine;

/// <summary>
/// 계정 레벨 별 경험치
/// </summary>
[MemoryPackable]
public partial class AccountExp
{
	/// <summary>
	/// 키
	/// </summary>
	public string Key { get; private set; }

	/// <summary>
	/// 레벨
	/// </summary>
	public int Level { get; private set; }

	/// <summary>
	/// 필요 경험치
	/// </summary>
	public int NeedExp { get; private set; }

	public AccountExp(string key, int level, int needExp)
	{
		Key = key;
		Level = level;
		NeedExp = needExp;
	}
}
