//****************** 代码文件申明 ************************
//* 文件：StarGUIStyle                                       
//* 作者：wheat
//* 创建时间：2024/08/27 09:32:12 星期二
//* 描述：统一管理StarTable的GUI相关的GUIStyle
//*****************************************************

using UnityEditor;
using UnityEngine;
namespace KFrame.StarTable
{
    public static class StarGUIStyle
    {
        
#if UNITY_2018_3_OR_NEWER
        internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
        internal static readonly float spacing = 2.0f;
#endif
        internal static readonly GUIStyle middleBackgroundStyle;
        internal static readonly GUIStyle elementBackgroundStyle;
        internal static readonly Color unselectedColor = new Color(0.6f, 0.6f, 0.6f);
        internal static readonly Color selectedColor = Color.white;
        
        private static GUIStyle popup;
        internal static GUIStyle Popup
        {
            get
            {
                if (popup == null)
                {
                    popup = new GUIStyle(EditorStyles.miniButton)
                    {
                        alignment = TextAnchor.MiddleLeft
                    };
                }

                return popup;
            }
        }
        
        static StarGUIStyle()
        {
            //创建一些Style提供绘制使用
            middleBackgroundStyle = new GUIStyle("RL Background");
            elementBackgroundStyle = new GUIStyle("RL Element");
        }
    }
}

