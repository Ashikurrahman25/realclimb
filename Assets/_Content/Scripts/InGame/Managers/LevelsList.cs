using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Content.InGame.Managers
{
	
	[CreateAssetMenu(menuName = "Levels List")]
	public class LevelsList: ScriptableObject
	{
		[SerializeField] private List<LevelInfo> _levels;

		public List<LevelInfo> Levels => _levels;
	}

	[Serializable]
	public class LevelInfo
	{
		[SerializeField] private string _levelName;
		[SerializeField] private int _levelId;
		[SerializeField] private string _sceneName;
		[SerializeField] private bool _canBeFoundInRandom;

		public bool CanBeFoundInRandom => _canBeFoundInRandom;
		public int LevelId => _levelId;
		public string LevelName => _levelName;
		public string SceneName => _sceneName;
	}
}