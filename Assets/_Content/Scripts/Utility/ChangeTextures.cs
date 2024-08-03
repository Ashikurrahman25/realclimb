using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
	public class ChangeTextures: MonoBehaviour
	{
		public string Prefix;

		public List<Material> mats; 

		private void ChangeTexture()
		{
			var mesh = GetComponent<MeshRenderer>();

			for (int i = 0; i < mesh.materials.Length - 1; i++)
			{
				var m = mats.Find(mat => mat.name == (Prefix + mesh.materials[i].name + " 1"));

				if (m != null)
				{
					mesh.materials.SetValue(m,i);;
				}
			}
		}
	}
}