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

    AssetBundle mainAB = null;                      // ����
    AssetBundleManifest mainManifest = null;        // �����������ļ�---���Ի�ȡ������
    List<string> abNameList = new List<string>();   // ���е�ab��Դ������
    Dictionary<string, AssetBundle> abCache = new Dictionary<string, AssetBundle>();    // ab��Դ������

    void Awake()
    {
        s_instance = this;

        //loadMainAB();
    }

    // ����main ab��
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
                Debug.Log("AssetBundleMnager.loadMainAB ����ab��Դ:" + str[i]);
            }
        }
    }

    public AssetBundle loadAB(string name)
    {
        if(abCache.ContainsKey(name))
        {
            Debug.Log("AssetBundleMnager.loadAB �Ѽ��أ�ʹ�û���:" + name);
            return abCache[name];
        }
        string path = Application.dataPath.Replace("/Assets", "/AssetBundles/" + assetBundlePlatform.ToString());
        AssetBundle ab = AssetBundle.LoadFromFile(path + "/" + name);
        abCache[name] = ab;

        Debug.Log("AssetBundleMnager.loadAB �������:" + name);

        return ab;
    }

    public void unLoadAB(string name)
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
    //    var parefab = bundle.LoadAsset<GameObject>("bullet");
    //    Instantiate(parefab);
    //}
}
