#if UNITY_EDITOR_64
using System.IO;
using UnityEditor;
#endif
using UnityEngine;

public static class TextureExtensions
{
	public static Texture2D ToTexture2D(this RenderTexture rTex, int width, int height, TextureFormat format)
	{
		Texture2D tex = new Texture2D(width, height, format, rTex.useMipMap);
		// ReadPixels looks at the active RenderTexture.
		RenderTexture.active = rTex;
		tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
		tex.Apply();
		return tex;
	}
#if UNITY_EDITOR_64
	
	[MenuItem("Assets/Save as PNG")]
	private static void PasteTextureType(MenuCommand command)
	{
		var texture = (RenderTexture)Selection.activeObject;
		var format = TextureFormat.RGBA32;
		SaveRenderTextureToFile(texture, format);
		/*textureImporter.textureType = (TextureImporterType)copied;
         
		UnityEngine.Debug.Log("Pasted TextureImporterType: " + copied);*/
	}
	
	public static void SaveRenderTextureToFile(RenderTexture rTex, TextureFormat format)
	{
		var texture = rTex.ToTexture2D(rTex.width, rTex.height, format);
		var path = EditorUtility.SaveFilePanel(
			"Save texture as PNG",
			"",
			texture.name + ".png",
			"png");

		if (path.Length != 0)
		{
			var pngData = texture.EncodeToPNG();
			if (pngData != null)
				File.WriteAllBytes(path, pngData);
		}
	}
#endif
}