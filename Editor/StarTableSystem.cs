//****************** 代码文件申明 ************************
//* 文件：StarTableSystem                                       
//* 作者：wheat
//* 创建时间：2024/08/26 14:13:49 星期一
//* 描述：用来处理一些基础的东西
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace KFrame.StarTable
{
    [InitializeOnLoad]
    public static class StarTableSystem
    {
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
            if (EditorPrefs.HasKey(StarTableData.SaveKey))
            {
                jsonData = EditorPrefs.GetString(StarTableData.SaveKey);
            }
            data = JsonUtility.FromJson<StarTableData>(jsonData);
            if (data == null)
            {
                data = new StarTableData();
            }
            
            //然后初始化数据
            data.Init();
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        public static void SaveData()
        {
            EditorPrefs.SetString(StarTableData.SaveKey, JsonUtility.ToJson(data));
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
    }
}

