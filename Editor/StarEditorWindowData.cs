//****************** 代码文件申明 ************************
//* 文件：StarEditorWindowData                                       
//* 作者：wheat
//* 创建时间：2024/09/18 08:05:37 星期三
//* 描述：用来存储编辑器窗口在桌面的一些数据
//*****************************************************
using UnityEngine;

namespace KFrame.StarTable
{
    [System.Serializable]
    public class StarEditorWindowData
    {
        /// <summary>
        /// 对应的编辑器的Key
        /// </summary>
        public string EditorKey;
        /// <summary>
        /// 在桌面上的位置
        /// </summary>
        public int TableIndex;
    }
}

