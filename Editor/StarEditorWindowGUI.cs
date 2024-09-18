//****************** 代码文件申明 ************************
//* 文件：StarEditorWindowGUI                                       
//* 作者：wheat
//* 创建时间：2024/09/18 07:23:48 星期三
//* 描述：用来记录、处理、绘制每个Asset的一些数据
//*****************************************************
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace KFrame.StarTable
{
    [System.Serializable]
    public class StarEditorWindowGUI : StarTableGUI
    {
        /// <summary>
        /// 打开窗口的Method
        /// </summary>
        [NonSerialized]
        public MethodInfo ShowWindowMethod;
        /// <summary>
        /// 存在存档数据里面的key
        /// </summary>
        public string EditorKey;
        
        public StarEditorWindowGUI(string guid, string name, Object obj) : base(guid, name, obj)
        {
            Icon = StarEditorIcon.UnityPrefabIcon;
            IsEditor = true;
        }

        public StarEditorWindowGUI(Object obj) : base(obj)
        {
        }

        public StarEditorWindowGUI(string name, string editorKey, MethodInfo showWindowMethod, int tableIndex) : this("", name, null)
        {
            ShowWindowMethod = showWindowMethod;
            EditorKey = editorKey;
            TableIndex = tableIndex;
        }

        public override void OpenAsset()
        {
            ShowWindowMethod?.Invoke(null, null);
        }
    }
}

