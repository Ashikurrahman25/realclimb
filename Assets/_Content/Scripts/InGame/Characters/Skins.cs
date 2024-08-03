using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Content.InGame.Characters
{
	[CreateAssetMenu(menuName = "Skins")]
	public class Skins: ScriptableObject
	{
		[Serializable]
		public class Skin
		{
			public string SkinId;
			public List<Material> BodyMaterials;
			public List<Material> ArmMaterials;
			public List<Material> LegMaterials;
			public Sprite Icon;
			public int Cost = 100;
		}

		[SerializeField] private List<Skin> _skinsList;
		public List<Skin> SkinsList => _skinsList;
	}
}