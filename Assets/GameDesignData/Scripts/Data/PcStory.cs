using MemoryPack;
using System;
using UnityEngine;

/// <summary>
/// 캐릭터 이야기 테이블
/// </summary>
[MemoryPackable]
public partial class PcStory
{
	/// <summary>
	/// 고유식별번호
	/// </summary>
	public string Key { get; private set; }

	/// <summary>
	/// 이야기 고유 번호
	/// </summary>
	public int StoryNo { get; private set; }

	/// <summary>
	/// 이야기 파일 이름
	/// </summary>
	public string StoryFileName { get; private set; }

	/// <summary>
	/// 이야기 설명
	/// </summary>
	public string StoryDesc { get; private set; }

	/// <summary>
	/// 이야기 파일 이미지
	/// </summary>
	public string StoryFileCharIcon { get; private set; }

	/// <summary>
	/// 이야기 파일 번호
	/// </summary>
	public string StoryFileNo { get; private set; }

	/// <summary>
	/// 연결된 후속 이야기 파일 키
	/// </summary>
	public string StoryFile_Next { get; private set; }

	/// <summary>
	/// 이야기 시작 대화
	/// </summary>
	public string StartStoryDialog { get; private set; }

	/// <summary>
	/// 이야기 종료 대화
	/// </summary>
	public string EndStoryDialog { get; private set; }

	/// <summary>
	/// 개방 스테이지
	/// </summary>
	public string DgStageOpen { get; private set; }

	/// <summary>
	/// 이야기 개방 조건1
	/// </summary>
	public DLG_START_TRIGGER_TYPE DgStartTriggerType1 { get; private set; }

	/// <summary>
	/// 이야기 개방 조건 값1
	/// </summary>
	public string DgStartTriggerValue1 { get; private set; }

	public PcStory(string key, int storyNo, string storyFileName, string storyDesc, string storyFileCharIcon, string storyFileNo, string storyFile_Next, string startStoryDialog, string endStoryDialog, string dgStageOpen, DLG_START_TRIGGER_TYPE dgStartTriggerType1, string dgStartTriggerValue1)
	{
		Key = key;
		StoryNo = storyNo;
		StoryFileName = storyFileName;
		StoryDesc = storyDesc;
		StoryFileCharIcon = storyFileCharIcon;
		StoryFileNo = storyFileNo;
		StoryFile_Next = storyFile_Next;
		StartStoryDialog = startStoryDialog;
		EndStoryDialog = endStoryDialog;
		DgStageOpen = dgStageOpen;
		DgStartTriggerType1 = dgStartTriggerType1;
		DgStartTriggerValue1 = dgStartTriggerValue1;
	}
}
