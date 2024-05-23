using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.Model.DataOperation.SemiAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Business.Items
{
    public class ItemBusiness : BusinessBase<ItemBusiness>
    {
        public List<Sin_Item> GetAllItems()
        {
            List<Sin_Item> sinItems = Sin_Item_DataOperation.Instance.Query(o => true);
            if (sinItems.Count != 0)
            {
                return sinItems.OrderBy(p => p.Test_order).ToList();
            }

            return null;
        }

        public List<string> GetItemNames()
        {
            List<Sin_Item> itemList = GetAllItems();
            if (itemList != null)
            {
                return itemList.Select(o => o.ItemName).ToList();
            }
            return null;
        }
    }
}
