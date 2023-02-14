using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public enum AssetBundlePlatform
{
    PC,
    Android,
    iOS,
}

public class AssetBundleMnager : MonoBehaviour
{
    public static AssetBundleMnager s_instance = null;

    // 需要自己配置的参数
    public AssetBundlePlatform assetBundlePlatform = AssetBundlePlatform.PC;
    string url = "http://qiniu-hp-us.hanxinyi.cn/AssetBundleTest/";         // 服务器ab包地址
    string localSaveABPath = "";                                            // 本地存储ab包路径

    AssetBundle mainAB = null;                                              // 主包
    AssetBundleManifest mainManifest = null;                                // 主包中配置文件---用以获取依赖包
    List<string> abNameList = new List<string>();                           // 所有的ab资源包名字
    List<string> waitDownLoadABList = new List<string>();                   // 等待下载的ab资源包名字
    Dictionary<string, AssetBundle> abCache;                                // ab资源包缓存

    void Awake()
    {
        s_instance = this;
        
        url += (assetBundlePlatform.ToString() + "/");
        abCache = new Dictionary<string, AssetBundle>();
        localSaveABPath = Application.persistentDataPath + "/AssetBundles/" + (assetBundlePlatform.ToString() + "/");

        Debug.Log("AssetBundleMnager AB资源包本地保存路径:" + localSaveABPath);
    }

    public void downLoadMainABFile()
    {
        Debug.Log("开始下载主包:" + assetBundlePlatform.ToString());
        StartCoroutine(downLoadFile(url + assetBundlePlatform.ToString(), assetBundlePlatform.ToString(), localSaveABPath, (result)=>
        {
            Debug.Log("AssetBundleMnager.downLoadMainABFile 主包" + assetBundlePlatform.ToString() + "  下载" + (result ? "成功":"失败"));
        }));
    }

    public void downLoadABFile()
    {
        if (mainAB == null)
        {
            Debug.LogError("AssetBundleMnager.downLoadABFile  先下载/加载主包");
            return;
        }

        if (waitDownLoadABList.Count <= 0)
        {
            Debug.Log("AssetBundleMnager.downLoadABFile 已经全部下载完毕，不需要再次下载");
            return;
        }
        
        Debug.Log("开始下载AB包:" + waitDownLoadABList[0]);
        StartCoroutine(downLoadFile(url + waitDownLoadABList[0], waitDownLoadABList[0], localSaveABPath, (result) =>
        {
            if (waitDownLoadABList.Count > 0)
            {
                waitDownLoadABList.RemoveAt(0);

                // 继续下一个文件下载
                if (waitDownLoadABList.Count > 0)
                {
                    downLoadABFile();
                }
                else
                {
                    Debug.Log("AssetBundleMnager.downLoadABFile 全部下载完毕");
                }
            }
        }));
    }

    IEnumerator downLoadFile(string url, string fileName, string savePath, Action<bool> callback = null)
    {
        Debug.Log("AssetBundleMnager.downLoadFile 开始下载文件:" + fileName);

        url += ("?" + getTimeStamp_Millisecond());
        using (UnityWebRequest downloader = UnityWebRequest.Get(url))
        {
            if (!Directory.Exists(savePath))
            {
                // 如果文件夹不存在，则新建一个
                Directory.CreateDirectory(savePath);
            }
            string fullSavePath = savePath + fileName;

            if (File.Exists(fullSavePath))
            {
                Debug.Log("删除本地已有文件:" + fullSavePath);
                File.Delete(fullSavePath);
            }

            bool isError = false;
            try
            {
                downloader.downloadHandler = new DownloadHandlerFile(fullSavePath);
                downloader.SendWebRequest();
            }
            catch (Exception ex)
            {
                Debug.Log("downloadFile异常1:" + fileName + "  " + ex.ToString());
                if (callback != null)
                {
                    callback(false);
                }

                isError = true;
            }

            if (isError)
            {
                yield return null;
            }
            else
            {
                Debug.Log("开始下载:" + url);

                while (!downloader.isDone)
                {
                    //Debug.Log("下载" + fileName + "进度" + downloader.downloadProgress);
                    yield return null;
                }

                if (downloader.error != null)
                {
                    Debug.Log("下载失败:" + url);
                    Debug.Log("失败信息:" + downloader.error);

                    if (callback != null)
                    {
                        callback(false);
                    }
                }
                else
                {
                    try
                    {
                        if (File.Exists(fullSavePath))
                        {
                            if (callback != null)
                            {
                                callback(true);
                            }
                        }
                        else
                        {
                            Debug.Log("下载到本地文件不存在:" + fileName);
                            if (callback != null)
                            {
                                callback(false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("downloadFile异常2:" + fileName + "  " + ex.ToString());
                        if (callback != null)
                        {
                            callback(false);
                        }
                    }
                }
            }
        }
    }

    // 加载main ab包
    public void loadMainAB()
    {
        try
        {
            if (mainAB == null)
            {
                mainAB = AssetBundle.LoadFromFile(localSaveABPath + assetBundlePlatform.ToString());
                mainManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                string[] str = mainManifest.GetAllAssetBundles();
                for (int i = 0; i < str.Length; i++)
                {
                    abNameList.Add(str[i]);
                    waitDownLoadABList.Add(str[i]);
                    Debug.Log("AssetBundleMnager.loadMainAB 包含ab资源:" + str[i]);
                }
            }
            else
            {
                Debug.Log("AssetBundleMnager.loadMainAB已加载，不可重复加载");
            }
        }
        catch(Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    public AssetBundle loadAB(string name)
    {
        try
        {
            if (abCache.ContainsKey(name))
            {
                Debug.Log("AssetBundleMnager.loadAB 已加载，使用缓存:" + name);
                return abCache[name];
            }
            string abPath = localSaveABPath + name;
            if (!File.Exists(abPath))
            {
                Debug.LogError("AssetBundleMnager.loadAB 文件不存在:" + abPath);
                return null;
            }
            AssetBundle ab = AssetBundle.LoadFromFile(abPath);
            abCache[name] = ab;

            Debug.Log("AssetBundleMnager.loadAB 加载完毕:" + name);
            return ab;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            return null;
        }
    }

    public void unLoadAB(string name)
    {
        try
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
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
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
    //}

    // 13位时间戳：毫秒
    long getTimeStamp_Millisecond()
    {
        DateTime original = new DateTime(1970, 1, 1, 0, 0, 0);
        return (long)(DateTime.Now.ToUniversalTime() - original).TotalMilliseconds;
    }
}
