using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels.Mapping
{
    /// <summary>
    /// ◆单元名称：系统日志实体部署关系
    /// ◆功能描述：
    /// ◆创建人  ： sunch
    /// ◆创建日期： 2019/3/13 14:00
    /// ◆修改人  ：
    /// ◆修改日期：
    /// ◆修改原因：
    /// </summary>
    public class SysLogModelMap : EntityTypeConfiguration<SysLogModel>
    {
        public SysLogModelMap()
        {
            // Primary Key

            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.Type).IsRequired();
            Property(t => t.Datetime).IsRequired();
            Property(t => t.UserID).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Message).IsRequired().HasColumnType("varchar").HasMaxLength(2000);
            Property(t => t.Create_user).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings
            ToTable("SYS_LOG");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.Type).HasColumnName("TYPES");
            Property(t => t.Datetime).HasColumnName("DATE_TIME");
            Property(t => t.UserID).HasColumnName("USER_ID");
            Property(t => t.Message).HasColumnName("MESSAGES");
            Property(t => t.Create_user).HasColumnName("CREATE_USER");
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");

            // Relationships
        }
    }
}
