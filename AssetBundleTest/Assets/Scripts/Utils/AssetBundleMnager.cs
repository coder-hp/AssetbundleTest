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

    // ��Ҫ�Լ����õĲ���
    public AssetBundlePlatform assetBundlePlatform = AssetBundlePlatform.PC;
    string url = "http://qiniu-hp-us.hanxinyi.cn/AssetBundleTest/";         // ������ab����ַ
    string localSaveABPath = "";                                            // ���ش洢ab��·��

    AssetBundle mainAB = null;                                              // ����
    AssetBundleManifest mainManifest = null;                                // �����������ļ�---���Ի�ȡ������
    List<string> abNameList = new List<string>();                           // ���е�ab��Դ������
    List<string> waitDownLoadABList = new List<string>();                   // �ȴ����ص�ab��Դ������
    Dictionary<string, AssetBundle> abCache;                                // ab��Դ������

    void Awake()
    {
        s_instance = this;
        
        url += (assetBundlePlatform.ToString() + "/");
        abCache = new Dictionary<string, AssetBundle>();
        localSaveABPath = Application.persistentDataPath + "/AssetBundles/" + (assetBundlePlatform.ToString() + "/");

        Debug.Log("AssetBundleMnager AB��Դ�����ر���·��:" + localSaveABPath);
    }

    public void downLoadMainABFile()
    {
        Debug.Log("��ʼ��������:" + assetBundlePlatform.ToString());
        StartCoroutine(downLoadFile(url + assetBundlePlatform.ToString(), assetBundlePlatform.ToString(), localSaveABPath, (result)=>
        {
            Debug.Log("AssetBundleMnager.downLoadMainABFile ����" + assetBundlePlatform.ToString() + "  ����" + (result ? "�ɹ�":"ʧ��"));
        }));
    }

    public void downLoadABFile()
    {
        if (mainAB == null)
        {
            Debug.LogError("AssetBundleMnager.downLoadABFile  ������/��������");
            return;
        }

        if (waitDownLoadABList.Count <= 0)
        {
            Debug.Log("AssetBundleMnager.downLoadABFile �Ѿ�ȫ��������ϣ�����Ҫ�ٴ�����");
            return;
        }
        
        Debug.Log("��ʼ����AB��:" + waitDownLoadABList[0]);
        StartCoroutine(downLoadFile(url + waitDownLoadABList[0], waitDownLoadABList[0], localSaveABPath, (result) =>
        {
            if (waitDownLoadABList.Count > 0)
            {
                waitDownLoadABList.RemoveAt(0);

                // ������һ���ļ�����
                if (waitDownLoadABList.Count > 0)
                {
                    downLoadABFile();
                }
                else
                {
                    Debug.Log("AssetBundleMnager.downLoadABFile ȫ���������");
                }
            }
        }));
    }

    IEnumerator downLoadFile(string url, string fileName, string savePath, Action<bool> callback = null)
    {
        Debug.Log("AssetBundleMnager.downLoadFile ��ʼ�����ļ�:" + fileName);

        url += ("?" + getTimeStamp_Millisecond());
        using (UnityWebRequest downloader = UnityWebRequest.Get(url))
        {
            if (!Directory.Exists(savePath))
            {
                // ����ļ��в����ڣ����½�һ��
                Directory.CreateDirectory(savePath);
            }
            string fullSavePath = savePath + fileName;

            if (File.Exists(fullSavePath))
            {
                Debug.Log("ɾ�����������ļ�:" + fullSavePath);
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
                Debug.Log("downloadFile�쳣1:" + fileName + "  " + ex.ToString());
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
                Debug.Log("��ʼ����:" + url);

                while (!downloader.isDone)
                {
                    //Debug.Log("����" + fileName + "����" + downloader.downloadProgress);
                    yield return null;
                }

                if (downloader.error != null)
                {
                    Debug.Log("����ʧ��:" + url);
                    Debug.Log("ʧ����Ϣ:" + downloader.error);

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
                            Debug.Log("���ص������ļ�������:" + fileName);
                            if (callback != null)
                            {
                                callback(false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("downloadFile�쳣2:" + fileName + "  " + ex.ToString());
                        if (callback != null)
                        {
                            callback(false);
                        }
                    }
                }
            }
        }
    }

    // ����main ab��
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
                    Debug.Log("AssetBundleMnager.loadMainAB ����ab��Դ:" + str[i]);
                }
            }
            else
            {
                Debug.Log("AssetBundleMnager.loadMainAB�Ѽ��أ������ظ�����");
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
                Debug.Log("AssetBundleMnager.loadAB �Ѽ��أ�ʹ�û���:" + name);
                return abCache[name];
            }
            string abPath = localSaveABPath + name;
            if (!File.Exists(abPath))
            {
                Debug.LogError("AssetBundleMnager.loadAB �ļ�������:" + abPath);
                return null;
            }
            AssetBundle ab = AssetBundle.LoadFromFile(abPath);
            abCache[name] = ab;

            Debug.Log("AssetBundleMnager.loadAB �������:" + name);
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
                Debug.LogError("AssetBundleMnager.unLoadAB û�и���Դ����:" + name);
                return;
            }

            // Ĭ���ͷ��ѵ���Ķ���
            // ����һ��Image���ʹ����ab�����Sprite���ͷź�Image��ʾ��
            abCache[name].Unload(true);
            abCache.Remove(name);
            Debug.Log("AssetBundleMnager.unLoadAB ж����Դ:" + name);
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
            Debug.Log("AssetBundleMnager.getAssetBundle �и���Դ����:" + name);
            return abCache[name];
        }
        Debug.LogError("AssetBundleMnager.getAssetBundle û�и���Դ����:" + name);
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

    // 13λʱ���������
    long getTimeStamp_Millisecond()
    {
        DateTime original = new DateTime(1970, 1, 1, 0, 0, 0);
        return (long)(DateTime.Now.ToUniversalTime() - original).TotalMilliseconds;
    }
}
