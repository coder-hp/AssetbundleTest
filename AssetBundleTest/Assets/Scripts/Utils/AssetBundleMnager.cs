using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AssetBundlePlatform
{
    PC,
    Android,
    iOS,
}

public class AssetBundleMnager : MonoBehaviour
{
    public static AssetBundleMnager s_instance = null;

    AssetBundlePlatform assetBundlePlatform = AssetBundlePlatform.PC;

    AssetBundle mainAB = null;                      // 主包
    AssetBundleManifest mainManifest = null;        // 主包中配置文件---用以获取依赖包
    List<string> abNameList = new List<string>();   // 所有的ab资源包名字
    Dictionary<string, AssetBundle> abCache = new Dictionary<string, AssetBundle>();    // ab资源包缓存

    void Awake()
    {
        s_instance = this;

        //loadMainAB();
    }

    // 加载main ab包
    public void loadMainAB()
    {
        if (mainAB == null)
        {
            string path = Application.dataPath.Replace("/Assets", "/AssetBundles/" + assetBundlePlatform.ToString());
            mainAB = AssetBundle.LoadFromFile(path + "/" + assetBundlePlatform.ToString());
            mainManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] str = mainManifest.GetAllAssetBundles();
            for (int i = 0; i < str.Length; i++)
            {
                abNameList.Add(str[i]);
                Debug.Log("AssetBundleMnager.loadMainAB 包含ab资源:" + str[i]);
            }
        }
    }

    public AssetBundle loadAB(string name)
    {
        if(abCache.ContainsKey(name))
        {
            Debug.Log("AssetBundleMnager.loadAB 已加载，使用缓存:" + name);
            return abCache[name];
        }
        string path = Application.dataPath.Replace("/Assets", "/AssetBundles/" + assetBundlePlatform.ToString());
        AssetBundle ab = AssetBundle.LoadFromFile(path + "/" + name);
        abCache[name] = ab;

        Debug.Log("AssetBundleMnager.loadAB 加载完毕:" + name);

        return ab;
    }

    public void unLoadAB(string name)
    {
        if (!abCache.ContainsKey(name))
        {
            Debug.LogError("AssetBundleMnager.unLoadAB 没有该资源缓存:" + name);
            return;
        }
        // 默认释放已导入的对象
        // 比如一个Image组件使用了ab包里的Sprite，释放后Image显示空
        abCache[name].Unload(true);
        abCache.Remove(name);
        Debug.Log("AssetBundleMnager.unLoadAB 卸载资源:" + name);
    }

    public AssetBundle getAssetBundle(string name)
    {
        if (abCache.ContainsKey(name))
        {
            Debug.Log("AssetBundleMnager.getAssetBundle 有该资源缓存:" + name);
            return abCache[name];
        }
        Debug.LogError("AssetBundleMnager.getAssetBundle 没有该资源缓存:" + name);
        return null;
    }

    //IEnumerator LoadFromFileASync(string path)
    //{
    //    AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(path);
    //    yield return createRequest;
    //    AssetBundle bundle = createRequest.assetBundle;
    //    if (bundle == null)
    //    {
    //        Debug.Log("Failed to load AssetBundle!");
    //        yield break;
    //    }
    //    var parefab = bundle.LoadAsset<GameObject>("bullet");
    //    Instantiate(parefab);
    //}
}
