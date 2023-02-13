using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle : MonoBehaviour
{
    [MenuItem("HP Utils/AssetBundle/Build")]
    static void StartBuildAssetBundles()
    {
        // ab��Դ�洢Ŀ¼
        string path = Application.dataPath.Replace("/Assets", "/AssetBundles/PC");
        if(!Directory.Exists(path))
        {
            // ����ļ��в����ڣ����½�һ��
            Directory.CreateDirectory(path);
        }

        // ʹ��LZ4ѹ����ʽ
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);

        Debug.Log("AssetBundle Build���:" + path);
    }

    [MenuItem("HP Utils/AssetBundle/Clear")]
    static void ClearBuildAssetBundles()
    {
        // ab��Դ�洢Ŀ¼
        string path = Application.dataPath.Replace("/Assets", "/AssetBundles/PC");
        if (!Directory.Exists(path))
        {
            // ����ļ��в����ڣ����½�һ��
            Directory.CreateDirectory(path);
        }

        // ʹ��LZ4ѹ����ʽ
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);

        Debug.Log("AssetBundle Build���:" + path);
    }
}