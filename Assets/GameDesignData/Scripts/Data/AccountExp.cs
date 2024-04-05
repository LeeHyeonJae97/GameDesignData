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
	public string Key1 { get; private set; }

	/// <summary>
	/// 레벨
	/// </summary>
	public int[] Level1 { get; private set; }

	/// <summary>
	/// 필요 경험치
	/// </summary>
	public int NeedExp1 { get; private set; }

	/// <summary>
	/// 아이템 타입
	/// </summary>
	public ItemType ItemType { get; private set; }

	public AccountExp(string key1, int[] level1, int needExp1, ItemType itemType)
	{
		Key1 = key1;
		Level1 = level1;
		NeedExp1 = needExp1;
		ItemType = itemType;
	}
}
