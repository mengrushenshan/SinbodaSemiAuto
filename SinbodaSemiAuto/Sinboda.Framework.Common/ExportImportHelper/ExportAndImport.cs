
using Microsoft.Office.Interop.Excel;
using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sinboda.Framework.Common.ExportImportHelper
{
    /// <summary>
    /// 
    /// </summary>
    public class ExportAndImport : IExportAndImport
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        private string FileNew = null;
        /// <summary>
        /// 文件目录
        /// </summary>
        private string PathNew = null;
        /// <summary>
        /// 导出文件类型 1-xls 或 xlsx 2-csv 3- txt
        /// </summary>
        private byte FileType = 0;

        /// <summary>
        /// 导入/导出 构造函数
        /// </summary>
        public ExportAndImport()
        {

        }

        #region 数据导入及导出公有方法
        /// <summary>
        /// 文件名合法性检测
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>true-合法 false-非法</returns>
        private bool FileCheck(string filename)
        {
            if ((filename == "") || (filename == null))
            {
                return false;
            }
            //获得文件扩展名
            string temp = System.IO.Path.GetExtension(filename).ToLower();

            if (temp == ".xls" || temp == ".txt" || temp == ".csv" || temp == ".xlsb" || temp == ".xlsx")
            {
                FileNew = System.IO.Path.GetFileName(filename);
                PathNew = System.IO.Path.GetDirectoryName(filename);
                if (temp == ".xls" || temp == ".xlsx")
                    FileType = 1;
                else if (temp == ".csv")
                    FileType = 2;
                else if (temp == ".txt")
                    FileType = 3;
                return true;
            }
            return false;
        }
        #endregion

        #region 数据导出

        /// <summary>
        /// 导出单种数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="exportDatas"></param>
        /// <returns></returns>
        public bool ExportFile<T>(string fileName, ExportData<T> exportDatas) where T : class
        {
            if (exportDatas.Datas != null && exportDatas.PropertiesToColumnHeads != null && exportDatas.PropertiesToColumnHeads.Count != 0)
            {
                DataSet dsExport = new DataSet();
                System.Data.DataTable dtExport = new System.Data.DataTable();

                Type exportType = typeof(T);

                if (!string.IsNullOrEmpty(exportDatas.SheetName))
                {
                    dtExport.TableName = exportDatas.SheetName;
                }

                foreach (string value in exportDatas.PropertiesToColumnHeads.Values)
                {
                    dtExport.Columns.Add(new DataColumn(value));
                }

                foreach (T item in exportDatas.Datas)
                {
                    DataRow dr = dtExport.NewRow();

                    foreach (var keyValue in exportDatas.PropertiesToColumnHeads)
                    {
                        PropertyInfo info = exportType.GetProperty(keyValue.Key);
                        if (info != null)
                        {
                            dr[keyValue.Value] = info.GetValue(item);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    dtExport.Rows.Add(dr);
                }
                dsExport.Tables.Add(dtExport);
                return ExportFile(fileName, dsExport);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 导出两种数据
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="exportDatas1"></param>
        /// <param name="exportDatas2"></param>
        /// <returns></returns>
        public bool ExportFile<T1, T2>(string fileName, ExportData<T1> exportDatas1, ExportData<T2> exportDatas2) where T1 : class
                                                                                       where T2 : class
        {
            if (exportDatas1.Datas != null && exportDatas2.Datas != null &&
                exportDatas1.PropertiesToColumnHeads != null && exportDatas2.PropertiesToColumnHeads != null &&
                exportDatas1.PropertiesToColumnHeads.Count != 0 && exportDatas2.PropertiesToColumnHeads.Count != 0)
            {
                DataSet dsExport = new DataSet();
                System.Data.DataTable dtExport1 = new System.Data.DataTable();
                System.Data.DataTable dtExport2 = new System.Data.DataTable();

                Type exportType1 = typeof(T1);
                Type exportType2 = typeof(T2);

                if (!string.IsNullOrEmpty(exportDatas1.SheetName))
                {
                    dtExport1.TableName = exportDatas1.SheetName;
                }

                if (!string.IsNullOrEmpty(exportDatas2.SheetName))
                {
                    dtExport2.TableName = exportDatas2.SheetName;
                }

                foreach (string value in exportDatas1.PropertiesToColumnHeads.Values)
                {
                    dtExport1.Columns.Add(new DataColumn(value));
                }

                foreach (string value in exportDatas2.PropertiesToColumnHeads.Values)
                {
                    dtExport2.Columns.Add(new DataColumn(value));
                }

                foreach (T1 item in exportDatas1.Datas)
                {
                    DataRow dr = dtExport1.NewRow();

                    foreach (var keyValue in exportDatas1.PropertiesToColumnHeads)
                    {
                        PropertyInfo info = exportType1.GetProperty(keyValue.Key);
                        if (info != null)
                        {
                            dr[keyValue.Value] = info.GetValue(item);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    dtExport1.Rows.Add(dr);
                }

                foreach (T2 item in exportDatas2.Datas)
                {
                    DataRow dr = dtExport2.NewRow();

                    foreach (var keyValue in exportDatas2.PropertiesToColumnHeads)
                    {
                        PropertyInfo info = exportType2.GetProperty(keyValue.Key);
                        if (info != null)
                        {
                            dr[keyValue.Value] = info.GetValue(item);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    dtExport2.Rows.Add(dr);
                }

                dsExport.Tables.Add(dtExport2);
                dsExport.Tables.Add(dtExport1);

                return ExportFile(fileName, dsExport);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 导出三种数据
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="exportDatas1"></param>
        /// <param name="exportDatas2"></param>
        /// <param name="exportDatas3"></param>
        /// <returns></returns>
        public bool ExportFile<T1, T2, T3>(string fileName, ExportData<T1> exportDatas1, ExportData<T2> exportDatas2, ExportData<T3> exportDatas3) where T1 : class
                                                                                        where T2 : class
                                                                                        where T3 : class
        {
            if (exportDatas1.Datas != null && exportDatas2.Datas != null && exportDatas3.Datas != null &&
                exportDatas1.PropertiesToColumnHeads != null && exportDatas2.PropertiesToColumnHeads != null && exportDatas3.PropertiesToColumnHeads != null &&
                exportDatas1.PropertiesToColumnHeads.Count != 0 && exportDatas2.PropertiesToColumnHeads.Count != 0 && exportDatas3.PropertiesToColumnHeads.Count != 0)
            {
                DataSet dsExport = new DataSet();
                System.Data.DataTable dtExport1 = new System.Data.DataTable();
                System.Data.DataTable dtExport2 = new System.Data.DataTable();
                System.Data.DataTable dtExport3 = new System.Data.DataTable();

                Type exportType1 = typeof(T1);
                Type exportType2 = typeof(T2);
                Type exportType3 = typeof(T3);

                if (!string.IsNullOrEmpty(exportDatas1.SheetName))
                {
                    dtExport1.TableName = exportDatas1.SheetName;
                }

                if (!string.IsNullOrEmpty(exportDatas2.SheetName))
                {
                    dtExport2.TableName = exportDatas2.SheetName;
                }

                if (!string.IsNullOrEmpty(exportDatas3.SheetName))
                {
                    dtExport3.TableName = exportDatas3.SheetName;
                }

                foreach (string value in exportDatas1.PropertiesToColumnHeads.Values)
                {
                    dtExport1.Columns.Add(new DataColumn(value));
                }

                foreach (string value in exportDatas2.PropertiesToColumnHeads.Values)
                {
                    dtExport2.Columns.Add(new DataColumn(value));
                }

                foreach (string value in exportDatas3.PropertiesToColumnHeads.Values)
                {
                    dtExport3.Columns.Add(new DataColumn(value));
                }

                foreach (T1 item in exportDatas1.Datas)
                {
                    DataRow dr = dtExport1.NewRow();

                    foreach (var keyValue in exportDatas1.PropertiesToColumnHeads)
                    {
                        PropertyInfo info = exportType1.GetProperty(keyValue.Key);
                        if (info != null)
                        {
                            dr[keyValue.Value] = info.GetValue(item);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    dtExport1.Rows.Add(dr);
                }

                foreach (T2 item in exportDatas2.Datas)
                {
                    DataRow dr = dtExport2.NewRow();

                    foreach (var keyValue in exportDatas2.PropertiesToColumnHeads)
                    {
                        PropertyInfo info = exportType2.GetProperty(keyValue.Key);
                        if (info != null)
                        {
                            dr[keyValue.Value] = info.GetValue(item);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    dtExport2.Rows.Add(dr);
                }

                foreach (T3 item in exportDatas3.Datas)
                {
                    DataRow dr = dtExport3.NewRow();

                    foreach (var keyValue in exportDatas3.PropertiesToColumnHeads)
                    {
                        PropertyInfo info = exportType3.GetProperty(keyValue.Key);
                        if (info != null)
                        {
                            dr[keyValue.Value] = info.GetValue(item);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    dtExport3.Rows.Add(dr);
                }
                dsExport.Tables.Add(dtExport3);
                dsExport.Tables.Add(dtExport2);
                dsExport.Tables.Add(dtExport1);

                return ExportFile(fileName, dsExport);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 导出主从数据
        /// </summary>
        /// <typeparam name="MasterT"></typeparam>
        /// <typeparam name="DetailT"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="exportDatas"></param>
        /// <returns></returns>
        public bool ExportFile<MasterT, DetailT>(string fileName, ExportMasterDetailData<MasterT, DetailT> exportDatas) where MasterT : class where DetailT : class
        {
            if (FileCheck(fileName))
            {
                try
                {
                    if (File.Exists(fileName))  //如果存在同名路径的文件，删除该同名路径的文件
                    {
                        File.Delete(fileName);
                    }
                }
                catch (Exception e)
                {
                    LogHelper.logSoftWare.Error("ExportFile  File is Exists", e);
                    string str = e.Message.ToString();
                    return false;
                }

            }
            else
            {
                return false;
            }

            if (exportDatas.MasterData != null && !string.IsNullOrEmpty(exportDatas.DetailProperty)
            && exportDatas.MasterPropertiesToColumnHeads != null && exportDatas.MasterPropertiesToColumnHeads.Count != 0
            && exportDatas.DetailPropertiesToColumnHeads != null && exportDatas.DetailPropertiesToColumnHeads.Count != 0)
            {
                Type MasterType = typeof(MasterT);
                Type detailType = typeof(DetailT);

                int masterRowNum = exportDatas.MasterData.Count;         //主表的行数
                int masterColumnNum = exportDatas.MasterPropertiesToColumnHeads.Count;   //主表的列数

                int detailColumnNum = exportDatas.DetailPropertiesToColumnHeads.Count;   //子表的列数

                string[] masterColumns = exportDatas.MasterPropertiesToColumnHeads.Values.ToArray();  //主表列头
                string[] masterKeys = exportDatas.MasterPropertiesToColumnHeads.Keys.ToArray();  //主表属性

                string[] detailColumns = exportDatas.DetailPropertiesToColumnHeads.Values.ToArray();  //子表列头
                string[] detailKeys = exportDatas.DetailPropertiesToColumnHeads.Keys.ToArray();     //子表属性

                try
                {

                    PropertyInfo detailPropertyInfo = MasterType.GetProperty(exportDatas.DetailProperty); //子表属性信息                 
                    Type detailPropertyType = detailPropertyInfo.PropertyType; //子表集合类型
                    if (detailPropertyInfo == null)   //主表中不存在传入的子表属性
                    {
                        return false;
                    }

                    //子表属性是否为ICollection类型
                    bool isCollectionType = typeof(ICollection<DetailT>).IsAssignableFrom(detailPropertyType);

                    if (!isCollectionType)
                    {
                        return false;
                    }

                    Application xlApp = new Application();
                    xlApp.DisplayAlerts = false;    //不显示更改提示
                    xlApp.Visible = false;          //不显示界面
                    Workbook xlBook = xlApp.Workbooks.Add(true);    //新增工作簿
                    Sheets xlSheets = xlBook.Sheets;                //获取工作表
                    Worksheet xlSheet = xlSheets.get_Item(1);       //取得sheet1

                    xlSheet = xlSheets[1];

                    if (!string.IsNullOrEmpty(exportDatas.SheetName))
                    {
                        xlSheet.Name = exportDatas.SheetName;
                    }

                    Range rangeField = xlSheet.Range[xlSheet.Cells[1, 1], xlSheet.Cells[1, masterColumnNum]]; //加 主表列头 (导出Excel表的第一行)
                    rangeField.Value2 = masterColumns;
                    rangeField.Interior.ColorIndex = 15;     //15代表灰色  
                    rangeField.Font.Bold = true;
                    rangeField.Font.Size = 9;
                    rangeField.RowHeight = 14.25;
                    rangeField.Borders.LineStyle = 1;
                    rangeField.HorizontalAlignment = 1;

                    Type[] copyToMethodParams = new Type[2];
                    copyToMethodParams[0] = typeof(DetailT[]);
                    copyToMethodParams[1] = typeof(int);

                    MethodInfo countMethod = detailPropertyType.GetMethod("get_Count"); //获取子表元素个数方法
                    MethodInfo copyToMethod = detailPropertyType.GetMethod("CopyTo", copyToMethodParams);   //获取CopyTo方法

                    int previousDetailRows = 0;  //以前所有子表的行数
                    for (int i = 0; i < masterRowNum; i++)
                    {
                        for (int j = 0; j < masterColumnNum; j++)
                        {
                            PropertyInfo info = MasterType.GetProperty(masterKeys[j]);
                            if (info != null)
                            {
                                object obj = info.GetValue(exportDatas.MasterData[i]);
                                xlSheet.Cells[previousDetailRows + (i + 1) * 2 + i * 1, j + 1] = obj == null ? "" : "'" + obj.ToString().Trim();   //在obj.ToString()前加单引号是为了防止自动转化格式 
                                                                                                                                                   //主表头和数据的总数+子表列头总数   
                            }
                            else
                            {
                                return false;
                            }
                        }

                        //加 子表列头
                        rangeField = xlSheet.Range[xlSheet.Cells[previousDetailRows + (i + 1) * 2 + i * 1 + 1, 1], xlSheet.Cells[previousDetailRows + (i + 1) * 2 + i * 1 + 1, detailColumnNum]];
                        rangeField.Value2 = detailColumns;
                        rangeField.Interior.ColorIndex = 15;     //15代表灰色  
                        rangeField.Font.Bold = true;
                        rangeField.Font.Size = 9;
                        rangeField.RowHeight = 14.25;
                        rangeField.Borders.LineStyle = 1;
                        rangeField.HorizontalAlignment = 1;

                        object detailDatas = detailPropertyInfo.GetValue(exportDatas.MasterData[i]); //子表数据
                        if (detailDatas != null)  //子表数据为空
                        {
                            int detailCounts = (int)countMethod.Invoke(detailDatas, null); //子表的行数  
                            DetailT[] detailData = new DetailT[detailCounts];
                            for (int x = 0; x < detailCounts; x++)
                            {
                                copyToMethod.Invoke(detailDatas, new object[] { detailData, 0 });
                                for (int y = 0; y < detailColumnNum; y++)
                                {
                                    PropertyInfo info = detailType.GetProperty(detailKeys[y]);
                                    if (info != null)
                                    {
                                        object obj = info.GetValue(detailData[x]);
                                        xlSheet.Cells[previousDetailRows + (i + 1) * 2 + i * 1 + 1 + x + 1, y + 1] = obj == null ? "" : "'" + obj.ToString().Trim();   //在obj.ToString()前加单引号是为了防止自动转化格式 
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }

                            previousDetailRows += detailCounts;
                        }

                        if (i + 1 != masterRowNum)
                        {
                            //加 主表列头
                            rangeField = xlSheet.Range[xlSheet.Cells[previousDetailRows + (i + 1) * 2 + 1 + i * 1 + 1, 1], xlSheet.Cells[previousDetailRows + (i + 1) * 2 + 1 + i * 1 + 1, masterColumnNum]];
                            rangeField.Value2 = masterColumns;
                            rangeField.Interior.ColorIndex = 15;     //15代表灰色  
                            rangeField.Font.Bold = true;
                            rangeField.Font.Size = 9;
                            rangeField.RowHeight = 14.25;
                            rangeField.Borders.LineStyle = 1;
                            rangeField.HorizontalAlignment = 1;
                        }
                    }

                    xlSheet.Columns.EntireColumn.AutoFit();     //列宽自适应                                    
                    xlApp.DisplayAlerts = false;
                    xlBook.SaveCopyAs(fileName);

                    xlApp.Quit();
                    xlApp = null;
                    System.GC.Collect();

                    return true;
                }
                catch (Exception e)
                {
                    LogHelper.logSoftWare.Error("ExportFileMasterDetail", e);
                    string str = e.Message.ToString();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// DataSet导出到指定文件中
        /// </summary>
        /// <param name="filename">预导出文件名（含路径）</param>
        /// <param name="srcData">数据源</param>
        /// <returns>false-失败 ture-成功</returns>
        public bool ExportFile(string filename, System.Data.DataSet srcData)
        {
            if (srcData == null)
            {
                return false;
            }
            try
            {
                if (FileCheck(filename))
                {
                    if (File.Exists(filename))  //如果存在同名路径的文件，删除该同名路径的文件
                    {
                        File.Delete(filename);
                    }
                    using (srcData)
                    {
                        if (srcData == null)
                        {
                            return false;
                        }
                        if (FileType == 1)
                        {
                            ExportToExcel(filename, srcData); //参数顺序：数据源、生成的文件名、生成的表名（复制数据库中的表名）
                        }
                        else if (FileType == 2)
                        {
                            ExportToCsvAndTxt(FileType, filename, srcData);
                        }
                        else if (FileType == 3)
                        {
                            ExportToCsvAndTxt(FileType, filename, srcData);
                        }
                        srcData.Dispose();
                    }
                }
                else
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("ExportFile", ex);
                string s = ex.Message.ToString();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将表中数据导入到指定Excel文件中，使用方法前，请先判断DataSet是否为null
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="ds">数据表</param>
        private void ExportToExcel(string filename, System.Data.DataSet ds)
        {
            try
            {

                //int PageMax2003 = 65535;    //Excel2003版本设置单页输出最大为50000行

                Application xlApp = new Application();
                xlApp.DisplayAlerts = false;    //不显示更改提示
                xlApp.Visible = false;          //不显示界面
                Workbook xlBook = xlApp.Workbooks.Add(true);    //新增工作簿
                Sheets xlSheets = xlBook.Sheets;                //获取工作表
                Worksheet xlSheet = xlSheets.get_Item(1);       //取得sheet1

                #region Excel2003导出
                //if (System.IO.Path.GetExtension(filename) == ".xls" && tableRowNum > PageMax2003)
                //{
                //    int sheetCount = (int)(tableRowNum / PageMax2003);  //导出数据生成的表单数 
                //    if (sheetCount * PageMax2003 < tableRowNum)         //当总行数不被pageRows整除时，经过四舍五入可能页数不准  
                //    {
                //        sheetCount = sheetCount + 1;
                //    }

                //    for (int sc = 1; sc <= sheetCount; sc++)
                //    {
                //        if (sc > 1)
                //        {
                //            object missing = System.Reflection.Missing.Value;
                //            xlSheet = xlSheets.Add(missing, missing, missing, missing); //添加一个sheet页
                //        }
                //        else
                //        {
                //            xlSheet = xlSheets[sc]; //取得sheet1
                //        }
                //        string[,] datas = new string[tableRowNum + 1, tableColumnNum];
                //        for (int i = 0; i < tableColumnNum; i++)    //写入字段
                //        {
                //            datas[0, i] = srcData.Columns[i].Caption;   //表头信息
                //        }

                //        //表头区域，稍后设置
                //        Range rangeField = xlSheet.Range[xlSheet.Cells[1, 1], xlSheet.Cells[1, tableColumnNum]];

                //        int init = int.Parse(((sc - 1) * PageMax2003).ToString());    //当前sheet页起始数据位置
                //        int r = 0;
                //        int index = 0;
                //        int result;     //当前sheet页终止位置

                //        if (sc * PageMax2003 >= tableRowNum)    //最后一页
                //        {
                //            result = (int)tableRowNum;
                //        }
                //        else
                //        {
                //            result = int.Parse((sc * PageMax2003).ToString());    //非最后一页
                //        }

                //        for (r = init; r < result; r++)
                //        {
                //            index = index + 1;
                //            for (int i = 0; i < tableColumnNum; i++)
                //            {
                //                object obj = srcData.Rows[r][srcData.Columns[i].ToString()];
                //                datas[index, i] = obj == null ? "" : "'" + obj.ToString().Trim();   //在obj.ToString()前加单引号是为了防止自动转化格式  
                //            }
                //        }
                //        Range rangeData = xlSheet.Range[xlSheet.Cells[1, 1], xlSheet.Cells[index + 1, tableColumnNum]];
                //        rangeData.Value2 = datas;
                //        xlSheet.Columns.EntireColumn.AutoFit();     //列宽自适应

                //        //表头区域设置
                //        rangeField.Interior.ColorIndex = 15;     //15代表灰色  
                //        rangeField.Font.Bold = true;
                //        rangeField.Font.Size = 9;
                //        rangeField.RowHeight = 14.25;
                //        rangeField.Borders.LineStyle = 1;
                //        rangeField.HorizontalAlignment = 1;

                //        xlApp.DisplayAlerts = false;
                //        xlBook.SaveAs(filename,Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8,
                //        System.Type.Missing,System.Type.Missing, System.Type.Missing, System.Type.Missing,
                //        Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, System.Type.Missing,
                //        System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing);


                //    }
                //}
                #endregion

                #region Excel2003以上版本导出
                //else
                //{
                for (int tableNum = 0; tableNum < ds.Tables.Count; tableNum++)
                {
                    if (tableNum > 0)
                    {
                        object missing = System.Reflection.Missing.Value;
                        xlSheet = xlSheets.Add(missing, missing, missing, missing); //添加一个sheet页
                    }
                    else
                    {
                        xlSheet = xlSheets[1]; //取得sheet1
                    }
                    System.Data.DataTable srcData = ds.Tables[tableNum];
                    int tableRowNum = srcData.Rows.Count;         //数据表的行数
                    int tableColumnNum = srcData.Columns.Count;   //数据表的列数
                    string[,] datas = new string[tableRowNum + 1, tableColumnNum];
                    for (int i = 0; i < tableColumnNum; i++)    //写入字段
                    {
                        datas[0, i] = srcData.Columns[i].Caption;   //表头信息
                    }

                    //表头区域，稍后设置
                    Range rangeField = xlSheet.Range[xlSheet.Cells[1, 1], xlSheet.Cells[1, tableColumnNum]];

                    int r = 0;
                    for (r = 0; r < tableRowNum; r++)
                    {
                        for (int i = 0; i < tableColumnNum; i++)
                        {
                            object obj = srcData.Rows[r][srcData.Columns[i].ToString()];
                            datas[r + 1, i] = obj == null ? "" : "'" + obj.ToString().Trim();   //在obj.ToString()前加单引号是为了防止自动转化格式  
                        }
                    }

                    Range rangeData = xlSheet.Range[xlSheet.Cells[1, 1], xlSheet.Cells[tableRowNum + 1, tableColumnNum]];
                    rangeData.Value2 = datas;
                    xlSheet.Columns.EntireColumn.AutoFit();     //列宽自适应

                    //表头区域设置
                    rangeField.Interior.ColorIndex = 15;     //15代表灰色  
                    rangeField.Font.Bold = true;
                    rangeField.Font.Size = 9;
                    rangeField.RowHeight = 14.25;
                    rangeField.Borders.LineStyle = 1;
                    rangeField.HorizontalAlignment = 1;

                    xlSheet.Name = ds.Tables[tableNum].TableName;
                    xlApp.DisplayAlerts = false;
                    xlBook.SaveCopyAs(filename);
                }
                //}
                #endregion

                xlApp.Quit();
                xlApp = null;
                System.GC.Collect();
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("ExportExcel", ex);
                string s = ex.Message.ToString();
                //System.GC.Collect();
            }
        }

        /// <summary>
        /// 将表中数据导入到指定Csv文件中，使用方法前，请先判断DataSet是否为null
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="ds">数据表</param>
        private void ExportToCsvAndTxt(byte filetype, string filename, System.Data.DataSet ds)
        {
            try
            {
                StreamWriter CsvWrite;
                using (CsvWrite = new StreamWriter(filename, false, Encoding.Unicode))
                {
                    for(int tableNum = 0; tableNum < ds.Tables.Count; tableNum++ )
                    {
                        System.Data.DataTable srcData = ds.Tables[tableNum];
                        int columns = srcData.Columns.Count;

                        foreach (DataColumn col in srcData.Columns)
                        {
                            CsvWrite.Write(col.ColumnName);
                            CsvWrite.Write('\t');
                        }
                        CsvWrite.WriteLine();

                        foreach (DataRow row in srcData.Rows)
                        {
                            for (int c = 0; c < columns; c++)
                            {
                                if (filetype == 2)
                                {
                                    CsvWrite.Write(row[c].ToString().Replace('+', '﹢'));
                                }
                                else if (filetype == 3)
                                {
                                    CsvWrite.Write(row[c].ToString());
                                }
                                CsvWrite.Write('\t');
                            }
                            CsvWrite.WriteLine();
                        }

                        // 添加此内容，目的让表与表之间隔开五行
                        for(int i = 0; i < 5; i++)
                        {
                            CsvWrite.WriteLine();
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("ExportCSVTXT", ex);
                string s = ex.Message.ToString();
            }

        }

        #endregion

        #region 数据导入
        /// <summary>
        /// 数据导入功能
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>DataSet结果不为空-成功，DataSet结果为null-失败</returns>
        public System.Data.DataSet ImportFromFile(string filename)
        {
            //System.Data.DataTable resultDataTable = new System.Data.DataTable();
            //resultDataTable = null;
            if (filename == null || filename.Trim() == "")
            {
                return null;
            }
            System.Data.DataSet dsResult = new System.Data.DataSet();
            try
            {
                if (FileCheck(filename))
                {
                    if (!File.Exists(filename))  //如果路径下不存在文件，返回false值
                    {
                        return null;
                    }
                    if (FileType == 1)
                    {
                        dsResult = ImportFromExcel(filename);
                        //ExportToExcel(filename); //参数顺序：数据源、生成的文件名、生成的表名（复制数据库中的表名）
                    }
                    else if (FileType == 2)
                    {
                        dsResult = ImportFromCsvAndTxt(filename);
                    }
                }
                else
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("ImportFile", ex);
                string s = ex.Message.ToString();
                return null;
            }
            return dsResult;
        }

        /// <summary>
        /// Excel文件导入
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>返回DataSet结果</returns>
        private System.Data.DataSet ImportFromExcel(string filename)
        {
            DataSet dsResult = new DataSet();
            string connectionString = string.Empty;
            //System.Data.DataTable resultDataTable = new System.Data.DataTable();
            if (System.IO.Path.GetExtension(filename) == ".xls")    //Excel2003
            {
                connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filename + ";" + "Extended Properties=Excel 8.0 ;";
                OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();
                System.Data.DataTable dtTemp = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                int SheetCount = dtTemp.Rows.Count;
                for (int i = 0; i < SheetCount; i++)
                {
                    string strSheetName = dtTemp.Rows[i][2].ToString().Trim();//获取工作薄的name
                    OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + strSheetName + "]", conn); //根据工作薄的名字查找对应工作薄的数据
                    oada.Fill(dsResult.Tables.Add(strSheetName));
                    dsResult.Tables[i].TableName = strSheetName.Replace("$", "");

                }
                conn.Close();
            }
            //Excel2007/Excel2010
            else
            {
                connectionString = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;HDR=YES;IMEX=1""", filename);
                OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();
                System.Data.DataTable dtTemp = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                int SheetCount = dtTemp.Rows.Count;
                for (int i = 0; i < SheetCount; i++)
                {
                    string strSheetName = dtTemp.Rows[i][2].ToString().Trim();//获取工作薄的name
                    OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + strSheetName + "]", conn); //根据工作薄的名字查找对应工作薄的数据
                    oada.Fill(dsResult.Tables.Add());
                    dsResult.Tables[i].TableName = strSheetName.Replace("$", "");

                }
                conn.Close();
            }
            return dsResult;
        }

        /// <summary>
        /// Csv文件导入
        /// </summary>
        /// <param name="filename">文件路径</param>
        private System.Data.DataSet ImportFromCsvAndTxt(string filename)
        {
            System.Data.DataSet dsResult = new System.Data.DataSet();
            System.Data.DataTable resultDataTable = new System.Data.DataTable();
            FileStream fs = new FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中各字段内容
            string[] arrayLine;
            //标示列数
            int columnCount = 0;

            //读取第一行数据获取列名
            strLine = sr.ReadLine();
            if (strLine == null)
            {
                return null;
            }
            arrayLine = strLine.Split('\t');
            columnCount = arrayLine.Length;
            //创建列
            for (int i = 0; i < columnCount; i++)
            {
                DataColumn dc = new DataColumn(arrayLine[i]);
                resultDataTable.Columns.Add(dc);
            }

            //读取数据内容
            while ((strLine = sr.ReadLine()) != null)
            {
                arrayLine = strLine.Split('\t');
                DataRow dr = resultDataTable.NewRow();
                for (int i = 0; i < columnCount; i++)
                {
                    dr[i] = arrayLine[i];
                }
                resultDataTable.Rows.Add(dr);
            }
            sr.Close();
            fs.Close();
            //return resultDataTable;
            dsResult.Tables.Add(resultDataTable);
            return dsResult;
        }

        #endregion

    }
}
