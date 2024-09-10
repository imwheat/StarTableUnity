//****************** 代码文件申明 ************************
//* 文件：StarTableWindow                                       
//* 作者：wheat
//* 创建时间：2024/08/26 14:07:54 星期一
//* 描述：用于收藏管理、快捷访问一些asset
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace KFrame.StarTable
{
    public class StarTableWindow : EditorWindow
    {
        #region 参数引用

        /// <summary>
        /// Label文字的高度
        /// </summary>
        private const float LabelHeight = 24f;
        private StarTableData data => StarTableSystem.Data;

        #endregion

        #region 操作相关
        
        /// <summary>
        /// 当前右键菜单选中的GUI
        /// </summary>
        private StarAssetGUI curRightMenuSelectGUI;
        /// <summary>
        /// 点击空白处显示的菜单项
        /// </summary>
        private readonly IReadOnlyCollection<string> clickSpaceOptions = new[] { "刷新"};
        /// <summary>
        /// 点击桌面GUI显示的菜单项
        /// </summary>
        private readonly IReadOnlyCollection<string> clickTableGUIOptions = new[] { "打开", "从桌面上删除", "从Unity中删除"};
        /// <summary>
        /// 点击最近访问列表GUI显示的菜单项
        /// </summary>
        private readonly IReadOnlyCollection<string> clickVisitGUIOptions = new[] { "打开", "从Unity中删除"};
        /// <summary>
        /// 当前选择的Asset
        /// </summary>
        private StarAssetGUI curSelectAsset;
        /// <summary>
        /// 拖拽中
        /// </summary>
        private bool isDragging;
        /// <summary>
        /// 检测双击中
        /// </summary>
        private bool checkDoubleClick;
        /// <summary>
        /// 检测双击的时间间隔，只要在这段时间内再点击一次算双击
        /// </summary>
        private const float CheckDoubleClickInterval = 0.2f;
        /// <summary>
        /// 检测双击的计时器
        /// </summary>
        private float checkDoubleClickTimer;
        /// <summary>
        /// Repaint请求
        /// </summary>
        private bool repaintRequest;

        #endregion

        #region GUI绘制相关

        /// <summary>
        /// 标准化统一当前编辑器的绘制Style
        /// </summary>
        private static class MStyle
        {
#if UNITY_2018_3_OR_NEWER
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
            internal static readonly float spacing = 2.0f;
#endif
            internal static readonly float btnWidth = 40f;
            internal static readonly float labelHeight = 20f;
            internal static readonly float lineWidth = 2f;

            #region 桌面相关

            internal static readonly float iconSize = 100f;
            internal static readonly float iconSpace = 25f;

            #endregion
            
        }

        /// <summary>
        /// 最近访问GUI宽度
        /// </summary>
        private float latestVisitGUIWidth = -1;
        /// <summary>
        /// 分割线区域
        /// </summary>
        private Rect lineRect;
        /// <summary>
        /// 拖拽分割线检测区域
        /// </summary>
        private Rect dragLineCheckRect;
        /// <summary>
        /// 正在拖拽line的区域
        /// </summary>
        private bool isDraggingLineRect;
        /// <summary>
        /// 拖拽线条起始位点
        /// </summary>
        private float draggingLineOriginX;
        /// <summary>
        /// 拖拽前线条的宽度
        /// </summary>
        private float prevLineWidth;
        /// <summary>
        /// 桌面一行图标数量
        /// </summary>
        private int tableRowCount;
        /// <summary>
        /// 桌面区域
        /// </summary>
        private Rect tableRect;
        /// <summary>
        /// 图标放大比例
        /// </summary>
        private float iconSizeRatio = 1.0f;
        /// <summary>
        /// 图标尺寸
        /// </summary>
        private float IconSize => iconSizeRatio * MStyle.iconSize;
        
        #endregion

        #region 暂时的Repaint处理

        private Vector2 mousePos;
        private const float checkUpdateInterval = 0.1f;
        private float checkUpdateTimer = 0f;

        #endregion

        #region 初始化

        public static void ShowWindow()
        {
            StarTableWindow window = EditorWindow.GetWindow<StarTableWindow>();
            window.titleContent = new GUIContent("星标桌面");
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            //读取存档中的GUI宽度
            latestVisitGUIWidth = data.LatestVisitGUIWidth;
            
            //更新Rect区域
            UpdateRect();
        }
        /// <summary>
        /// 更新Rect区域
        /// </summary>
        private void UpdateRect()
        {
            if (latestVisitGUIWidth <= 0)
            {
                latestVisitGUIWidth = position.width / 3f;
            }
            
            lineRect = new Rect(latestVisitGUIWidth, 0, MStyle.lineWidth, position.height);
            dragLineCheckRect = new Rect(lineRect.position - new Vector2(2f, 0), lineRect.size + new Vector2(4f, 0));
            //更新桌面区域
            tableRect = new Rect(latestVisitGUIWidth + MStyle.lineWidth, 0,
                position.width - (latestVisitGUIWidth + MStyle.lineWidth), position.height);
            int tRowCount = Mathf.FloorToInt((tableRect.width - MStyle.iconSpace) / IconSize);
            //如果桌面每行图标数量发生变化，那就更细桌面
            if (tRowCount != tableRowCount)
            {
                tableRowCount = tRowCount;
                UpdateTable();
                
            }
            
            //更新存档的GUI宽度
            data.LatestVisitGUIWidth = latestVisitGUIWidth;
        }

        #endregion

        #region 生命周期

        private void OnEnable()
        {
            //先进行初始化
            Init();
        }

        private void OnDisable()
        {
            //保存操作数据
            StarTableSystem.SaveData();
        }

        private void Update()
        {
            
            //处理输入Timer相关
            HanldeInputTimer();
            
            //处理重绘
            RepaintIfRequest();
        }

        #endregion

        #region 桌面相关

        /// <summary>
        /// 创建桌面GUI
        /// </summary>
        /// <param name="asset">要放到桌面上的Asset</param>
        /// <param name="placeIndex">放置的位置的下标</param>
        /// <param name="autoPlace">自动放置</param>
        /// <returns>新建的GUI</returns>
        private StarAssetGUI CreateTableGUI(Object asset, int placeIndex, bool autoPlace)
        {
            //如果为空直接返回null
            if (asset == null) return null;
            
            //新建GUI然后赋值
            StarTableGUI gui = new StarTableGUI(asset);
            gui.TableIndex = placeIndex;
            gui.DrawType = StarAssetDrawType.Square;
            Rect placeRect = GetIconRect(placeIndex);
            gui.TableRect = placeRect;
            gui.UpdateRect(new Rect(placeRect.position + tableRect.position, placeRect.size));
            if (!autoPlace)
            {
                gui.PrevPlacePos = placeRect.position;
            }
            
            //存进数据然后保存
            data.AssetOnTable.Add(gui);
            StarTableSystem.SaveData();
            
            return gui;
        }
        /// <summary>
        /// 更新桌面
        /// </summary>
        private void UpdateTable()
        {
            //遍历更新Rect
            foreach (StarTableGUI assetGUI in data.AssetOnTable)
            {
                assetGUI.UpdateRect(GetIconRect(assetGUI.TableIndex));
            }
        }
        /// <summary>
        /// 判断图标位置是否位于屏幕范围内
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool IsIconInTheScreen(int i)
        {
            //如果是行第一个直接返回true
            if (i % tableRowCount == 0) return true;
            
            //其他的检测是否在屏幕内
            return GetIconRect(i).xMax <= position.width;
        }
        /// <summary>
        /// 获取Icon的Rect根据鼠标位置
        /// </summary>
        /// <param name="mousePosition">鼠标位置</param>
        /// <returns>Icon在桌面上的Rect</returns>
        private int GetIconRectIndex(Vector2 mousePosition)
        {
            //先将鼠标位置转为相对桌面的位置
            Vector2 tablePos = mousePosition - tableRect.position;
            float slotSize = MStyle.iconSpace + IconSize;
            
            //然后换算成下标位置
            return Mathf.FloorToInt(tablePos.x / slotSize) +
                   Mathf.FloorToInt(tablePos.y / slotSize) * tableRowCount;
        }
        /// <summary>
        /// 获取到Icon的Rect根据下标位置
        /// </summary>
        /// <param name="i">下标</param>
        /// <returns>Icon在桌面上的Rect</returns>
        private Rect GetIconRect(int i)
        {
            return new Rect(MStyle.iconSpace + (IconSize + MStyle.iconSpace) * (i % tableRowCount),
                MStyle.iconSpace + (IconSize + MStyle.iconSpace) * (i / tableRowCount), IconSize, IconSize);
        }
        /// <summary>
        /// 获取到Icon的Rect根据鼠标位置
        /// </summary>
        /// <param name="mousePosition">鼠标位置</param>
        /// <returns>Icon在桌面上的Rect</returns>
        private Rect GetIconRect(Vector2 mousePosition)
        {
            return GetIconRect(GetIconRectIndex(mousePosition));
        }

        #endregion
        
        #region GUI绘制

        private void OnGUI()
        {
            //绘制最近访问列表
            DrawLatestVisitList();
            
            //绘制分割线
            DrawDividLine();

            //绘制桌面区域
            DrawTableRect();
            
            //处理鼠标输入
            HandleMouseInput();
            
        }

        /// <summary>
        /// 绘制最近访问列表
        /// </summary>
        private void DrawLatestVisitList()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("刷新"))
            {
                RepaintWindow();
            }
            
            EditorGUILayout.EndHorizontal();
            
            Rect rect = new Rect(0, 0, latestVisitGUIWidth, LabelHeight);
            EditorGUI.LabelField(rect,"最近访问");
            
            for (int i = 0; i < data.VisitLatestList.Count; i++)
            {
                rect.y += StarGUIStyle.spacing + LabelHeight;
                
                //如果超出绘制区域了，那就中断绘制
                if (rect.y + LabelHeight > position.height)
                {
                    break;
                }
                
                StarAssetGUI assetGUI = data.VisitLatestList[i];
                if (Event.current.type == EventType.Repaint)
                {
                    assetGUI.DrawAssetOptionGUI(rect);
                }
            }
            
            EditorGUILayout.EndVertical();

        }
        /// <summary>
        /// 绘制分割线
        /// </summary>
        private void DrawDividLine()
        {
            EditorGUI.DrawRect(lineRect, Color.black);
        }
        /// <summary>
        /// 绘制桌面区域
        /// </summary>
        private void DrawTableRect()
        {
            if(Event.current.type != EventType.Repaint) return;
            
            for (int i = 0; i < data.AssetOnTable.Count; i++)
            {
                data.AssetOnTable[i].DrawAssetOptionGUI(new Rect(tableRect.position + data.AssetOnTable[i].TableRect.position,
                    data.AssetOnTable[i].TableRect.size));
            }
        }
        /// <summary>
        /// 请求重绘Window
        /// </summary>
        private void RepaintWindow()
        {
            repaintRequest = true;
        }
        /// <summary>
        /// 如果有请求重绘的话那就重绘
        /// </summary>
        private void RepaintIfRequest()
        {
            //如果有请求重绘的话那就重绘
            if (repaintRequest)
            {
                repaintRequest = false;
                Repaint();
            }
        }
        
        #endregion

        #region 操作相关

        /// <summary>
        /// 处理输入的一些计时器相关的
        /// </summary>
        private void HanldeInputTimer()
        {
            //检测双击
            if (checkDoubleClick)
            {
                if (Time.realtimeSinceStartup - checkDoubleClickTimer >= CheckDoubleClickInterval)
                {
                    checkDoubleClick = false;
                    OnMouseLeftClick();
                }
            }
            
            //定时刷新面板
            if (checkUpdateTimer > 0)
            {
                checkUpdateTimer -= Time.unscaledDeltaTime;
            }
            else
            {
                RepaintWindow();
                checkUpdateTimer = checkUpdateInterval;
            }
        }
        /// <summary>
        /// 鼠标单击左键
        /// </summary>
        private void OnMouseLeftClick()
        {
            //如果现在选择Asset为空，那就没反应
            if (curSelectAsset == null) return;
            
            //单击选择Asset
            Selection.activeObject = curSelectAsset.AssetObj;
            EditorGUIUtility.PingObject(curSelectAsset.AssetObj);
        }
        /// <summary>
        /// 鼠标双击左键
        /// </summary>
        private void OnMouseDoubleClick()
        {
            //如果现在选择Asset为空，那就没反应
            if (curSelectAsset == null) return;

            //双击打开Asset
            curSelectAsset.OpenAsset();
        }
        /// <summary>
        /// 显示右键点击打开的菜单
        /// </summary>
        private void ShowRightClickMenu(StarAssetGUI gui)
        {
            curRightMenuSelectGUI = gui;
            Vector2 windowSize = new Vector2(200f, 20f);
            
            if (gui == null)
            {
                PopupWindow.Show(new Rect(mousePos, windowSize), new StarGUIPopupWindow(clickSpaceOptions,
                    (x) =>
                    {
                        RepaintWindow();
                    }));
            }
            else if (gui is StarTableGUI)
            {
                PopupWindow.Show(new Rect(mousePos, windowSize), new StarGUIPopupWindow(clickTableGUIOptions,
                    (x) =>
                    {
                        switch (x)
                        {
                            case "打开":
                                curRightMenuSelectGUI.OpenAsset();
                                break;
                            case "从桌面上删除":
                                data.DeleteTableGUI(curRightMenuSelectGUI as StarTableGUI);
                                RepaintWindow();
                                break;
                            case "从Unity中删除":

                                if (EditorUtility.DisplayDialog("警告", "这是一项危险操作你确定要删除该文件嘛？", "确定", "取消"))
                                {
                                    data.DeleteTableGUI(curRightMenuSelectGUI as StarTableGUI);
                                    curRightMenuSelectGUI.DeleteAsset();
                                    RepaintWindow();
                                }
                                
                                break;
                        }

                        curRightMenuSelectGUI = null;
                    }));
            }
            else
            {
                PopupWindow.Show(new Rect(mousePos, windowSize), new StarGUIPopupWindow(clickVisitGUIOptions,
                    (x) =>
                    {
                        switch (x)
                        {
                            case "打开":
                                curRightMenuSelectGUI.OpenAsset();
                                break;
                            case "从列表删除":
                                data.DeleteVisitGUI(curRightMenuSelectGUI);
                                RepaintWindow();
                                break;
                            case "从Unity中删除":
                                data.DeleteVisitGUI(curRightMenuSelectGUI);
                                curRightMenuSelectGUI.DeleteAsset();
                                RepaintWindow();
                                break;
                        }
                        
                        curRightMenuSelectGUI = null;
                    }));
            }
        }
        /// <summary>
        /// 处理拖拽
        /// </summary>
        private void HandleDragAndDrop(Event currentEvent)
        {
            //正在拖拽分割线
            if (isDraggingLineRect)
            {
                //更新鼠标显示
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                //更新分割线宽度，然后更新Rect
                latestVisitGUIWidth = prevLineWidth + (mousePos.x - draggingLineOriginX);
                UpdateRect();
                //然后重绘
                RepaintWindow();
                        
                currentEvent.Use();
            }
            //如果在拖拽物品放入桌面
            else if (tableRect.Contains(mousePos))
            {
                //把鼠标的显示改为Copy的样子
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        
                //如果放下了鼠标
                if (currentEvent.type == EventType.DragPerform)
                {
                    //那就完成拖拽
                    DragAndDrop.AcceptDrag();
                        
                    //然后如果拖拽的物品只有一个
                    if (DragAndDrop.objectReferences.Length == 1)
                    {
                        //那就把Asset创建并添加到鼠标选择的位置
                        CreateTableGUI(DragAndDrop.objectReferences[0], GetIconRectIndex(mousePos), false);
                    }
                    else
                    {
                        //记录原先已经有的下标
                        HashSet<int> k = new HashSet<int>();
                        foreach (StarTableGUI gui in data.AssetOnTable)
                        {
                            k.Add(gui.TableIndex);
                        }
                        //遍历选中的物品
                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
                            //创建并按照下标升序逐个添加
                            //从小开始找没有的下标
                            int j = 0;
                            while (k.Contains(j))
                            {
                                j++;
                            }
                            k.Add(j);
                            //然后创建添加
                            CreateTableGUI(draggedObject, j,true);
                            
                        }
                    }
                    
                }
                
                //然后重绘
                RepaintWindow();

                currentEvent.Use();
            }
        }
        /// <summary>
        /// 处理鼠标输入
        /// </summary>
        private void HandleMouseInput()
        {
            Event currentEvent = Event.current;
            
            //如果当前事件为空就返回
            if(currentEvent == null) return;

            //更新鼠标位置
            mousePos = currentEvent.mousePosition;
            
            //如果目前没有选择的Asset
            if (curSelectAsset == null)
            {
                //那就检测当前鼠标位置有没有Asset
                foreach(var node in data.VisitLatestList)
                {
                    if(node.DrawRect.Contains(mousePos))
                    {
                        curSelectAsset = node;
                        curSelectAsset.Selected = true;
                        RepaintWindow();
                        GUI.changed = true;
                        break;
                    }
                }
                //如果还没有那就再遍历检测桌面上的Asset
                if (curSelectAsset == null)
                {
                    foreach(var node in data.AssetOnTable)
                    {
                        if(node.DrawRect.Contains(mousePos))
                        {
                            curSelectAsset = node;
                            curSelectAsset.Selected = true;
                            RepaintWindow();
                            GUI.changed = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                //如果已经有选择Asset了，那就检测鼠标有没有脱离选项
                if(curSelectAsset.DrawRect.Contains(mousePos) == false)
                {
                    curSelectAsset.Selected = false;
                    curSelectAsset = null;
                    RepaintWindow();
                    GUI.changed = true;
                    
                    //取消点击上个Asset的双击检测
                    checkDoubleClick = false;
                }
            }

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (currentEvent.button == 0)
                    {
                        if (curSelectAsset != null)
                        {
                            currentEvent.Use();
                        }
                        //如果点下的位置是分割线位置，那就开始拖拽线条
                        else if (dragLineCheckRect.Contains(mousePos))
                        {
                            isDraggingLineRect = true;
                            prevLineWidth = latestVisitGUIWidth;
                            draggingLineOriginX = currentEvent.mousePosition.x;
                            DragAndDrop.StartDrag("拖拽分割线");
                            currentEvent.Use();
                        }
                    }
                    
                    break;
                case EventType.MouseUp:
                    //点击Asset
                    if (currentEvent.button == 0)
                    {
                        if (curSelectAsset != null)
                        {
                            //判断是双击还是单击
                            if (checkDoubleClick)
                            {
                                //双击，那就调用双击
                                checkDoubleClick = false;
                                OnMouseDoubleClick();
                            }
                            else
                            {
                                //开始计时判断是不是双击
                                checkDoubleClick = true;
                                checkDoubleClickTimer = Time.realtimeSinceStartup;
                            }
                            currentEvent.Use();
                        }
                        
                    }
                    //点击的是右键
                    else if (currentEvent.button == 1)
                    {
                        //如果点在了AssetGUI上面
                        if (curSelectAsset != null)
                        {
                            ShowRightClickMenu(curSelectAsset);
                            currentEvent.Use();
                        }
                        //点在桌面空白处
                        else if (tableRect.Contains(mousePos))
                        {
                            ShowRightClickMenu(null);
                            currentEvent.Use();
                        }
                    }

                    if (isDraggingLineRect)
                    {
                        isDraggingLineRect = false;
                        DragAndDrop.AcceptDrag();
                    }
                    break;
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.MouseDrag:
                    HandleDragAndDrop(currentEvent);
                    break;
            }
            
        }

        #endregion
        

    }
}

