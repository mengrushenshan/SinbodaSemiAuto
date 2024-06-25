using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.Model.DataOperation.SemiAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Business.Samples
{
    public class BoardTemplateBusiness : BusinessBase<BoardTemplateBusiness>
    {
        public List<Sin_BoardTemplate> GetBoardList(string templateName) 
        {
            List<Sin_BoardTemplate> boardList = null;

            boardList = Sin_BoardTemplateOperation.Instance.Query(o => o.TemplateName == templateName);

            return boardList;
        }

        public List<string> GetTemplateNameList()
        {
            List<string> tempLateNameList = null;

            var tempList = Sin_BoardTemplateOperation.Instance.Query(o => o.TemplateName != "Default");
            if (tempList != null) 
            {
                tempLateNameList = tempList.Select(o => o.TemplateName).Distinct().ToList();
            }
            
            return tempLateNameList;
        }

        public bool SaveTemplateNameList(List<Sin_BoardTemplate> boardTemplateList)
        {
            bool result = false;

            if (boardTemplateList == null)
            {
                return result;
            }

            Sin_BoardTemplateOperation.Instance.Update(boardTemplateList);

            return result = true;
        }

        public bool CreateTemplateList(List<Sin_BoardTemplate> boardTemplateList)
        {
            bool result = false;

            if (boardTemplateList == null)
            {
                return result;
            }

            Sin_BoardTemplateOperation.Instance.Insert(boardTemplateList);

            return result = true;
        }

        public bool DeleteTemplateNameList(List<Sin_BoardTemplate> boardTemplateList)
        {
            bool result = false;

            if (boardTemplateList == null)
            {
                return result;
            }

            boardTemplateList.ForEach(o => Sin_BoardTemplateOperation.Instance.Delete(o.Id));

            return result = true;
        }
    }
}
