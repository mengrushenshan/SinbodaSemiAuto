using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.View.MachineryDebug
{
    public class InitializeModule : IModule
    {
        public void FinalizeResource()
        {

        }

        public List<ModuleMenuItem> GetMenus()
        {
            throw new NotImplementedException();
        }

        public InitTaskResult InitializeResource()
        {
            return new InitTaskResult();
        }


    }
}
