using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels.Mapping
{
    /// <summary>
    /// ◆单元名称：版本信息实体表部署
    /// ◆功能描述：
    /// ◆创建人  ： sunch
    /// ◆创建日期： 2019/3/13 13:54
    /// ◆修改人  ：
    /// ◆修改日期：
    /// ◆修改原因：
    /// </summary>
    public class VersionModelMap : EntityTypeConfiguration<VersionModel>
    {
        public VersionModelMap()
        {
            // Primary Key

            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.DBI_NAME).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.DBI_VALUE).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Create_user).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings
            ToTable("ACA_DATABASEINFO");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.DBI_NAME).HasColumnName("DBI_NAME");
            Property(t => t.DBI_VALUE).HasColumnName("DBI_VALUE");
            Property(t => t.Create_user).HasColumnName("CREATE_USER");
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");

            // Relationships

        }
    }
}
