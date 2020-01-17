using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Mov2Mp4 : MonoBehaviour
{
    public FileDragAndDrop fileDragAndDrop;
    [Header("MOV2MP4")]
    [SerializeField] Button btnOpenMovFile;
    [SerializeField] Button btnClear;
    [SerializeField] Button btnSaveFolder;
    [SerializeField] Button btnConvertMov;
    [SerializeField] Text txtMovList, txtSeletePath;
    [SerializeField] InputField inputRate;
    [SerializeField] Dropdown dropdownTarget;

    private Mov2Video mov2Mp4;

    private void Start()
    {
        #region MOV2MP4
        mov2Mp4 = new Mov2Video();
        fileDragAndDrop.OnMovFileDragEnd += (files) => mov2Mp4.AddDragFile(files);

        btnOpenMovFile.onClick.AddListener(OpenMovFile);
        btnClear.onClick.AddListener(mov2Mp4.ClearMovList);
        btnSaveFolder.onClick.AddListener(OpenMovFolder);
        btnConvertMov.onClick.AddListener(ConvertMov);
        mov2Mp4.OnAddMovFiles += Mov2Mp4_OnSeleteMovEnd;
        mov2Mp4.OnSeleteSaveFolderEnd += Mov2Mp4_OnSeleteSaveFolderEnd;
        inputRate.onValueChanged.AddListener((num) => mov2Mp4.codeRate = int.Parse(num));
        #endregion
    }

    private void OpenMovFile()
    {
        mov2Mp4.OpenMovFile();
    }
    private void OpenMovFolder()
    {
        mov2Mp4.OpenSaveFolder();
    }
    private void ConvertMov()
    {
        mov2Mp4.Convert(dropdownTarget.captionText.text);
    }
    private void Mov2Mp4_OnSeleteSaveFolderEnd(string path)
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

}
