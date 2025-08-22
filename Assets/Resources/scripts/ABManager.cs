#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using DG.Tweening.Plugins.Core.PathCore;

public class ABManager : BaseManager<ABManager>, IModule
    {
    public void Handle(object msg)
    {
        
    }
    // 构建AB包
#if UNITY_EDITOR
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        // 输出路径（建议放在StreamingAssets或服务器目录）
        string outputPath = "Assets/StreamingAssets/AssetBundles";
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        // 构建AB包（支持多平台）
        BuildPipeline.BuildAssetBundles(
            outputPath,
            BuildAssetBundleOptions.None,  // 构建选项
            EditorUserBuildSettings.activeBuildTarget  // 目标平台
        );

        // 刷新Project窗口
        AssetDatabase.Refresh();
        UnityEngine.Debug.Log("AssetBundles built successfully!");
    }
#endif
    public string abPath;

    public void Initial()
    {
    #if UNITY_ANDROID
            abPath = Application.streamingAssetsPath + "/AssetBundles/models/player";
    #elif UNITY_IOS
            abPath = Application.streamingAssetsPath + "/AssetBundles/models/player";
    #else
            abPath = "file://" + Application.streamingAssetsPath + "/AssetBundles";
    #endif
    }
    // 加载AB包
    IEnumerator Start()
    {
        // AB包路径（不同平台路径差异）
        

        // 加载AB包
        using (UnityWebRequest www = new UnityWebRequest(abPath))  // 或使用UnityWebRequest
        {
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError("AB包加载失败: " + www.error);
                yield break;
            }

            // 获取AB包实例
            AssetBundle ab = DownloadHandlerAssetBundle.GetContent(www);
            if (ab == null)
            {
                Debug.LogError("AB包为空");
                yield break;
            }

            // 加载资源（资源名需与打包时一致）
            GameObject playerPrefab = ab.LoadAsset<GameObject>("player");
            GameObject.Instantiate(playerPrefab);

            // 卸载AB包（false：仅卸载包，不卸载已加载资源）
            ab.Unload(false);
        }
    }
    public IEnumerator GetAB()
    {
        using (UnityWebRequest www = new UnityWebRequest(abPath))  // 或使用UnityWebRequest
        {
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError("AB包加载失败: " + www.error);
                yield break;
            }

            // 获取AB包实例
            AssetBundle ab = DownloadHandlerAssetBundle.GetContent(www);
            if (ab == null)
            {
                Debug.LogError("AB包为空");
                yield break;
            }

            // 加载资源（资源名需与打包时一致）
            GameObject playerPrefab = ab.LoadAsset<GameObject>("player");
            GameObject.Instantiate(playerPrefab);

            // 卸载AB包（false：仅卸载包，不卸载已加载资源）
            ab.Unload(false);
        }
    }
    // 下载并加载AB包
    IEnumerator DownloadAndLoadAB(string url)
    {
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("下载失败: " + request.error);
                yield break;
            }

            //// 获取AB包
            //AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
            //// 加载资源（示例：加载纹理）
            //Texture2D tex = ab.LoadAsset<Texture2D>("icon");
            //GetComponent<Renderer>().material.mainTexture = tex;

            //ab.Unload(false);
        }
    }
    // 加载依赖包
    IEnumerator LoadWithDependencies()
    {
        // 加载主清单
        string manifestPath = "Assets/StreamingAssets/AssetBundles/AssetBundles";
        using (WWW www = new WWW(manifestPath))
        {
            yield return www;
            AssetBundle manifestAB = www.assetBundle;
            AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            // 获取目标包的依赖列表
            string targetABName = "models/player";
            string[] dependencies = manifest.GetAllDependencies(targetABName);

            // 加载所有依赖包
            foreach (string dep in dependencies)
            {
                string depPath = "Assets/StreamingAssets/AssetBundles/" + dep;
                using (WWW depWWW = new WWW(depPath))
                {
                    yield return depWWW;
                    AssetBundle depAB = depWWW.assetBundle;
                    // 依赖包无需显式卸载，主包卸载时自动处理
                }
            }

            // 加载目标包
            string targetPath = "Assets/StreamingAssets/AssetBundles/" + targetABName;
            using (WWW targetWWW = new WWW(targetPath))
            {
                yield return targetWWW;
                AssetBundle targetAB = targetWWW.assetBundle;
                // 加载资源...
            }

            manifestAB.Unload(false);
        }
    }
    private Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();

    // 加载 AssetBundle 并缓存
    public bool LoadAndCacheBundle(string bundleName)
    {
        if (loadedBundles.ContainsKey(bundleName))
        {
            Debug.Log($"AB包已缓存: {bundleName}");
            return true;
        }

        string bundlePath = System.IO.Path.Combine(Application.streamingAssetsPath, "AssetBundles", bundleName);

        if (!File.Exists(bundlePath))
        {
            Debug.LogError($"AB包文件不存在: {bundlePath}");
            return false;
        }

        AssetBundle ab = AssetBundle.LoadFromFile(bundlePath);
        if (ab == null)
        {
            Debug.LogError($"无法加载AB包: {bundleName}");
            return false;
        }

        loadedBundles[bundleName] = ab;
        Debug.Log($"AB包加载并缓存成功: {bundleName}");
        string[] allAssetNames = ab.GetAllAssetNames();
        foreach(var sb in allAssetNames)
        {
            Debug.Log($"资源: {sb}");
        }
        return true;
    }

    // 从缓存的 AssetBundle 加载资源
    public T LoadAssetFromCachedBundle<T>(string bundleName, string assetName) where T : Object
    {
        if (!loadedBundles.ContainsKey(bundleName))
        {
            if (!LoadAndCacheBundle(bundleName))
            {
                return null;
            }
        }
       
        AssetBundle ab = loadedBundles[bundleName];
        T asset = ab.LoadAsset<T>(assetName);

        if (asset == null)
        {
            Debug.LogError($"未找到资源: {assetName} in {bundleName}");
        }

        return asset;
    }
    public List<T> LoadAllAssetFromCachedBundle<T>(string bundleName) where T : Object
    {
        List<T> assets = new List<T>();
        if (!loadedBundles.ContainsKey(bundleName))
        {
            if (!LoadAndCacheBundle(bundleName))
            {
                return null;
            }
        }

        AssetBundle ab = loadedBundles[bundleName];
        foreach(var sb in ab.GetAllAssetNames())
        {
            assets.Add(ab.LoadAsset<T>(sb));
        }

        

        return assets;
    }
    // 卸载缓存的 AssetBundle
    public void UnloadBundle(string bundleName, bool unloadAllLoadedObjects = false)
    {
        if (loadedBundles.ContainsKey(bundleName))
        {
            loadedBundles[bundleName].Unload(unloadAllLoadedObjects);
            loadedBundles.Remove(bundleName);
            Debug.Log($"AB包已卸载: {bundleName}");
        }
    }

    // 卸载所有缓存的 AssetBundle
    public void UnloadAllBundles(bool unloadAllLoadedObjects = false)
    {
        foreach (var kvp in loadedBundles)
        {
            kvp.Value.Unload(unloadAllLoadedObjects);
        }
        loadedBundles.Clear();
        Debug.Log("所有AB包已卸载");
    }

    void OnDestroy()
    {
        UnloadAllBundles();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        
    }

    void IModule.Update()
    {
        throw new System.NotImplementedException();
    }
}
