using System;
using System.Collections.Generic;
using _Content.Events;
using _Content.InGame.Managers;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using EventHandler = Opsive.Shared.Events.EventHandler;

namespace _Content.InGame.Characters
{
	public class SkinHandler : MonoBehaviour
	{
		[Serializable]
		private class SkinHat
		{
			[Dropdown("GetVectorValues")] public string SkinId;
			public GameObject HatObject;

			private DropdownList<string> GetVectorValues()
			{
#if UNITY_EDITOR
				var result = new DropdownList<string>();
				string[] assetNames = AssetDatabase.FindAssets("t:Skins", new[] { "Assets/_Content" });
				foreach (string SOName in assetNames)
				{
					var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
					var skins = AssetDatabase.LoadAssetAtPath<Skins>(SOpath);
					foreach (var skin in skins.SkinsList)
					{
						result.Add(skin.SkinId, skin.SkinId);
					}
				}

				if (assetNames.Length == 0)
				{
					result.Add("Empty", "Empty");
				}

				return result;
#else
				return new DropdownList<string>();
#endif
			}
		}

		[SerializeField] private List<SkinHat> _hatSkin;
		[SerializeField] private SkinnedMeshRenderer _bodyRenderer;
		[SerializeField] private List<SkinnedMeshRenderer> _armRenderers;
		[SerializeField] private List<SkinnedMeshRenderer> _legRenderers;

		[Space] [Header("Editor")] [SerializeField] [Dropdown("GetSkinsList")]
		private string _skinToSet;

		private void OnEnable()
		{
			EventHandler.RegisterEvent(InGameEvents.CurrentSkinChanged, OnCurrentSkinChanged);
		}

		private void OnDisable()
		{
			EventHandler.UnregisterEvent(InGameEvents.CurrentSkinChanged, OnCurrentSkinChanged);
		}

		private void OnCurrentSkinChanged()
		{
			SetSkin(GameManager.Instance.GetCurrentSkin());
		}

		public void SetSkin(Skins.Skin skin)
		{
			foreach (var hat in _hatSkin)
			{
				if (hat.SkinId == skin.SkinId)
				{
					hat.HatObject.SetActive(true);
				}
				else
				{
					hat.HatObject.SetActive(false);
				}
			}

			_bodyRenderer.materials = skin.BodyMaterials.ToArray();

			foreach (var armRenderer in _armRenderers)
			{
				armRenderer.materials = skin.ArmMaterials.ToArray();
			}

			foreach (var legRenderer in _legRenderers)
			{
				legRenderer.materials = skin.LegMaterials.ToArray();
			}
		}

		[Button()]
		public void SetSkinViaEditor()
		{
			var skin = GameManager.Instance.GetSkin(_skinToSet);
			SetSkin(skin);
		}

		private DropdownList<string> GetSkinsList()
		{
#if UNITY_EDITOR
			var result = new DropdownList<string>();
			string[] assetNames = AssetDatabase.FindAssets("t:Skins", new[] { "Assets/_Content" });
			foreach (string SOName in assetNames)
			{
				var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
				var skins = AssetDatabase.LoadAssetAtPath<Skins>(SOpath);
				foreach (var skin in skins.SkinsList)
				{
					result.Add(skin.SkinId, skin.SkinId);
				}
			}

			if (assetNames.Length == 0)
			{
				result.Add("Empty", "Empty");
			}

			return result;
#else
				return new DropdownList<string>();
#endif
		}
	}
}