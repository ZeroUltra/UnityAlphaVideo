using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIManager : MonoBehaviour
{
    #region UI
    public Toggle[] togs;
    public RectTransform[] content;

    [Header("MOV2MP4")]
    public Button btnOpenMovFile;
    public Button btnFolder;
    public Button btnConvert;
    public Text txtMovList,txtSeletePath;
    public InputField inputRate;
    #endregion

    private Mov2Mp4 mov2Mp4;
    private void Start()
    {
        for (int i = 0; i < togs.Length; i++)
        {
            int index = i;
            togs[index].onValueChanged.AddListener((ison)=>OnTogChange(index,ison));
            if (index == 0) { togs[index].isOn = true; togs[index].image.color = Color.green; } 
            else togs[index].isOn = false;
        }

        #region MOV2MP4
        mov2Mp4 = new Mov2Mp4();
        btnOpenMovFile.onClick.AddListener(OpenMovFile);
        btnFolder.onClick.AddListener(OpenMovFolder);
        btnConvert.onClick.AddListener(ConvertMov);
        mov2Mp4.OnSeleteMovEnd += Mov2Mp4_OnSeleteMovEnd;
        mov2Mp4.OnSeleteFolderEnd += Mov2Mp4_OnSeleteFolderEnd;
        inputRate.onValueChanged.AddListener((num) => mov2Mp4.codeRate = int.Parse(num));
        #endregion
    }

    

    private void OnTogChange(int index, bool ison)
    {
        if (ison)
        {
            togs[index].image.color = Color.green;
            content[index].DOScaleX(1, 0.4f);
        }
        else
        {
           
            togs[index].image.color = Color.white;
            content[index].localScale = new Vector3(0, 1, 1);
        }
    }

    #region Mov
    private void OpenMovFile()
    {
        mov2Mp4.OpenMovFile();
    }
    private void OpenMovFolder()
    {
        mov2Mp4.OpenFolder();
    }
    private void ConvertMov()
    {
        if (mov2Mp4.GetPathIsNull() == false)
        {
            mov2Mp4.RunFFmpeg();
        }
    }
    private void Mov2Mp4_OnSeleteFolderEnd(string path)
    {
        txtSeletePath.color = Color.white;
        txtSeletePath.text = path;
    }

    private void Mov2Mp4_OnSeleteMovEnd(string[] paths)
    {
        txtMovList.text = "";
        txtMovList.color = Color.white;
        foreach (var item in paths)
        {
            txtMovList.text += item + "\n";
        }
    }
    #endregion
}
