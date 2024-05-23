using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels.Mapping
{
    /// <summary>
    ///系统注册信息实体部署关系 
    /// </summary>
    public class SysRegisterModelMap : EntityTypeConfiguration<SysRegisterModel>
    {
        public SysRegisterModelMap()
        {
            Property(t => t.AgentName).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.HospitalAddr).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.HospitalName).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.HospitalPhone).IsRequired().HasColumnType("varchar").HasMaxLength(20);
            //Property(t => t.RegisterCode).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.SaleName).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.SalePhone).IsRequired().HasColumnType("varchar").HasMaxLength(20);
            //Property(t => t.Create_user).IsRequired().HasColumnType("varchar").HasMaxLength(200);
            //Property(t => t.Create_time).IsRequired();
            //
            ToTable("SYS_REGISTER");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.AgentName).HasColumnName("AGENTNAME");
            Property(t => t.HospitalAddr).HasColumnName("HOSPITALADDR");
            Property(t => t.HospitalName).HasColumnName("HOSPITALNAME");
            Property(t => t.HospitalPhone).HasColumnName("HOSPITALPHONE");
            Property(t => t.RegisterCode).HasColumnName("REGISTERCODE");
            Property(t => t.SaleName).HasColumnName("SALENAME");
            Property(t => t.SalePhone).HasColumnName("SALEPHONE");
            Property(t => t.Memo).HasColumnName("MEMO");
            Property(t => t.Create_user).HasColumnName("CREATE_USER");
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");
        }
    }
}
