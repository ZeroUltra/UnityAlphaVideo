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
    private List<string> movFullNameList = new List<string>(); //全路径名字 c:/XXX/xxx/test.mov
    private List<string> movNameList = new List<string>();  //test
    private string ffmpegPath;

    public int codeRate = 1000;
    public event Action<string[]> OnSeleteMovEnd;
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
        movFullNameList.Clear();
        movNameList.Clear();
        movFullNameList.AddRange(paths);  //添加所有全路径
        movFullNameList.ForEach((path) => movNameList.Add(Path.GetFileNameWithoutExtension(path))); //添加文件名
        seleteMovPath = Path.GetDirectoryName(paths[0]); //当前选择的路径
        OnSeleteMovEnd?.Invoke(paths);
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
        if (string.IsNullOrEmpty(saveMovPath) || string.IsNullOrEmpty(seleteMovPath)) return true;
        return false;
    }

    public void RunFFmpeg()
    {
        for (int i = 0; i < movFullNameList.Count; i++)
        {
            DatasStruct datas = new DatasStruct();
            Thread thread = new Thread(RunFFmpegExe);
            datas.thread = thread;
            datas.movFileName = movFullNameList[i];
            datas.outFileName = $"{saveMovPath}/{ movNameList[i]}.mp4";
            if (i == movFullNameList.Count - 1) datas.showEndDialog = true;
            thread.Start(datas);
        }
    }
    private void RunFFmpegExe(object obj)
    {
        DatasStruct data = obj as DatasStruct;
        Process p = new Process();
        p.StartInfo.FileName = ffmpegPath + "/ffmpeg.exe";
        p.StartInfo.Arguments = $"-i {data.movFileName} -vf \"split[a], pad = iw * 2:ih[b], [a] alphaextract, [b] overlay=w\" -b {codeRate}k -y {data.outFileName}";
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
