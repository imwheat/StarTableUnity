//****************** 代码文件申明 ************************
//* 文件：StarTableGUI                                       
//* 作者：wheat
//* 创建时间：2024/09/09 14:54:01 星期一
//* 描述：用来记录、处理、绘制每个Asset的一些数据
//*****************************************************
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace KFrame.StarTable
{
    [System.Serializable]
    public class StarTableGUI : StarAssetGUI
    {
        #region 参数

        /// <summary>
        /// 拖拽中
        /// </summary>
        [NonSerialized]
        public bool IsDragging;
        /// <summary>
        /// 拖拽时的Rect
        /// </summary>
        [NonSerialized]
        public Rect DragRect;

        #endregion
        
        #region 构造函数

        public StarTableGUI(string guid, string name, Object obj) : base(guid, name, obj)
        {
        }

        public StarTableGUI(Object obj) : base(obj)
        {
        }

        #endregion

        #region 重写一写操作
        

        #endregion
        
    }
}

