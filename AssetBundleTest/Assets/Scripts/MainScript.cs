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

    public void onClickDownLoadMainAB()
    {
        // ����������
        AssetBundleMnager.s_instance.downLoadMainABFile();
    }

    public void onClickLoadMainAB()
    {
        AssetBundleMnager.s_instance.loadMainAB();
    }

    public void onClickDownLoadAB()
    {
        // ��������AB��
        AssetBundleMnager.s_instance.downLoadABFile();
    }

    public void onClickLoadAB()
    {
        AssetBundleMnager.s_instance.loadAB("images.ab");
    }

    public void onClickGetAB()
    {
        image1.sprite = AssetBundleMnager.s_instance.getAssetBundle("images.ab").LoadAsset<Sprite>("1");
    }

    public void onClickUnLoadAB()
    {
        AssetBundleMnager.s_instance.unLoadAB("images.ab");
    }
}
