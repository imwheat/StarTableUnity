//****************** 代码文件申明 ************************
//* 文件：StarEditorIcon                                       
//* 作者：wheat
//* 创建时间：2024/08/27 09:47:18 星期二
//* 描述：用于存放一些StarTable会使用到的编辑器图标
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace KFrame.StarTable
{
    public static class StarEditorIcon
    {
        private static Texture2D unityAssetIcon;
        private static Texture2D unityCSFile;
        private static Texture2D unityFolderIcon;
        private static Texture2D unityGameObjectIcon;
        private static Texture2D unityPrefabIcon;

        /// <summary>
        /// GameObject图标
        /// </summary>
        
        public static Texture2D UnityAssetIcon
        {
            get
            {
                if ((UnityEngine.Object) StarEditorIcon.unityAssetIcon == (UnityEngine.Object) null)
                    StarEditorIcon.unityAssetIcon = EditorGUIUtility.FindTexture("ScriptableObject Icon");
                return StarEditorIcon.unityAssetIcon;
            }
        }
        
        /// <summary>
        /// GameObject图标
        /// </summary>
        
        public static Texture2D UnityCSFileIcon
        {
            get
            {
                if ((UnityEngine.Object) StarEditorIcon.unityCSFile == (UnityEngine.Object) null)
                    StarEditorIcon.unityCSFile = EditorGUIUtility.FindTexture("cs Script Icon");
                return StarEditorIcon.unityCSFile;
            }
        }
        
        /// <summary>
        /// GameObject图标
        /// </summary>
        
        public static Texture2D UnityGameObjectIcon
        {
            get
            {
                if ((UnityEngine.Object) StarEditorIcon.unityGameObjectIcon == (UnityEngine.Object) null)
                    StarEditorIcon.unityGameObjectIcon = EditorGUIUtility.FindTexture("GameObject Icon");
                return StarEditorIcon.unityGameObjectIcon;
            }
        }
        /// <summary>
        /// 文件夹图标
        /// </summary>
        public static Texture2D UnityFolderIcon
        {
            get
            {
                if ((UnityEngine.Object) StarEditorIcon.unityFolderIcon == (UnityEngine.Object) null)
                    StarEditorIcon.unityFolderIcon = AssetPreview.GetMiniThumbnail(AssetDatabase.LoadAssetAtPath("Assets", typeof (UnityEngine.Object)));
                return StarEditorIcon.unityFolderIcon;
            }
        }
        /// <summary>
        /// 预制体图标
        /// </summary>
        public static Texture2D UnityPrefabIcon
        {
            get
            {
                if ((UnityEngine.Object) StarEditorIcon.unityPrefabIcon == (UnityEngine.Object) null)
                    StarEditorIcon.unityPrefabIcon = EditorGUIUtility.FindTexture("Prefab Icon");
                return StarEditorIcon.unityPrefabIcon;
            }
        }
    }
}

