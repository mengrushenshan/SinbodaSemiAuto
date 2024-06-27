using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.Model.DataOperation.SemiAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Business.Samples
{
    public class BoardBusiness : BusinessBase<BoardBusiness>
    {

        //最小样本号
        private const int MIN_BOARDID = 1;
        //最大样本号
        private const int MAX_BOARDID = 99999999;

        /// <summary>
        /// 保存板内信息
        /// </summary>
        /// <param name="boardList"></param>
        /// <returns></returns>
        public bool SaveTemplateNameList(List<Sin_Board> boardList)
        {
            bool result = false;

            if (boardList == null)
            {
                return result;
            }

            Sin_BoardOperation.Instance.Update(boardList);

            return result = true;
        }

        /// <summary>
        /// 创建板内信息
        /// </summary>
        /// <param name="boardList"></param>
        /// <returns></returns>
        public OperationResult CreateTemplateList(List<Sin_Board> boardList)
        {
            OperationResult or = null;

            if (boardList == null)
            {
                return or = new OperationResult() { ResultEnum = OperationResultEnum.FAILED, Message = SystemResources.Instance.GetLanguage(0, "登记列表为空") }; ;
            }

            int sampleNo = SampleBusiness.Instance.GetMaxSampleCode();

            foreach (var board in boardList)
            {
                if (board.TestType == TestType.Sample && board.IsEnable == true)
                {
                    or = SampleBusiness.Instance.CreateSample(sampleNo++, board.Rack, board.Position, "", 1, board.ItemName, board.BoardId);
                }
            }
            Sin_BoardOperation.Instance.Insert(boardList);

            return or;
        }

        /// <summary>
        /// 删除板内信息
        /// </summary>
        /// <param name="boardList"></param>
        /// <returns></returns>
        public bool DeleteTemplateNameList(List<Sin_Board> boardList)
        {
            bool result = false;

            if (boardList == null)
            {
                return result;
            }

            boardList.ForEach(o => Sin_BoardOperation.Instance.Delete(o.Id));

            return result = true;
        }

        /// <summary>
        /// 取出当天登记的最大板号
        /// </summary>
        /// <returns></returns>
        public int GetMaxBoardId()
        {
            List<Sin_Board> boardList = Sin_BoardOperation.Instance.QueryTodayBoardList();
            if (boardList != null)
            {
                int maxCode = boardList.Max(o => o.BoardId);
                if (maxCode < MIN_BOARDID || maxCode > MAX_BOARDID)
                {
                    return MIN_BOARDID;
                }
                return maxCode + 1;
            }

            return MIN_BOARDID;
        }

        public List<Sin_Board> GetBoardListByBoardId(int boardId) 
        {
            List<Sin_Board> boardList = null;

            var tempList = Sin_BoardOperation.Instance.QueryTodayBoardList();
            if (tempList != null && tempList.Count != 0) 
            {
                boardList = tempList.Where(o => o.BoardId == boardId).ToList();
            }

            return boardList;
        }
    }
}
