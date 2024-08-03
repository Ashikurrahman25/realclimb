using System;
using System.Collections.Generic;
using _Content.Data;
using _Content.InGame.Managers;
using Common.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Content.InGame.UI.Main
{
	public class LevelProgressionUi: UIViewWrapper
	{
		[Serializable]
		private class LevelProgressionUiInfo
		{
			public TextMeshProUGUI LevelNumber;
			public Image LevelBackground;
		}
		[SerializeField] private List<LevelProgressionUiInfo> _levels;
		[SerializeField] private Color _currentColor;
		[SerializeField] private Color _nonCurrentColor;
		
		public override void ShowView(bool force = false)
		{
			base.ShowView(force);
			UpdateInformation();
		}

		private void UpdateInformation()
		{
			var currentLevelNumber = PlayerData.Instance.LevelNumberThroughLoops;
			var levelNumberPerProgression = GameManager.Instance.LevelsInProgression;
			var startNumber = (currentLevelNumber - 1)/ levelNumberPerProgression * levelNumberPerProgression + 1;
			for (int i = 0; i < levelNumberPerProgression; i++)
			{
				var levelInfo = _levels[i];
				var number = startNumber + i;
				levelInfo.LevelNumber.text = $"{startNumber+i}";
				var color = number == currentLevelNumber ? _currentColor : _nonCurrentColor;
				levelInfo.LevelBackground.color = color;
			}
		}
	}
}