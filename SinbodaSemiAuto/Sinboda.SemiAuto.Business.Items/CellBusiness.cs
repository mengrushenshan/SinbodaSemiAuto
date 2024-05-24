using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.Model.DataOperation.SemiAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Business.Items
{
    public class CellBusiness : BusinessBase<CellBusiness>
    {
        private Dictionary<string, Sin_Cell> CellDicS = new Dictionary<string, Sin_Cell>();

        /// <summary>
        /// 计算孔位板坐标
        /// </summary>
        /// <param name="CellOrigin">原点孔位</param>
        /// <param name="CellRow">同行孔位</param>
        /// <param name="CellCol">同列孔位</param>
        /// <returns></returns>
        public void Calc(Sin_Cell CellOrigin, Sin_Cell CellRow, Sin_Cell CellCol)
        {
            if (CellOrigin == null ||
                CellRow == null ||
                CellCol == null)
                return;

            //清空数据
            CellDicS.Clear();

            //x偏移
            int diffX1 = Math.Max(CellCol.X - CellOrigin.X, CellRow.X - CellOrigin.X);
            int diffX2 = Math.Min(CellCol.X - CellOrigin.X, CellRow.X - CellOrigin.X);

            //y偏移
            int diffY1 = Math.Max(CellCol.Y - CellOrigin.Y, CellRow.Y - CellOrigin.Y);
            int diffY2 = Math.Min(CellCol.Y - CellOrigin.Y, CellRow.Y - CellOrigin.Y);

            //z偏移
            int diffZ1 = CellCol.Z - CellOrigin.Z;
            int diffZ2 = CellRow.Z - CellOrigin.Z;

            //通过偏移计算孔板坐标
            for (int row = 0; row < GlobalData.PlateRow; row++)
            {
                for (int col = 0; col < GlobalData.PlateCol; col++)
                {
                    Sin_Cell cell = new Sin_Cell();
                    string strPos = (row + 1) + "-" + (col + 1);
                    cell.X = CellOrigin.X + diffX1 * col / (GlobalData.PlateCol - 1) + diffX2 * row / (GlobalData.PlateRow - 1);
                    cell.Y = CellOrigin.Y + diffY1 * row / (GlobalData.PlateRow - 1) + diffY2 * col / (GlobalData.PlateCol - 1);
                    cell.Z = CellOrigin.Z + diffZ1 * row / (GlobalData.PlateRow - 1) + diffZ2 * col / (GlobalData.PlateCol - 1);
                    CellDicS[strPos] = cell;
                }
            }
        }

        /// <summary>
        /// 获取孔位
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Sin_Cell GetCell(string index)
        {
            if (CellDicS.Count <= 0)
            {
                return null;
            }

            if (!CellDicS.ContainsKey(index))
            {
                return null;
            }

            Sin_Cell cell = CellDicS[index];
            return cell;
        }

        public bool SaveCell(Sin_Cell sinCell)
        {
            bool result = false;

            if (sinCell == null)
            {
                return result;
            }

            try
            {
                if (Sin_Cell_DataOperation.Instance.Find(sinCell.Id) != null)
                {
                    Sin_Cell_DataOperation.Instance.Update(sinCell);
                }
                else
                {
                    Sin_Cell_DataOperation.Instance.Insert(sinCell);
                }

                return result = true;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("SaveCell Error:" + e.Message);
                return result;
            }
        }

        public List<Sin_Cell> GetCellList() 
        {
            List<Sin_Cell> result = new List<Sin_Cell>();
            result = Sin_Cell_DataOperation.Instance.Query(o => true);
            return result;
        }

        public Sin_Cell GetCellByIndex(int index)
        {
            var cellList = Sin_Cell_DataOperation.Instance.Query(o => o.Index == index);
            if (cellList.Count > 0)
            {
                return cellList.FirstOrDefault();
            }
            else
            { 
                return null; 
            }
        }

        public bool SetCellDic()
        {
            bool result = false;
            var cellList = GetCellList();

            if (cellList.Count != 3)
            {
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "计算点位不全"));
                return result;
            }
            var cellOrigin = cellList.Find(o => o.Index == 1);
            var cellRow = cellList.Find(o => o.Index == 2);
            var cellCol = cellList.Find(o => o.Index == 3);

            Calc(cellOrigin, cellRow, cellCol);
            return result = true;
        }
    }
}
