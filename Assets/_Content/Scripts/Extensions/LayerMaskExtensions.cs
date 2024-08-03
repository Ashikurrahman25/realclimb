using UnityEngine;

public static class LayerMaskExtensions
{
	public static int LayerMaskToLayer(this LayerMask layerMask)
	{
		int layerNumber = 0;
		int layer = layerMask.value;
		while (layer > 0)
		{
			layer = layer >> 1;
			layerNumber++;
		}

		return layerNumber - 1;
	}

	/// <summary>
	/// Returns true if layer mask contains layer.
	/// </summary>
	/// <param name="layer"></param>
	/// <returns></returns>
	public static bool Contains(this LayerMask layerMask, int layer)
	{
		return ((1 << layer) & layerMask) != 0;
	}
}