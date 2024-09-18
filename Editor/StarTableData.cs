//****************** 代码文件申明 ************************
//* 文件：StarTableData                                       
//* 作者：wheat
//* 创建时间：2024/08/26 14:29:49 星期一
//* 描述：用于保存用户的操作数据
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace KFrame.StarTable
{
    [System.Serializable]
    public class StarTableData
    {
        #region 参数设定

        /// <summary>
        /// 保存Key
        /// </summary>
        public const string SaveKey = "StarTableUserData";
        
        //默认分数
        public const int DefaultScore = 100;
        
        //分数的幂数
        public const float ScorePow = 0.5f;
        
        //最长有效时间，超过这个时间的访问数据被删除
        public const int DDL = 10;

        #endregion

        #region 最近访问相关

        /// <summary>
        /// 最近常访问的Asset列表
        /// </summary>
        [NonSerialized] 
        public List<StarAssetGUI> VisitLatestList;
        /// <summary>
        /// 访问的Asset的字典
        /// </summary>
        private Dictionary<string, StarAssetGUI> visitOrderDic;
        /// <summary>
        /// 访问数据
        /// </summary>
        [SerializeField]
        private List<StarVisitData> visitDatas;

        #endregion

        #region 桌面GUI相关
        
        /// <summary>
        /// 最近访问列表宽度
        /// </summary>
        public float LatestVisitGUIWidth = -1f;
        /// <summary>
        /// 图标放大比例
        /// </summary>
        public float IconSizeRatio = 1.0f;
        /// <summary>
        /// 桌面上的Asset
        /// </summary>
        public List<StarTableGUI> AssetOnTable;
        /// <summary>
        /// 桌面上的编辑器的存储数据
        /// </summary>
        public List<StarEditorWindowData> EditorOnTableData;
        /// <summary>
        /// 桌面上的编辑器的存储数据
        /// </summary>
        [NonSerialized]
        public Dictionary<string, StarEditorWindowData> EditorDataDic;
        /// <summary>
        /// 记录桌面上已经有的id
        /// </summary>
        [NonSerialized]
        public HashSet<int> TableIndexes = new HashSet<int>();

        #endregion

        #region 初始化相关

        /// <summary>
        /// 初始化最近访问列表
        /// </summary>
        protected void InitLatestVisitList()
        {
            //获取现在的时间
            DateTime curTime = DateTime.Now;
            //用于记录初始化的时候丢失的Obejct
            HashSet<string> missingObj = new HashSet<string>();

            //逐个遍历访问记录
            for (int i = 0; i < visitDatas.Count; i++)
            {
                StarVisitData visitData = visitDatas[i];
                if (visitData == null || !DateTime.TryParse(visitData.VisitTime, out DateTime visitTime)
                    || missingObj.Contains(visitData.GUID))
                {
                    visitDatas.RemoveAt(i);
                    i--;
                    continue;
                }
                
                //获取访问日期的相差日期
                int subDay = curTime.Subtract(visitTime).Days;
                int score = DefaultScore;
                
                //如果相差日期为负数，说明手动改了日期，直接记录分数为0
                if (subDay < 0)
                {
                    score = 0;
                }
                //没超过过期日期，正常计算分数
                else if (subDay <= DDL)
                {
                    score = Mathf.FloorToInt(score * Mathf.Pow(ScorePow, subDay));
                }
                //记录过期了那就移除
                else
                {
                    visitDatas.RemoveAt(i);
                    i--;
                    continue;
                }
                
                //如果访问数据里面还没有，那就创建
                if (!visitOrderDic.TryGetValue(visitData.GUID, out StarAssetGUI orderData))
                {
                    //先获取这个Asset，如果没有的话，那就说明Asset丢失了，从访问数据里面移除
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(visitData.GUID));
                    if (obj == null)
                    {
                        missingObj.Add(visitData.GUID);
                        visitDatas.RemoveAt(i);
                        i--;
                        continue;
                    }
                    
                    orderData = new StarAssetGUI(visitData.GUID, obj.name, obj);
                    visitOrderDic[visitData.GUID] = orderData;
                }

                //更新优先度
                orderData.Order += score;
            }

            //新建最常访问的数据列表
            VisitLatestList = new List<StarAssetGUI>();
            
            //遍历每一个访问优先度数据
            foreach (StarAssetGUI orderData in visitOrderDic.Values)
            {
                AddVisitDataByOrder(orderData);
            }
        }
        /// <summary>
        /// 初始化桌面
        /// </summary>
        protected void InitTable()
        {
            if (AssetOnTable == null)
            {
                AssetOnTable = new List<StarTableGUI>();
            }

            if (EditorOnTableData == null)
            {
                EditorOnTableData = new List<StarEditorWindowData>();
            }
            
            //遍历加载每个Asset
            for (int i = AssetOnTable.Count -1; i >= 0; i--)
            {
                //移除里面的Editor(会在下一步完成初始化)
                if (AssetOnTable[i].IsEditor)
                {
                    AssetOnTable.RemoveAt(i);
                }
                else
                {
                    StarTableGUI tableGUI = AssetOnTable[i];
                    //获取Object和对应的图标
                    tableGUI.AssetObj =
                        AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(tableGUI.GUID));
                    tableGUI.Icon = AssetPreview.GetMiniThumbnail(tableGUI.AssetObj);
                    TableIndexes.Add(tableGUI.TableIndex);
                }
            }
            
            //初始化编辑器数据
            //遍历每一个data然后存入字典
            EditorDataDic = new Dictionary<string, StarEditorWindowData>();
            foreach (StarEditorWindowData data in EditorOnTableData)
            {
                EditorDataDic[data.EditorKey] = data;
            }
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            //初始化防空
            if (visitDatas == null)
            {
                visitDatas = new List<StarVisitData>();
            }
            if (visitOrderDic == null)
            {
                visitOrderDic = new Dictionary<string, StarAssetGUI>();
            }
            
            //逐个初始化
            InitLatestVisitList();
            InitTable();
        }


        #endregion

        #region 最近访问相关

        /// <summary>
        /// 当打开Asset的时候调用
        /// </summary>
        /// <param name="obj">访问的Object</param>
        public void OnOpenAsset(Object obj)
        {
            //获取路径和GUID
            string path = AssetDatabase.GetAssetPath(obj);
            string guid = AssetDatabase.AssetPathToGUID(path);

            //先更新访问记录
            visitDatas.Add(new StarVisitData(guid, DateTime.Now, obj.name));
            
            //如果已经访问过相同的物品了，那就更新
            if (visitOrderDic.ContainsKey(guid))
            {
                //获取data，更新优先度
                StarAssetGUI assetGUI = visitOrderDic[guid];
                assetGUI.Order += DefaultScore;
                
                //先找到当前的位置
                int k = 0;
                for (int i = 0; i < VisitLatestList.Count; i++)
                {
                    if (VisitLatestList[i].GUID == guid)
                    {
                        k = i;
                        break;
                    }
                }

                //如果不是第一，那就可能需要重新排序
                if (k > 0)
                {
                    //如果超过了上一个优先度，那就更新一下优先度
                    if (VisitLatestList[k - 1].Order < assetGUI.Order)
                    {
                        //先移除，然后重新找一下存入位置
                        VisitLatestList.RemoveAt(k);
                        VisitLatestList.Insert(FindVisitDataOrderIndex(assetGUI.Order, k -1), assetGUI);
                    }
                }
                
            }
            //没有的话那就新添加
            else
            {
                StarAssetGUI assetGUI = new StarAssetGUI(guid, obj.name, obj);
                assetGUI.Order = DefaultScore;
                visitOrderDic[guid] = assetGUI;
                //然后添加到最近访问列表里面
                AddVisitDataByOrder(assetGUI);
            }
            
        }
        /// <summary>
        /// 按照优先度降序排序添加访问数据
        /// </summary>
        /// <param name="gui">要添加的数据</param>
        private void AddVisitDataByOrder(StarAssetGUI gui)
        {
            //如果为空，那就返回
            if(gui == null) return;
            
            //如果列表为空的话，那就直接添加
            if (VisitLatestList.Count == 0)
            {
                VisitLatestList.Add(gui);
            }
            //否则就使用二分查找合适的位置然后插入
            else
            {
                VisitLatestList.Insert(FindVisitDataOrderIndex(gui.Order), gui);
            }
        }
        /// <summary>
        /// 找到放入列表的位置按优先度降序排序
        /// </summary>
        /// <param name="order">优先度</param>
        /// <param name="r">右边界</param>
        /// <returns></returns>
        private int FindVisitDataOrderIndex(int order, int r = -1)
        {
            if (VisitLatestList.Count == 0)
            {
                return 0;
            }
            else
            {
                int l = 0;
                if (r == -1)
                {
                    r = VisitLatestList.Count - 1;
                }

                while (l <= r)
                {
                    int mid = (l + r) / 2;
                    
                    if (order > VisitLatestList[mid].Order)
                    {
                        r = mid - 1;
                    }
                    else
                    {
                        l = mid + 1;
                    }
                }

                return l;
            }
        }

        #endregion

        #region 数据管理

        /// <summary>
        /// 删除桌面上的GUI
        /// </summary>
        /// <param name="tableGUI">要删除的GUI</param>
        public void DeleteTableGUI(StarTableGUI tableGUI)
        {
            //遍历查找然后删除
            for (int i = 0; i < AssetOnTable.Count; i++)
            {
                if (tableGUI == AssetOnTable[i])
                {
                    AssetOnTable.RemoveAt(i);
                    break;
                }
            }
            //移除桌面占用id
            TableIndexes.Remove(tableGUI.TableIndex);
            
            //保存删除后的数据
            StarTableSystem.SaveData();
        }
        /// <summary>
        /// 从最近访问列表上删除GUI和其访问记录
        /// </summary>要删除的GUI
        /// <param name="visitGUI"></param>
        public void DeleteVisitGUI(StarAssetGUI visitGUI)
        {
            //先从最近访问列表上删除
            for (int i = 0; i < VisitLatestList.Count; i++)
            {
                if (visitGUI == VisitLatestList[i])
                {
                    VisitLatestList.RemoveAt(i);
                    break;
                }
            }
            
            //再删除数据
            for (int i = visitDatas.Count - 1; i >= 0 ; i--)
            {
                if (visitDatas[i].GUID == visitGUI.GUID)
                {
                    visitDatas.RemoveAt(i);
                }
            }
            visitOrderDic.Remove(visitGUI.GUID);

            //保存删除后的数据
            StarTableSystem.SaveData();
        }

        #endregion
        
    }
}

