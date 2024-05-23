using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels.Mapping
{
    /// <summary>
    /// ◆单元名称：模块类型表
    /// ◆功能描述：
    /// ◆创建人  ： sunch
    /// ◆创建日期： 2019/3/13 13:54
    /// ◆修改人  ：
    /// ◆修改日期：
    /// ◆修改原因：
    /// </summary>
    public class ModuleTypeModelMap : EntityTypeConfiguration<ModuleTypeModel>
    {
        public ModuleTypeModelMap()
        {
            // Primary Key

            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.LanguageID).IsRequired();
            Property(t => t.ModuleTypeName).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.ModuleTypeCode).IsRequired();
            Property(t => t.Create_user).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings
            ToTable("MODULE_TYPE");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.LanguageID).HasColumnName("LANGUAGE_ID");
            Property(t => t.ModuleTypeName).HasColumnName("MODULE_TYPE_NAME");
            Property(t => t.ModuleTypeCode).HasColumnName("MODULE_TYPE_CODE");
            Property(t => t.IsShow).HasColumnName("IS_SHOW");
            Property(t => t.Create_user).HasColumnName("CREATE_USER");
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");

            // Relationships
        }
    }

    /// <summary>
    /// 模块信息表
    /// </summary>
    public class ModuleInfoModelMap : EntityTypeConfiguration<ModuleInfoModel>
    {
        public ModuleInfoModelMap()
        {
            // Primary Key

            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.ModuleID).IsRequired();
            Property(t => t.ModuleName).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.ModuleType).IsRequired();
            Property(t => t.Create_user).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings
            ToTable("MODULE_INFO");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.ModuleID).HasColumnName("MODULE_ID");
            Property(t => t.ModuleName).HasColumnName("MODULE_NAMES");
            Property(t => t.LanguageID).HasColumnName("LANGUAGE_ID");
            Property(t => t.ModuleType).HasColumnName("MODULE_TYPES");
            Property(t => t.IsShow).HasColumnName("IS_SHOW");
            Property(t => t.Create_user).HasColumnName("CREATE_USER");
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");

            // Relationships
        }
    }
}
