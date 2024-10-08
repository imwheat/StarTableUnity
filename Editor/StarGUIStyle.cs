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
        private static GUIStyle middleBackgroundStyle;
        private static GUIStyle elementBackgroundStyle;
        private static GUIStyle labelStyle;
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

        internal static GUIStyle LabelStyle
        {
            get
            {
                if (labelStyle == null)
                {
                    labelStyle = new GUIStyle("ControlLabel")
                    {
                        fontSize = 12,
                        alignment = TextAnchor.UpperCenter,
                    };
                }

                return labelStyle;
            }
        }
        internal static GUIStyle MiddleBackgroundStyle
        {
            get
            {
                if (middleBackgroundStyle == null)
                {
                    middleBackgroundStyle = new GUIStyle("RL Background");
                }

                return middleBackgroundStyle;
            }
        }
        internal static GUIStyle ElementBackgroundStyle
        {
            get
            {
                if (elementBackgroundStyle == null)
                {
                    elementBackgroundStyle = new GUIStyle("RL Element");
                }

                return elementBackgroundStyle;
            }
        }
        
    }
}

