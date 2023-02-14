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
        // ab��Դ�洢Ŀ¼
        string path = Application.dataPath.Replace("/Assets", "/AssetBundles/" + assetBundlePlatform.ToString());

        // ������ش����ļ�������յ�
        if (Directory.Exists(path))
        {
            ClearBuildAssetBundles(assetBundlePlatform);
        }

        if (!Directory.Exists(path))
        {
            // ����ļ��в����ڣ����½�һ��
            Directory.CreateDirectory(path);
        }

        // ʹ��LZ4ѹ����ʽ
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

        Debug.Log("AssetBundle.BuildAssetBundles " + assetBundlePlatform + " ���:" + path);
    }

    static void ClearBuildAssetBundles(AssetBundlePlatform assetBundlePlatform)
    {
        string path = Application.dataPath.Replace("/Assets", "/AssetBundles/" + assetBundlePlatform.ToString());
        DirectoryInfo di = new DirectoryInfo(path);
        di.Delete(true);
        Debug.Log("AssetBundle ClearBuildAssetBundles " + assetBundlePlatform + " ���:" + path);
    }
}