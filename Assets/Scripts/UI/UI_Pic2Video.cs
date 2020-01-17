using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Pic2Video : MonoBehaviour
{
    public FileDragAndDrop fileDragAndDrop;
    [Header("Pic2Video")]
    [SerializeField] Button btnSeletePicFolder;
    [SerializeField] Button btnSeleteSaveVideo, btnConvertPic;
    [SerializeField] Text txtPicFolderPath, txtSaveVideoName;
    [SerializeField] InputField inputCodeRate, inputFrameRate;

    private Pic2Video pic2Video;

    private void Start()
    {
        pic2Video = new Pic2Video();
        fileDragAndDrop.OnPicFolderDragEnd += (file) => pic2Video.OnDragPicFolder(file);
      
        btnSeletePicFolder.onClick.AddListener(OpenPicFolder);
        btnSeleteSaveVideo.onClick.AddListener(SavePicVideo);
        btnConvertPic.onClick.AddListener(PicConvertVideo);

        inputCodeRate.onValueChanged.AddListener((num) => pic2Video.codeRate = int.Parse(num));
        inputFrameRate.onValueChanged.AddListener((num) => pic2Video.frameRate = int.Parse(num));

        pic2Video.OnAddPicEnd += Pic2Video_OnSeletePicEnd;
        pic2Video.OnSeleteSaveFileEnd += Pic2Video_OnSeleteSaveFileEnd;
    }
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
        txtSaveVideoName.color = Color.white;
        txtSaveVideoName.text = filePath;
    }

    private void Pic2Video_OnSeletePicEnd(string path)
    {
        txtPicFolderPath.color = Color.white;
        txtPicFolderPath.text = path;
    }

}
