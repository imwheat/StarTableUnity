//****************** 代码文件申明 ************************
//* 文件：StarGUIPopupWindow                                       
//* 作者：wheat
//* 创建时间：2024/09/09 15:01:08 星期一
//* 描述：处理StarGUI右键点击菜单的一些选项
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace KFrame.StarTable
{
    public class StarGUIPopupWindow : PopupWindowContent
    {
        /// <summary>
        /// gui的值
        /// </summary>
        protected string[] guiValues;
        /// <summary>
        /// gui的Content
        /// </summary>
        protected string[] guiContents;
        /// <summary>
        /// 每个elment高度
        /// </summary>
        protected virtual float elementHeight => 24f;
        /// <summary>
        /// 每个elment之间的空隙
        /// </summary>
        protected virtual float padding => 3f;
        /// <summary>
        /// 回调函数
        /// </summary>
        protected Action<string> callback;
        public StarGUIPopupWindow()
        {

        }
        public StarGUIPopupWindow(IReadOnlyCollection<string> strings, Action<string> callback) 
        {
            InitGUI(strings, callback);
        }
        protected virtual void InitGUI(IReadOnlyCollection<string> strings, Action<string> callback)
        {
            guiValues = strings.ToArray<string>();
            guiContents = new string[guiValues.Length];
            for (int i = 0; i < guiValues.Length; i++)
            {
                guiContents[i] = guiValues[i];
            }
            this.callback = callback;
        }
        public override void OnGUI(Rect rect)
        {
            rect.yMin += padding;
            rect.xMin += padding;
            rect.xMax -= padding;

            //逐个绘制每个元素
            for (int i = 0; i < guiValues.Length; i++)
            {
                //计算rect
                Rect curRect = rect;
                curRect.height = elementHeight;

                //显示选项
                if(GUI.Button(curRect, guiContents[i], StarGUIStyle.Popup))
                {
                    //点击以后调用回调函数，然后关闭窗口
                    callback?.Invoke(guiValues[i]);
                    editorWindow.Close();
                }

                //更新rect
                rect.yMin += elementHeight + padding;
            }
        }

    }
}

