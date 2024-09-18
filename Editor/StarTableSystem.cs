//****************** 代码文件申明 ************************
//* 文件：StarTableSystem                                       
//* 作者：wheat
//* 创建时间：2024/08/26 14:13:49 星期一
//* 描述：用来处理一些基础的东西
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Sirenix.Utilities;
using UnityEditor;
using Object = UnityEngine.Object;

namespace KFrame.StarTable
{
    [InitializeOnLoad]
    public static class StarTableSystem
    {
        public static readonly string SavePath = UnityEngine.Application.dataPath + "/StarTableData.json";
        /// <summary>
        /// 总的用户数据
        /// </summary>
        [SerializeField]
        private static StarTableData data;
        public static StarTableData Data => data;

        static StarTableSystem()
        {
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private static void Init()
        {
            //先获取data，没有的话那就新建
            string jsonData = "";
            if (File.Exists(SavePath))
            {
                jsonData = File.ReadAllText(SavePath);
            }
            data = JsonUtility.FromJson<StarTableData>(jsonData);
            if (data == null)
            {
                data = new StarTableData();
            }
            
            //然后初始化数据
            data.Init();
            
            //再搜寻一下添加到桌面上的编辑器GUI
            FindEditorWindowGUI();
        }
        /// <summary>
        /// 搜寻编辑器窗口GUI
        /// </summary>
        private static void FindEditorWindowGUI()
        {
            //记录目前桌面上被占用的id
            HashSet<int> tableIndexes = data.TableIndexes;
            
            //获取现在有的数据，如果已经编辑器已经没有了，那就要从数据里面移除
            HashSet<string> dataToRemove = new HashSet<string>();
            foreach (StarEditorWindowData data in data.EditorOnTableData)
            {
                dataToRemove.Add(data.EditorKey);
            }
            //获取所有程序集
            System.Reflection.Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            Type baseType = typeof(EditorWindow);
            //遍历程序集
            foreach (System.Reflection.Assembly assembly in asms)
            {
                //遍历程序集下的每一个类型
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    //如果是继承自这个类型的，且不是抽象的
                    if (baseType.IsAssignableFrom(type)
                        && !type.IsAbstract)
                    {
                        //那就尝试获取StarEditorWindowAttribute，然后就可以添加GUI
                        StarEditorWindowAttribute attribute = type.GetCustomAttribute<StarEditorWindowAttribute>();
                        //如果没有这个Attribute那就跳过
                        if(attribute == null) continue;
                        //尝试反射获取打开窗口的方法
                        MethodInfo showWindowMethod = type.GetMethod(attribute.ShowWindowMethodName,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        //如果没有那就跳过
                        if (showWindowMethod == null) continue;
                        //记录访问的data
                        int tableIndex = 0;
                        string key = type.GetEditorKey();
                        dataToRemove.Remove(key);
                        //尝试获取data
                        if (data.EditorDataDic.TryGetValue(key, out StarEditorWindowData windowData))
                        {
                            tableIndex = windowData.TableIndex;
                        }
                        //如果没有那就新建
                        else
                        {
                            //新建data存入
                            windowData = new StarEditorWindowData();
                            data.EditorDataDic[key] = windowData;
                            data.EditorOnTableData.Add(windowData);
                        }
                        //如果位置被占用了，那就换一下试试
                        while (tableIndexes.Contains(tableIndex))
                        {
                            tableIndex++;
                        }
                        //创建GUI，更新桌面id，然后加入列表
                        StarEditorWindowGUI gui = new StarEditorWindowGUI(attribute.GUIName, key, showWindowMethod, tableIndex);
                        windowData.TableIndex = tableIndex;
                        tableIndexes.Add(tableIndex);
                        data.AssetOnTable.Add(gui);
                        
                    }
                }
            }
            
            //遍历剩余的已经不在了，需要移除的编辑器
            foreach (string key in dataToRemove)
            {
                //获取他们的data然后移除
                if (data.EditorDataDic.TryGetValue(key, out StarEditorWindowData windowData))
                {
                    data.EditorDataDic.Remove(key);
                    data.EditorOnTableData.Remove(windowData);
                }
            }
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        public static void SaveData()
        {
            File.WriteAllText(SavePath,JsonUtility.ToJson(data));
        }
        
        [UnityEditor.Callbacks.OnOpenAsset(-100)]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            //获取Object，然后更新保存数据
            Object obj = EditorUtility.InstanceIDToObject(instanceId);
            //如果是EditorWindow那就直接跳过处理
            if (obj is EditorWindow) return false;
            
            //处理操作
            data.OnOpenAsset(obj);
            
            //保存操作
            SaveData();
            
            //要返回false，否则代表OpenAsset已完成
            return false;
        }
        /// <summary>
        /// 获取编辑器窗口的key
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>一个不会重复的编辑器窗口的key</returns>
        private static string GetEditorKey(this Type type)
        {
            return type.Assembly.FullName + "." + type.FullName;
        }
    }
}

