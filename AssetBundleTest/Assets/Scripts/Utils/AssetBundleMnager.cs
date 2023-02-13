using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AssetBundlePlatform
{
    PC,
    Android,
    iOS,
}

public class AssetBundleMnager : MonoBehaviour
{
    AssetBundlePlatform assetBundlePlatform = AssetBundlePlatform.PC;

    AssetBundle mainAB = null;                  // 主包
    AssetBundleManifest mainManifest = null;    // 主包中配置文件---用以获取依赖包
    Dictionary<string, AssetBundle> abCache = new Dictionary<string, AssetBundle>();
    List<string> abNameList = new List<string>();

    void Start()
    {
        loadMainAB();

        loadAB("images");
    }

    // 加载main ab包
    void loadMainAB()
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
                Debug.Log("包含ab资源:" + str[i]);
            }
        }
    }

    void loadAB(string name)
    {
        string path = Application.dataPath.Replace("/Assets", "/AssetBundles/" + assetBundlePlatform.ToString());
        AssetBundle ab = AssetBundle.LoadFromFile(path + "/" + name);
        string[] str = ab.GetAllAssetNames();
        for (int i = 0; i < str.Length; i++)
        {
            Debug.Log(str[i]);
        }

        GameObject.Find("Canvas/Image").GetComponent<Image>().sprite = ab.LoadAsset<Sprite>("1");
    }
}
