//****************** 代码文件申明 ************************
//* 文件：StarEditorWindowAttribute                                       
//* 作者：wheat
//* 创建时间：2024/09/18 07:25:53 星期三
//* 描述：给编辑窗口加上这个特性可以在桌面上显示编辑器的图标
//*****************************************************

using System;
using UnityEngine;

namespace KFrame.StarTable
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class StarEditorWindowAttribute : PropertyAttribute
    {
        public string GUIName;
        public string ShowWindowMethodName;
        public StarEditorWindowAttribute(string guiName, string showWindowMethodName)
        {
            GUIName = guiName;
            ShowWindowMethodName = showWindowMethodName;
        }
    }
}

