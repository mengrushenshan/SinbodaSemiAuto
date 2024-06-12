using Sinboda.SemiAuto.View.Samples.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sinboda.SemiAuto.View.Samples.ViewModel
{
    public class SampleRegisterBoardViewModel
    {
        private UIElementCollection children1;
        private UIElementCollection children2;
        private UIElementCollection children3;

        public SampleRegisterBoardViewModel(UIElementCollection children1, UIElementCollection children2, UIElementCollection children3) 
        {
            this.children1 = children1;
            this.children2 = children2;
            this.children3 = children3;

            ShowTextTpye();
        }

        private void ShowTextTpye()
        {
            //清除列表
            children1.Clear();
            children2.Clear();
            children3.Clear();
            
            for (int i = 1; i <= 3; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    if (i == 1)
                    {
                        children1.Add(new SpecimensManageItemControl($"{i}-{j}"));
                    }
                    else if (i == 2)
                    {
                        children2.Add(new SpecimensManageItemControl($"{i}-{j}"));
                    }
                    else if (i == 3)
                    {
                        children3.Add(new SpecimensManageItemControl($"{i}-{j}"));
                    }
                }
            }
        }
    }
}
