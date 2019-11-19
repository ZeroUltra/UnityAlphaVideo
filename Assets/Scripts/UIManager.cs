using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIManager : MonoBehaviour
{
    public FileDragAndDrop fileDragAndDrop;
    [Space(5)]
    #region UI
    public Toggle[] togs;
    public RectTransform[] content;

    [Header("MOV2MP4")]
    [Space(5)]
    public Button btnOpenMovFile;
    public Button btnClear;
    public Button btnFolder;
    public Button btnConvertMov;
    public Text txtMovList, txtSeletePath;
    public InputField inputRate;

    [Header("Pic2Video")]
    [Space(5)]
    public Button btnSeletePicFolder;
    public Button btnSeleteSaveVideo, btnConvertPic;
    public Text txtPicFolderPath, txtSaveVideoName;
    public InputField inputCodeRate, inputFrameRate;
    #endregion

    private Mov2Mp4 mov2Mp4;
    private Pic2Video pic2Video;

    private void Start()
    {


        #region MOV2MP4
        mov2Mp4 = new Mov2Mp4();
        fileDragAndDrop.OnMovFileDragEnd += (files) => mov2Mp4.AddDragFile(files);

        btnOpenMovFile.onClick.AddListener(OpenMovFile);
        btnClear.onClick.AddListener(mov2Mp4.ClearMovList);
        btnFolder.onClick.AddListener(OpenMovFolder);
        btnConvertMov.onClick.AddListener(ConvertMov);
        mov2Mp4.OnAddMovFiles += Mov2Mp4_OnSeleteMovEnd;
        mov2Mp4.OnSeleteFolderEnd += Mov2Mp4_OnSeleteFolderEnd;
        inputRate.onValueChanged.AddListener((num) => mov2Mp4.codeRate = int.Parse(num));
        #endregion

        #region Pic2Video
        pic2Video = new Pic2Video();
        fileDragAndDrop.OnPicFolderDragEnd += (file) => pic2Video.OnDragPicFolder(file);
        pic2Video.OnAddPicEnd += Pic2Video_OnSeletePicEnd;
        pic2Video.OnSeleteSaveFileEnd += Pic2Video_OnSeleteSaveFileEnd;
        btnSeletePicFolder.onClick.AddListener(OpenPicFolder);
        btnSeleteSaveVideo.onClick.AddListener(SavePicVideo);
        btnConvertPic.onClick.AddListener(PicConvertVideo);
        inputCodeRate.onValueChanged.AddListener((num) => pic2Video.codeRate = int.Parse(num));
        inputFrameRate.onValueChanged.AddListener((num) => pic2Video.frameRate = int.Parse(num));
        #endregion

        for (int i = 0; i < togs.Length; i++)
        {
            int index = i;
            togs[index].onValueChanged.AddListener((ison) => OnTogChange(index, ison));
            if (index == 0) { togs[index].isOn = true; togs[index].image.color = Color.green; }
            else togs[index].isOn = false;
        }
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
        mov2Mp4.Convert();
    }
    private void Mov2Mp4_OnSeleteFolderEnd(string path)
    {
        txtSeletePath.color = Color.white;
        txtSeletePath.text = path;
    }

    private void Mov2Mp4_OnSeleteMovEnd(string[] paths)
    {
        txtMovList.color = Color.white;
        txtMovList.text = "";

        foreach (var item in paths)
        {
            txtMovList.text += item + "\n";
        }
    }
    #endregion

    #region Pic2Video

    private void SavePicVideo()
    {
        pic2Video.SaveVideoFile();
    }

    private void OpenPicFolder()
    {
        pic2Video.OpenPicFolder();
    }
    private void PicConvertVideo()
    {
        pic2Video.Convert();
    }
    private void Pic2Video_OnSeleteSaveFileEnd(string filePath)
    {
        txtMovList.color = Color.white;
        txtSaveVideoName.text = filePath;
    }

    private void Pic2Video_OnSeletePicEnd(string path)
    {
        txtMovList.color = Color.white;
        txtPicFolderPath.text = path;
    }
    #endregion
}
