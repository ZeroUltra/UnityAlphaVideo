using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using B83.Win32;
using System.IO;
public class FileDragAndDrop : MonoBehaviour
{
    /// <summary>
    /// mov文件拖动结束
    /// </summary>
    public event System.Action<string[]> OnMovFileDragEnd;
    /// <summary>
    /// pic文件夹拖动结束
    /// </summary>
    public event System.Action<string> OnPicFolderDragEnd;

    private Rect movRect = new Rect(380f, 288f, 513f, 117f);  //mov文件拖放区域
    private Rect picRect = new Rect(380f, 288f, 530f, 50f);  //图片拖放区域

    public Toggle togmov2Video, tog2Pic2Video;
    UnityDragAndDropHook hook;

    public Text text;
    void OnEnable()
    {
        hook = new UnityDragAndDropHook();
        hook.InstallHook();
        hook.OnDroppedFiles += OnFiles;
    }
    void OnDisable()
    {
        hook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        Vector2 pos = new Vector2(aPos.x, aPos.y);

        if (togmov2Video.isOn)
        {
            if (movRect.Contains(pos))
            {
                text.text = "mov视频区域" + "Dropped " + aFiles.Count + " files at: " + aPos + "\n" +
                aFiles.Aggregate((a, b) => a + "\n" + b);

                List<string> movfile = new List<string>();
                foreach (var item in aFiles)
                {
                    if (GameManager.Instance.FileIsMov(item))
                    {
                        movfile.Add(item);
                    }
                }
                if (movfile.Count > 0)
                    OnMovFileDragEnd?.Invoke(movfile.ToArray());
            }
        }
        else if (tog2Pic2Video.isOn)
        {
            if (picRect.Contains(pos))
            {
                text.text = "图片区域" + "Dropped " + aFiles.Count + " files at: " + aPos + "\n" +
                             aFiles.Aggregate((a, b) => a + "\n" + b);
                OnPicFolderDragEnd?.Invoke(aFiles[0]);
            }
        }
        else
        {
            text.text = "没有!!!" + "Dropped " + aFiles.Count + " files at: " + aPos + "\n" +
        aFiles.Aggregate((a, b) => a + "\n" + b);

        }
    }
}