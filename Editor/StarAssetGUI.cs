//****************** 代码文件申明 ************************
//* 文件：StarAssetGUI                                       
//* 作者：wheat
//* 创建时间：2024/08/26 14:44:56 星期一
//* 描述：用来记录、处理、绘制每个Asset的一些数据
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEditor;
using Object = UnityEngine.Object;

namespace KFrame.StarTable
{
    [System.Serializable]
    public class StarAssetGUI
    {
        #region Asset相关

        /// <summary>
        /// Asset本身
        /// </summary>
        [NonSerialized]
        public Object AssetObj;
        /// <summary>
        /// Asset的名称
        /// </summary>
        public string Name;
        /// <summary>
        /// Asset的GUID
        /// </summary>
        public string GUID;
        /// <summary>
        /// Asset的优先级，越大越高
        /// </summary>
        [NonSerialized]
        public int Order;

        #endregion

        #region 桌面图标相关

        /// <summary>
        /// 在桌面上的位置下标
        /// </summary>
        public int TableIndex;
        /// <summary>
        /// 上一个手动放置的位置
        /// </summary>
        public Vector2 PrevPlacePos;
        /// <summary>
        /// 在桌面上的Rect
        /// </summary>
        public Rect TableRect;
        
        #endregion
        
        #region GUI绘制相关

        /// <summary>
        /// 绘制区域的Rect
        /// </summary>
        public Rect DrawRect;
        /// <summary>
        /// GUI的绘制类型
        /// </summary>
        public StarAssetDrawType DrawType;
        /// <summary>
        /// 被选择了
        /// </summary>
        public bool Selected;
        /// <summary>
        /// GUI显示的Icon
        /// </summary>
        public Texture Icon;
        /// <summary>
        /// 图标显示区域
        /// </summary>
        protected Rect iconRect;
        /// <summary>
        /// 名称显示区域
        /// </summary>
        protected Rect labelRect;

        #endregion

        public StarAssetGUI(string guid, string name, Object obj)
        {
            AssetObj = obj;
            GUID = guid;
            Name = name;
            Icon = AssetPreview.GetMiniThumbnail(obj);
        }
        public StarAssetGUI(Object obj) : this(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)), obj.name, obj)
        {

        }
        /// <summary>
        /// 更新Rect
        /// </summary>
        public void UpdateRect(Rect drawRect)
        {
            //如果相等那就返回
            if (drawRect == DrawRect)
            {
                return;
            }
            
            DrawRect = drawRect;

            switch (DrawType)
            {
                case StarAssetDrawType.Horizontal:
                    //显示图标
                    float iconHeight = DrawRect.height;
                    iconRect = DrawRect;
                    iconRect.width = iconHeight;
                    //显示名称
                    labelRect = DrawRect;
                    labelRect.xMin += iconHeight + StarGUIStyle.spacing;
                    break;
                case StarAssetDrawType.Square:
                    //显示图标
                    iconHeight = Mathf.RoundToInt((DrawRect.height - StarGUIStyle.spacing - 8f) /3 * 2f);
                    iconRect = DrawRect;
                    iconRect.xMin += (iconRect.width - iconHeight) / 2f;
                    iconRect.yMin += StarGUIStyle.spacing * 3f;
                    iconRect.width = iconHeight;
                    iconRect.height = iconHeight;
                    //显示名称
                    labelRect = DrawRect;
                    labelRect.xMin += StarGUIStyle.spacing * 3f;
                    labelRect.xMax -= StarGUIStyle.spacing * 3f;
                    labelRect.yMax -= StarGUIStyle.spacing * 3f;
                    labelRect.yMin += StarGUIStyle.spacing * 3f + iconHeight;
                    break;
            }
        }
        /// <summary>
        /// 绘制Asset选项GUI
        /// </summary>
        public void DrawAssetOptionGUI(Rect drawRect)
        {
            UpdateRect(drawRect);

            //根据是否被选择了调整GUI颜色
            GUI.color = Selected ? StarGUIStyle.selectedColor : StarGUIStyle.unselectedColor;

            switch (DrawType)
            {
                //横向的
                case StarAssetDrawType.Horizontal:
                    //绘制GUI背景
                    if (Event.current.type == EventType.Repaint)
                    {
                        StarGUIStyle.elementBackgroundStyle.Draw(DrawRect, false, false, false, false);
                    }
                    //显示图标
                    GUI.Box(iconRect, Icon);
                    //显示名称
                    GUI.Label(labelRect, Name);
                    
                    break;
                //方块的
                case StarAssetDrawType.Square:
                    
                    //绘制GUI背景
                    if (Event.current.type == EventType.Repaint)
                    {
                        StarGUIStyle.elementBackgroundStyle.Draw(DrawRect, false, false, false, false);
                    }
                    
                    //显示图标
                    GUI.Box(iconRect, Icon);
                    //显示名称
                    GUI.Label(labelRect, Name);
                    
                    break;
            }
            
            //恢复GUI颜色
            GUI.color = Color.white;

        }
        /// <summary>
        /// 打开Asset
        /// </summary>
        public virtual void OpenAsset()
        {
            if (AssetObj == null)
            {
                EditorUtility.DisplayDialog("错误", "无法打开！该桌面图标的Asset疑似已经丢失了。", "确认");
                return;
            }
            
            AssetDatabase.OpenAsset(AssetObj);
        }
        /// <summary>
        /// 删除Asset
        /// </summary>
        public void DeleteAsset()
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(AssetObj));
        }

    }
}

