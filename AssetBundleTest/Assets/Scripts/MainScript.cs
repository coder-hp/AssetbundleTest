using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public Image image1;
    AssetBundle ab_images;

    void Start()
    {
    }

    public void onClickLoadMainAB()
    {
        AssetBundleMnager.s_instance.loadMainAB();
    }

    public void onClickLoadAB()
    {
        ab_images = AssetBundleMnager.s_instance.loadAB("images");
    }

    public void onClickGetAB()
    {
        //image1.sprite = ab_images.LoadAsset<Sprite>("1");
        image1.sprite = AssetBundleMnager.s_instance.getAssetBundle("images").LoadAsset<Sprite>("1");
    }

    public void onClickUnLoadAB()
    {
        AssetBundleMnager.s_instance.unLoadAB("images");
    }
}
