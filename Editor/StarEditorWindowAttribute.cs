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
    /// <summary>
    /// 给编辑窗口加上这个特性可以在桌面上显示编辑器的图标
    /// 只支持static，传参为空的方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class StarEditorWindowAttribute : PropertyAttribute
    {
        public string GUIName;
        public string ShowWindowMethodName;
        /// <summary>
        /// 给编辑窗口加上这个特性可以在桌面上显示编辑器的图标
        /// 只支持static，传参为空的方法
        /// </summary>
        /// <param name="guiName">桌面图标名称</param>
        /// <param name="showWindowMethodName">打开窗口的方法名称</param>
        public StarEditorWindowAttribute(string guiName, string showWindowMethodName)
        {
            GUIName = guiName;
            ShowWindowMethodName = showWindowMethodName;
        }
    }
}

