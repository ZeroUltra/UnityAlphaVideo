using System.Collections.Generic;
using UnityEngine;
using SFB;
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Object = System.Object;
using Debug = UnityEngine.Debug;
public class Pic2Video
{
    private string seletePicPath, saveVideoPath;  //选择图片路径  保存视频路径
    private string saveVideoName;
    private string picStyteType;  //图片样式类型 000.png  %3d.png qwer_00000.png qwer_%5d.png 
    private string ffmpegPath;

   
    public int codeRate = 1000;
    public int frameRate = 25;
    public event Action<string> OnAddPicEnd;
    public event Action<string> OnSeleteSaveFileEnd;

    private ExtensionFilter[] extensions = new[] {
                                                    new ExtensionFilter("Webm Files", "Webm"),
                                                    new ExtensionFilter("Mp4 Files", "mp4"),
                                                    new ExtensionFilter("All Files", "*" ),
                                                 };

    public Pic2Video()
    {
        ffmpegPath = GameManager.FFMpegPath;
        if (!File.Exists(ffmpegPath + "/ffmpeg.exe"))
        {
            DialogMgr.Instance.ShowDialogTypeBtnOne($"{ffmpegPath}\n文件夹不存在ffmpeg,请检查", "错误");
        }
    }
    //打开图片序列路径
    public void OpenPicFolder()
    {
        if (string.IsNullOrEmpty(seletePicPath)) { seletePicPath = Application.dataPath; }
        GameManager.Instance.DialogOpenFolder("请选择图片序列", seletePicPath, SeletePicEnd, false);
    }
    //路径打开结束回调
    private void SeletePicEnd(string[] paths)
    {
        if (paths.Length <= 0) return;
        seletePicPath = paths[0]; //当前选择的路径
        if (GameManager.Instance.FolderIsFramePic(seletePicPath))
        {
            picStyteType = Path.GetFileName(Directory.GetFiles(seletePicPath)[0]);
            picStyteType = GetPicType(picStyteType);
            Debug.Log("样式 " + picStyteType);
            OnAddPicEnd?.Invoke(paths[0]);
        }
        else
        {
            DialogMgr.Instance.ShowDialogTypeBtnOne("图片文件有错 第一个文件名:" + picStyteType, "错误");
            return;
        }
    }

    private string GetPicType(string picType)
    {
        int index = picType.IndexOf('0');
        int lastindex = picType.LastIndexOf('.');
        return (picType.Substring(0, index) + $"%{lastindex - index}d" + picType.Substring(lastindex, picType.Length - lastindex));
    }

    //选择视频保存文件
    public void SaveVideoFile()
    {
        if (string.IsNullOrEmpty(saveVideoPath)) { saveVideoPath = Application.dataPath; }
        GameManager.Instance.DialogSaveFile("选择保存路径", saveVideoPath, new DirectoryInfo(seletePicPath).Name, extensions, SeleteSaveEnd);
    }
    //视频保存选择结束回调
    private void SeleteSaveEnd(string path)
    {
        saveVideoName = path;
        OnSeleteSaveFileEnd?.Invoke(path);
    }
    /// <summary>
    /// 检查路径是否为空
    /// </summary>
    /// <returns></returns>
    public bool GetPathIsNull()
    {
        if (string.IsNullOrEmpty(seletePicPath) || string.IsNullOrEmpty(saveVideoPath))
        {
            DialogMgr.Instance.ShowDialogTypeBtnOne("请选择目录", "错误提示");
            return true;
        }
        return false;
    }

    public void Convert()
    {
        if (GetPathIsNull()) return;
        DatasStruct datas = new DatasStruct();
        Thread thread = new Thread(RunFFmpegExe);
        datas.thread = thread;
        datas.picFolderName = seletePicPath;
        datas.outFileName = saveVideoName;
        thread.Start(datas);

    }
    private void RunFFmpegExe(object obj)
    {
        /*
            * 视频命令
            * MP4 ffmpeg.exe -f image2 -i c:\temp\d.jpg -vcodec libx264 -r 10 -b 200k  test.mp4
            * webm ffmpeg.exe - i png / B_ % 5d.png - auto - alt -ref 0 - c:v libvpx export.webm
        */
        DatasStruct data = obj as DatasStruct;
        Process p = new Process();
        p.StartInfo.FileName = ffmpegPath + "/ffmpeg.exe";
        Debug.Log(Path.GetExtension(saveVideoName));

        string arguments = Path.GetExtension(saveVideoName) == ".mp4" ? $"-f image2 -i {seletePicPath}/{picStyteType} -vf  \"scale=trunc(iw/2)*2:trunc(ih/2)*2\" -vf \"split[a], pad = iw * 2:ih[b], [a] alphaextract, [b] overlay=w\"  -b {codeRate}k {saveVideoName}" :
                                                                        $"-i {seletePicPath}/{picStyteType} -r {frameRate} -b {codeRate}k -auto-alt-ref 0  -vcodec libvpx  {saveVideoName}";

        p.StartInfo.Arguments = arguments;
        Debug.Log("ffmeeg 信息:  " + p.StartInfo.Arguments);
        p.StartInfo.CreateNoWindow = false;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.Start();
        Debug.Log("ffmeeg 信息: start");
        p.WaitForExit();
        Debug.Log("ffmeeg 信息: Exit");
        p.Close();
        Loom.QueueOnMainThread(() => ShowDiglog());
        data.thread.Abort();
    }

    public void OnDragPicFolder(string folder)
    {
        seletePicPath = folder; //当前选择的路径
        if (GameManager.Instance.FolderIsFramePic(seletePicPath))
        {
            picStyteType = Path.GetFileName(Directory.GetFiles(seletePicPath)[0]);
            picStyteType = GetPicType(picStyteType);
            Debug.Log("样式 " + picStyteType);
            OnAddPicEnd?.Invoke(folder);
        }
        else
        {
            DialogMgr.Instance.ShowDialogTypeBtnOne("图片文件有错 第一个文件名:" + picStyteType, "错误");
        }
    }

    private void ShowDiglog()
    {
        DialogMgr.Instance.ShowDialogTypeBtnTwo("是否打开文件", "转换完毕", btnOneClickAct: () =>
           {
               //Process.Start("Explorer", saveVideoName);
               Process.Start("Explorer", "/select," + saveVideoName);
           });
    }

    private class DatasStruct
    {
        public Thread thread;
        public string picFolderName;
        public string outFileName;

    }
}
