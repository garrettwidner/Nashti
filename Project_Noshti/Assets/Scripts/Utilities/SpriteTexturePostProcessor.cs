using UnityEngine;
using UnityEditor;
using System.Collections;
/*
public class SpriteTexturePostProcessor : AssetPostprocessor 
{
	// This will be called whenever a texture2D is processed in some way.
	private void OnPostprocessTexture(Texture2D texture)
	{
		TextureImporter importer = assetImporter as TextureImporter;
		Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));

		//If the asset already exists, and thus has only been changed within the editor...
		if(asset)
		{
			// Flags an object as dirty.
			// Unity uses this internally to find when assets have changed and need to be saved to disk.
			EditorUtility.SetDirty(asset);
		}
		//If the asset is being taken into the editor for the first time
		else
		{
			importer.textureType = TextureImporterType.Sprite;
			importer.spritePixelsPerUnit = 32;
			importer.filterMode = FilterMode.Point;
            importer.maxTextureSize = 2048;
            //importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
		}
	}

}

    */
