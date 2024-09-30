using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;


public static class AssetManager
{
    private static string imagePath = "Assets/Images/{0}.png";
    private static string prefabPath = "Assets/Prefabs/{0}.prefab";
    
    public static void LoadSprite(string spriteName, System.Action<Sprite> onLoad)
    {
        Addressables.LoadAssetAsync<Sprite>(string.Format(imagePath, spriteName)).Completed += (loadedSprite) =>
        {
            onLoad?.Invoke(loadedSprite.Result);
        };
    }

    public static void LoadPrefab(string prefabName, System.Action<GameObject> onLoad)
    {
        Addressables.LoadAssetAsync<GameObject>(string.Format(prefabPath, prefabName)).Completed += (loadedPrefab) =>
        {
            onLoad?.Invoke(loadedPrefab.Result);
        };
    }

}
