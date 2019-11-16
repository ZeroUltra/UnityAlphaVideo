using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.U2D;
using UnityEditor.Callbacks;
using System;
using Object = UnityEngine.Object;
using System.Threading;
using UnityEditor.U2D;

public class EditorTools
{

    #region 创建文件夹
    [MenuItem("Tools/创建文件夹", false, 200)]
    static void CreateFolder()
    {
        //"Models","Movie" "Textures",
        string[] folders = { "Editor", "Scripts", "Scenes", "UI", "Mats", "Resources", "Plugins", "Prefabs" };
        string path = "Assets";
        Debug.Log("开始创建文件夹");
        for (int i = 0; i < folders.Length; i++)
        {
            if (!Directory.Exists(Application.dataPath + "/" + folders[i]))
            {
                AssetDatabase.CreateFolder(path, folders[i]);
            }
        }
        Debug.Log("创建成功");
    }
    #endregion

    #region 设置材质贴图
    [MenuItem("Tools/设置材质贴图", false, 200)]
    static void SetTexture()
    {
        Object matgo = Selection.activeObject;
        Debug.Log(matgo.name);
        Material mat = matgo as Material;
        string goname = matgo.name;
        List<Texture> textures = new List<Texture>();
        foreach (var item in Resources.LoadAll<Texture>("image"))
        {
            if (item.name.StartsWith(goname))
            {
                string[] strs = item.name.Split('_');
                string endStr = strs[strs.Length - 1];
                if (endStr == "Color")
                {
                    mat.SetTexture("_MainTex", item);
                }
                else if (endStr == "Metallic")
                {
                    mat.SetTexture("_MetallicGlossMap", item);
                }
                else if (endStr == "AO")
                {
                    mat.SetTexture("_OcclusionMap", item);
                }
                else if (endStr == "Normal")
                {
                    mat.SetTexture("_BumpMap", item);
                }
            }
        }
    }
    #endregion

    #region 创建图集
#if UNITY_2018
    [MenuItem("Tools/创建图集", false, 200)]
    static void CreateSpriteAtlas()
    {
        Object[] objs = Selection.objects;
        string selePath = AssetDatabase.GetAssetPath(objs[0]);
        string spriteAtlasName = Directory.GetParent(AssetDatabase.GetAssetPath(objs[0])).Name;  //图集名字
        if ((AssetDatabase.LoadAssetAtPath(selePath, typeof(Sprite))) == null)
        {
            Debug.LogError("请选择需要打包的Sprite图片");
            return;  //返回
        }

        int index = selePath.LastIndexOf("/");
        selePath = selePath.Remove(index, selePath.Length - index);

        SpriteAtlas spriteAtlas = new SpriteAtlas();

        SpriteAtlasExtensions.Add(spriteAtlas, objs);
        AssetDatabase.CreateAsset(spriteAtlas, selePath + "/" + spriteAtlasName + ".spriteatlas");
        SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);
        Debug.Log(ColorToolRich.ColorYellow("图集打包成功"));
    }
#endif
    #endregion


    #region 程序初始化设置
    [MenuItem("Tools/程序初始化设置", false, 200)]
    static void ProgramInit()
    {
        string dataPath = Application.dataPath + "/../Data";
        string settingPath = dataPath + "/Setting";
        string updatePath = dataPath + "/Update";
        Directory.CreateDirectory(settingPath);
        Directory.CreateDirectory(dataPath);
        Directory.CreateDirectory(updatePath);

        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;//分辨率选择框关闭
        PlayerSettings.usePlayerLog = false; //不记录日志 自定义记录
        PlayerSettings.runInBackground = true;
        PlayerSettings.captureSingleScreen = true;
        PlayerSettings.forceSingleInstance = true;
        PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest; //4.6
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);
#if UNITY_2017
         EditorApplication.OpenProject(Environment.CurrentDirectory);
#endif
        Debug.Log("程序设置完成");
    }

    #endregion

    #region 当脚本加载时
    //[DidReloadScripts]
    //private static void OnLoadScripts()
    //{
    //    Debug.Log(111);
    //}
    #endregion
}
#region 添加定义
public class AddFileHeadComment : UnityEditor.AssetModificationProcessor
{
    /// <summary>
    /// 此函数在asset被创建完，文件已经生成到磁盘上，但是没有生成.meta文件和import之前被调用
    /// </summary>
    /// <param name="newFileMeta">newfilemeta 是由创建文件的path加上.meta组成的</param>
    public static void OnWillCreateAsset(string newFileMeta)
    {
        string newFilePath = newFileMeta.Replace(".meta", "");
        string fileExt = Path.GetExtension(newFilePath);
        if (fileExt != ".cs")
        {
            return;
        }
        //注意，Application.datapath会根据使用平台不同而不同
        string realPath = Application.dataPath.Replace("Assets", "") + newFilePath;
        string scriptContent = File.ReadAllText(realPath);

        //这里实现自定义的一些规则
        // scriptContent = scriptContent.Replace("#SCRIPTFULLNAME#", Path.GetFileName(newFilePath));
        //scriptContent = scriptContent.Replace("#COMPANY#", PlayerSettings.companyName);
        scriptContent = scriptContent.Replace("Luojie", "Luojie");
        scriptContent = scriptContent.Replace("1.0", "1.0");
        scriptContent = scriptContent.Replace("2018.4.6f1", Application.unityVersion);
        scriptContent = scriptContent.Replace("2019-10-17", System.DateTime.Now.ToString("yyyy-MM-dd"));

        File.WriteAllText(realPath, scriptContent);
    }
}

// public class ScriptLoad : AssetPostprocessor
//{
//    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
//    {
//        //当脚本创建完成 然后设置到Scripts文件夹下面
//        //限制 每次只会创建一个cs文件
//        if (importedAssets.Length == 1)
//        {
//            string filepath = importedAssets[0];

//            if (Path.GetExtension(filepath) == ".cs")
//            {
//                if (filepath.StartsWith("Assets/Scripts/")) return;
//                string fileName = Path.GetFileName(filepath);
//                Debug.Log(filepath);
//                if (!Directory.Exists(Application.dataPath + "/Scripts"))
//                    AssetDatabase.CreateFolder("Assets", "Scripts");

//                //AssetDatabase.MoveAsset(filepath, "Assets/Scripts/" + fileName);
//                //AssetDatabase.Refresh();
//                //SaveScripts(filepath, fileName);
//                Thread th = new Thread(SaveScripts);
//                th.Start(filepath + "_" + fileName);
//            }
//        }
//    }

//    static void SaveScripts(object @object)
//    {
//        string str = @object as string;
//        string filepath = str.Split('_')[0];
//        string fileName = str.Split('_')[1];
//        Debug.Log(11);
//        Thread.Sleep(4000);
//        Debug.Log(22);
//        AssetDatabase.MoveAsset(filepath, "Assets/Scripts/" + fileName);
//        AssetDatabase.Refresh();
//    }

//}

#endregion

