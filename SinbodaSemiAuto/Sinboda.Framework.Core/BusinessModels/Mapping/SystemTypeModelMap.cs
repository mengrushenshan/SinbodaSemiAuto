using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels.Mapping
{
    /// <summary>
    /// ◆单元名称：系统类型-基础信息部署关系
    /// ◆功能描述：
    /// ◆创建人  ： sunch
    /// ◆创建日期： 2019/3/13 14:00
    /// ◆修改人  ：
    /// ◆修改日期：
    /// ◆修改原因：
    /// </summary>
    public class SystemTypeModelMap : EntityTypeConfiguration<SystemTypeModel>
    {
        public SystemTypeModelMap()
        {
            // Primary Key

            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.Code).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Values).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.LanguageID).HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Order).IsRequired();
            Property(t => t.IsEnable).IsRequired();
            Property(t => t.Create_user).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings
            ToTable("SYSTEM_TYPE");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.Code).HasColumnName("CODE");
            Property(t => t.Values).HasColumnName("CODE_VALUES");
            Property(t => t.LanguageID).HasColumnName("LANGUAGE_ID");
            Property(t => t.Order).HasColumnName("ORDERS");
            Property(t => t.IsEnable).HasColumnName("IS_ENABLE");
            Property(t => t.Create_user).HasColumnName("CREATE_USER");
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");

            // Relationships
        }
    }

    /// <summary>
    /// ◆单元名称：系统类型-基础信息部署关系
    /// ◆功能描述：
    /// ◆创建人  ： sunch
    /// ◆创建日期： 2019/3/13 14:00
    /// ◆修改人  ：
    /// ◆修改日期：
    /// ◆修改原因：
    /// </summary>
    public class SystemTypeValueModelMap : EntityTypeConfiguration<SystemTypeValueModel>
    {
        public SystemTypeValueModelMap()
        {
            // Primary Key

            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.LanguageID).IsRequired();
            Property(t => t.Code).IsRequired();
            Property(t => t.DisplayValue).IsRequired().HasColumnType("varchar").HasMaxLength(100);
            Property(t => t.Order).IsRequired();
            Property(t => t.IsEnable).IsRequired();
            Property(t => t.IsDefault).IsRequired();
            Property(t => t.CodeGroupID).IsRequired();
            Property(t => t.Create_user).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings
            ToTable("SYSTEM_TYPE_VALUE");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.LanguageID).HasColumnName("LANGUAGE_ID");
            Property(t => t.Code).HasColumnName("CODE");
            Property(t => t.DisplayValue).HasColumnName("DISPLAY_VALUE");
            Property(t => t.Order).HasColumnName("ORDERS");
            Property(t => t.IsEnable).HasColumnName("IS_ENABLE");
            Property(t => t.IsDefault).HasColumnName("IS_DEFAULT");
            Property(t => t.CodeGroupID).HasColumnName("CODE_GROUP_ID");
            Property(t => t.Create_user).HasColumnName("CREATE_USER");
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");

            // Relationships
        }
    }
}
