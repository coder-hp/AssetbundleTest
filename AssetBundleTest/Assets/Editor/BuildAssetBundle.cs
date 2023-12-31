using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle : MonoBehaviour
{
    enum AssetBundlePlatform
    {
        PC,
        Android,
        iOS,
    }

    [MenuItem("HP-Tool/AssetBundle/Build/PC")]
    static void BuildPC()
    {
        BuildAssetBundles(AssetBundlePlatform.PC);
    }

    [MenuItem("HP-Tool/AssetBundle/Build/Android")]
    static void BuildAndroid()
    {
        BuildAssetBundles(AssetBundlePlatform.Android);
    }

    [MenuItem("HP-Tool/AssetBundle/Build/iOS")]
    static void BuildIos()
    {
        BuildAssetBundles(AssetBundlePlatform.iOS);
    }

    [MenuItem("HP-Tool/AssetBundle/Clear/PC")]
    static void ClearAssetBundlePC()
    {
        ClearBuildAssetBundles(AssetBundlePlatform.PC);
    }

    [MenuItem("HP-Tool/AssetBundle/Clear/Android")]
    static void ClearAssetBundleAndroid()
    {
        ClearBuildAssetBundles(AssetBundlePlatform.Android);
    }

    [MenuItem("HP-Tool/AssetBundle/Clear/iOS")]
    static void ClearAssetBundleIos()
    {
        ClearBuildAssetBundles(AssetBundlePlatform.iOS);
    }

    //---------------------------------------------------------------------------------------------

    static void BuildAssetBundles(AssetBundlePlatform assetBundlePlatform)
    {
        // ab资源存储目录
        string path = Application.dataPath.Replace("/Assets", "/AssetBundles/" + assetBundlePlatform.ToString());

        // 如果本地存在文件，先清空掉
        if (Directory.Exists(path))
        {
            ClearBuildAssetBundles(assetBundlePlatform);
        }

        if (!Directory.Exists(path))
        {
            // 如果文件夹不存在，则新建一个
            Directory.CreateDirectory(path);
        }

        // 使用LZ4压缩格式
        switch(assetBundlePlatform)
        {
            case AssetBundlePlatform.PC:
                {
                    BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
                    break;
                }

            case AssetBundlePlatform.Android:
                {
                    BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
                    break;
                }

            case AssetBundlePlatform.iOS:
                {
                    BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
                    break;
                }
        }

        Debug.Log("AssetBundle.BuildAssetBundles " + assetBundlePlatform + " 完毕:" + path);
    }

    static void ClearBuildAssetBundles(AssetBundlePlatform assetBundlePlatform)
    {
        string path = Application.dataPath.Replace("/Assets", "/AssetBundles/" + assetBundlePlatform.ToString());
        DirectoryInfo di = new DirectoryInfo(path);
        di.Delete(true);
        Debug.Log("AssetBundle ClearBuildAssetBundles " + assetBundlePlatform + " 完毕:" + path);
    }
}