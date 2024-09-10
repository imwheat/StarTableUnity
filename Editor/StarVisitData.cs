//****************** 代码文件申明 ************************
//* 文件：StarVisitData                                       
//* 作者：wheat
//* 创建时间：2024/08/26 14:39:20 星期一
//* 描述：访问打开某个Asset的记录
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
namespace KFrame.StarTable
{
    [System.Serializable]
    public class StarVisitData
    {
        /// <summary>
        /// Asset文件的名称
        /// </summary>
        public string Name;
        /// <summary>
        /// Asset文件的GUID
        /// </summary>
        public string GUID;

        /// <summary>
        /// 访问时间
        /// </summary>
        public string VisitTime;

        public StarVisitData()
        {
            
        }

        public StarVisitData(string guid, DateTime time, string name)
        {
            GUID = guid;
            VisitTime = time.ToString();
            Name = name;
        }
    }
}

