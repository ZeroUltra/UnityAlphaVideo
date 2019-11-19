using System.Collections.Generic;
using UnityEngine;
using SFB;
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Object = System.Object;
using Debug = UnityEngine.Debug;
public class Mov2Mp4
{
    private string seleteMovPath, saveMovPath;  //选择视频路径  保存路径
    private List<string> movFileList = new List<string>(); //全路径名字 c:/XXX/xxx/test.mov
    private string ffmpegPath;

    public int codeRate = 1000;
    public event Action<string[]> OnAddMovFiles;
    public event Action<string> OnSeleteFolderEnd;

    private ExtensionFilter[] extensions = new[] {
                                                    new ExtensionFilter("Mov Files", "Mov"),
                                                    new ExtensionFilter("All Files", "*" ),
                                                 };


    public Mov2Mp4()
    {
        ffmpegPath = GameManager.FFMpegPath;
        if (!File.Exists(ffmpegPath + "/ffmpeg.exe"))
        {
            DialogMgr.Instance.ShowDialogTypeBtnOne($"{ffmpegPath}\n文件夹不存在ffmpeg,请检查", "错误");
        }
    }
    public void OpenMovFile()
    {
        if (string.IsNullOrEmpty(seleteMovPath)) { seleteMovPath = Application.dataPath; }
        GameManager.Instance.DialogOpenFile("请选择一个mov", seleteMovPath, extensions, SeleteMovEnd, true);
    }

    private void SeleteMovEnd(string[] paths)
    {
        if (paths.Length <= 0) return;
        movFileList.AddRange(paths);  //添加所有全路径
        seleteMovPath = Path.GetDirectoryName(paths[0]); //当前选择的路径
        OnAddMovFiles?.Invoke(movFileList.ToArray());
    }

    public void OpenFolder()
    {
        if (string.IsNullOrEmpty(saveMovPath)) { saveMovPath = Application.dataPath; }
        GameManager.Instance.DialogOpenFolder("选择保存路径", saveMovPath, SeleteFolderEnd, false);
    }
    private void SeleteFolderEnd(string[] paths)
    {
        if (paths.Length <= 0) return;
        saveMovPath = paths[0];
        OnSeleteFolderEnd?.Invoke(paths[0]);
    }
    /// <summary>
    /// 检查路径是否为空
    /// </summary>
    /// <returns></returns>
    public bool GetPathIsNull()
    {
        if (string.IsNullOrEmpty(saveMovPath) || movFileList.Count == 0)
        {
            DialogMgr.Instance.ShowDialogTypeBtnOne("请选择目录", "错误提示");
            return true;
        }
        return false;
    }

    public void Convert()
    {
        if (GetPathIsNull()) return;
        for (int i = 0; i < movFileList.Count; i++)
        {
            DatasStruct datas = new DatasStruct();
            Thread thread = new Thread(RunFFmpegExe);
            datas.thread = thread;
            datas.movFileName = movFileList[i];
            datas.outFileName = $"{saveMovPath}/{ Path.GetFileNameWithoutExtension(movFileList[i])}.mp4";
            if (i == movFileList.Count - 1) datas.showEndDialog = true;
            thread.Start(datas);
        }
    }
    private void RunFFmpegExe(object obj)
    {
        DatasStruct data = obj as DatasStruct;
        Process p = new Process();
        p.StartInfo.FileName = ffmpegPath + "/ffmpeg.exe";
        // p.StartInfo.Arguments = $"-i {data.movFileName} -vf  \"split[a], pad = iw * 2:ih[b], [a] alphaextract, [b] overlay=w\" -b {codeRate}k -y {data.outFileName}";
        //scale=trunc(iw/2)*2:trunc(ih/2)*2 否则可能遇到“width not divisible by 2”
        p.StartInfo.Arguments = $"-i {data.movFileName} -vf  \"scale=trunc(iw/2)*2:trunc(ih/2)*2\" -vf \"split[a], pad = iw * 2:ih[b], [a] alphaextract, [b] overlay=w\" -b {codeRate}k -y {data.outFileName}";
        Debug.Log("ffmeeg 信息:  " + p.StartInfo.Arguments);
        p.StartInfo.CreateNoWindow = false;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.Start();
        Debug.Log("ffmeeg 信息: start");
        p.WaitForExit();
        Debug.Log("ffmeeg 信息: Exit");
        p.Close();
        if (data.showEndDialog)
            Loom.QueueOnMainThread(() => ShowDiglog());
        data.thread.Abort();
    }

    public void ClearMovList()
    {
        movFileList.Clear();
        OnAddMovFiles?.Invoke(movFileList.ToArray());
    }

    public void AddDragFile(string[] movfile)
    {
      
        movFileList.AddRange(movfile);
        OnAddMovFiles?.Invoke(movFileList.ToArray());
    }

    private void ShowDiglog()
    {
        DialogMgr.Instance.ShowDialogTypeBtnTwo("是否打开文件", "转换完毕", btnOneClickAct: () =>
           {
               Process.Start("Explorer", saveMovPath);
               //Process.Start("Explorer", $"/select,{saveMovPath}{ movNameList[0]}.mp4");
           });
    }
    private class DatasStruct
    {
        public Thread thread;
        public string movFileName;
        public string outFileName;
        public bool showEndDialog = false;
    }
}
