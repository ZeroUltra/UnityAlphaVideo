using System.Collections.Generic;
using UnityEngine;
using SFB;
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
public class Mov2Video
{
    private string seleteMovPath, saveMovPath;  //选择视频路径  保存路径
    private List<string> movFileList = new List<string>(); //全路径名字 c:/XXX/xxx/test.mov
    private string ffmpegPath;

    public int codeRate = 1000;
    public bool addchunk4 = false;
    /// <summary>
    /// 当选择mov文件结束
    /// </summary>
    public event Action<string[]> OnAddMovFiles;
    /// <summary>
    /// 当选择保存路径结束
    /// </summary>
    public event Action<string> OnSeleteSaveFolderEnd;

    private ExtensionFilter[] extensions = new[] {
                                                    new ExtensionFilter("Mov Files", "Mov"),
                                                    new ExtensionFilter("All Files", "*" ),
                                                 };

    public Mov2Video()
    {
        //ffmpegPath = GameManager.FFMpegPath;
        ////检查ffmpeg.exe是否存在
        //if (!File.Exists(ffmpegPath + "/ffmpeg.exe"))
        //{
        //    DialogMgr.Instance.ShowDialogTypeBtnOne($"{ffmpegPath}\n文件夹不存在ffmpeg,请检查", "错误");
        //}
    }



    #region 选择mov文件
    public void OpenMovFile()
    {
        if (string.IsNullOrEmpty(seleteMovPath)) { seleteMovPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); }
        GameManager.Instance.DialogOpenFile("请选择一个mov", seleteMovPath, extensions, SeleteMovEnd, true);
    }
    //选择文件mov文件结束
    private void SeleteMovEnd(string[] paths)
    {
        if (paths.Length <= 0) return;
        movFileList.AddRange(paths);  //添加所有全路径
        seleteMovPath = Path.GetDirectoryName(paths[0]); //记住当前选择的路径
        OnAddMovFiles?.Invoke(movFileList.ToArray());
    }
    #endregion

    #region 保存路径
    /// <summary>
    /// 打开保存路径
    /// </summary>
    public void OpenSaveFolder()
    {
        if (string.IsNullOrEmpty(saveMovPath)) { saveMovPath = GameManager.DesktopPath; }
        GameManager.Instance.DialogOpenFolder("选择保存路径", saveMovPath, SeleteFolderEnd, false);
    }
    private void SeleteFolderEnd(string[] paths)
    {
        if (paths.Length <= 0) return;
        saveMovPath = paths[0];
        OnSeleteSaveFolderEnd?.Invoke(paths[0]);
    }
    #endregion

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

    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="exName">目标文件</param>
    public void Convert(string exName)
    {
        if (GetPathIsNull()) return;
        for (int i = 0; i < movFileList.Count; i++)
        {
            DatasStruct datas = new DatasStruct();
            Thread thread = new Thread(RunFFmpegExe);

            datas.thread = thread;
            datas.inputMovFileName = movFileList[i];
            datas.outVideoFileName = $"{saveMovPath}/{ Path.GetFileNameWithoutExtension(movFileList[i])}{exName}";
            datas.addChunk4 = addchunk4;

            //是否显示dialog 多个mov只展示最后一个dialog  应该根据时间 需要修改
            if (i == movFileList.Count - 1) datas.showEndDialog = true;

            thread.Start(datas);
        }
    }

    private void RunFFmpegExe(object obj)
    {
        DatasStruct data = obj as DatasStruct;
        Process p = new Process();
        p.StartInfo.FileName = GameManager.FFMpegPath;
        Debug.Log(data.outVideoFileName);
        string exName = Path.GetExtension(data.outVideoFileName);
        switch (exName)
        {
            case ".mp4":
                p.StartInfo.Arguments = $"-i {data.inputMovFileName} -vf  \"scale=trunc(iw/2)*2:trunc(ih/2)*2\" -vf \"split[a], pad = iw * 2:ih[b], [a] alphaextract, [b] overlay=w\" -b {codeRate}k -y {data.outVideoFileName}";
                break;
            case ".webm":
                //-i text.mov -auto-alt-ref 0 -c:v libvpx -b 1000k  export.webm  //-y是覆盖原来的视频
                p.StartInfo.Arguments = $"-i {data.inputMovFileName} -auto-alt-ref 0 -c:v libvpx  -b {codeRate}k -y {data.outVideoFileName}";
                break;
            case ".mov":
                //ffmpeg -i input.mov -vcodec hap -format hap_alpha output-hap.mov
                if (!data.addChunk4)
                    p.StartInfo.Arguments = $"-i {data.inputMovFileName} -vcodec hap -format hap_alpha -y {data.outVideoFileName}";
                else
                    p.StartInfo.Arguments = $"-i {data.inputMovFileName} -vcodec hap -format hap_alpha -chunks 4 -y {data.outVideoFileName}";
                break;
        }

        //scale=trunc(iw/2)*2:trunc(ih/2)*2 否则可能遇到“width not divisible by 2”

        Debug.Log("ffmeeg 信息:  " + p.StartInfo.Arguments);
        p.StartInfo.CreateNoWindow = false;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.Start();
        Debug.Log("ffmeeg 信息: 开始转换");
        p.WaitForExit();
        Debug.Log("ffmeeg 信息: 转换结束");
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

    /// <summary>
    /// 结构(线程中传递)
    /// </summary>
    private class DatasStruct
    {
        public Thread thread;
        public string inputMovFileName;
        public string outVideoFileName;
        public bool addChunk4 = false;
        public bool showEndDialog = false;
    }
}
