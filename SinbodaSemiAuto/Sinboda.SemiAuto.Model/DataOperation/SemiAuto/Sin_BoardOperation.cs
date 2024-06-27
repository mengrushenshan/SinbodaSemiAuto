using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DataOperation.SemiAuto
{
    public class Sin_BoardOperation : EFDataOperationBase<Sin_BoardOperation, Sin_Board, Sin_DbContext>
    {
        /// <summary>
        /// 获取当天可用样本列表
        /// </summary>
        /// <returns></returns>
        public List<Sin_Board> QueryTodayBoardList()
        {
            DateTime dateToday = DateTime.Now.Date;
            DateTime dateTomorrow = DateTime.Now.AddDays(1).Date;

            List<Sin_Board> boardList = Sin_BoardOperation.Instance.Query(o => o.RegistDate >= dateToday && o.RegistDate < dateTomorrow);
            if (boardList.Count != 0)
            {
                return boardList;
            }
            return null;
        }
    }
}
