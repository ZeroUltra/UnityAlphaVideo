using UnityEngine;
using System.IO;
using System;
using SFB;


public class GameManager : Singleton<GameManager>
{
    public static string FFMpegPath;
    public UnityEngine.UI.Image dragImg;

    protected override void Awake()
    {
        base.Awake();
        Loom.Initialize();
        FFMpegPath = Application.streamingAssetsPath + "/FFmpeg/bin";
    }


    /// <summary>
    /// 打开文件
    /// </summary>
    /// <param name="filters">过滤</param>
    /// <param name="cb">回调</param>
    /// <param name="muliSete">是否可以多选</param>
    public void DialogOpenFile(string title, string path, ExtensionFilter[] filters, Action<string[]> cb, bool muliSete = true)
    {
        StandaloneFileBrowser.OpenFilePanelAsync(title, path, filters, muliSete, cb);
    }
    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="title"></param>
    /// <param name="path"></param>
    /// <param name="cb">回调</param>
    /// <param name="muliSete"></param>
    public void DialogOpenFolder(string title, string path, Action<string[]> cb, bool muliSete = false)
    {
        StandaloneFileBrowser.OpenFolderPanelAsync(title, path, muliSete, cb);
    }

    public void DialogSaveFile(string title, string directory, string fileName, ExtensionFilter[] extensions, Action<string> cb)
    {
        StandaloneFileBrowser.SaveFilePanelAsync(title, directory, fileName, extensions, cb);
    }

    public bool FileIsMov(string fileName)
    {
        return Path.GetExtension(fileName).ToLower() == ".mov";
    }

    public bool FolderIsFramePic(string folder)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(folder);
        //看第一个文件扩展名是否是图片文件 然后检查所有的文件
        string exName = Path.GetExtension(directoryInfo.GetFiles()[0].Name).ToLower();
        if (exName == ".png" || exName == ".jpg" || exName == ".bmp")
        {
            //然后查看所有文件
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (var item in files)
            {
                if (item.Extension != exName)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
