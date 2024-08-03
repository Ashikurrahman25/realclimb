using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Content.InGame.Level
{
	[CreateAssetMenu(menuName = "Bioms")]
	public class Bioms: ScriptableObject
	{
		[Serializable]
		public class BiomInfo
		{
			public int BiomId;
			public Sprite BiomSprite;
		}
		[SerializeField] private List<BiomInfo> _biomInfos;

		public List<BiomInfo> BiomInfos => _biomInfos;
	}
}