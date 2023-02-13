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
        // ab资源存储目录
        string path = Application.dataPath.Replace("/Assets", "/AssetBundles/PC");
        if(!Directory.Exists(path))
        {
            // 如果文件夹不存在，则新建一个
            Directory.CreateDirectory(path);
        }

        // 使用LZ4压缩格式
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);

        Debug.Log("AssetBundle Build完毕:" + path);
    }

    [MenuItem("HP Utils/AssetBundle/Clear")]
    static void ClearBuildAssetBundles()
    {
        // ab资源存储目录
        string path = Application.dataPath.Replace("/Assets", "/AssetBundles/PC");
        if (!Directory.Exists(path))
        {
            // 如果文件夹不存在，则新建一个
            Directory.CreateDirectory(path);
        }

        // 使用LZ4压缩格式
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);

        Debug.Log("AssetBundle Build完毕:" + path);
    }
}