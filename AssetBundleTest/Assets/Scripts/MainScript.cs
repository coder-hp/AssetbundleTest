using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    void Start()
    {
        //StartCoroutine(LoadFromFileASync("D:\\develop\\gitee\\assetbundletest\\AssetBundleTest\\AssetBundles\\PC"));
    }

    IEnumerator LoadFromFileASync(string path)
    {
        AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(path);
        yield return createRequest;
        AssetBundle bundle = createRequest.assetBundle;
        if (bundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }
        var parefab = bundle.LoadAsset<GameObject>("bullet");
        Instantiate(parefab);
    }

}
