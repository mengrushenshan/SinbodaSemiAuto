using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels.Mapping
{
    /// <summary>
    /// ◆单元名称：基础信息类型
    /// ◆功能描述：
    /// ◆创建人  ： sunch
    /// ◆创建日期： 2019/3/13 13:54
    /// ◆修改人  ：
    /// ◆修改日期：
    /// ◆修改原因：
    /// </summary>
    public class DataDictionaryTypeModelMap : EntityTypeConfiguration<DataDictionaryTypeModel>
    {
        public DataDictionaryTypeModelMap()
        {
            // Primary Key

            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.LanguageID).IsRequired();
            Property(t => t.TypeCode).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.TypeValues).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.ParentCode).HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.ShowOrder).IsRequired();
            Property(t => t.IsEnable).IsRequired();
            Property(t => t.IsSetHotKey).IsRequired();
            Property(t => t.IsSetDefault).IsRequired();
            Property(t => t.IsSetParentCode).IsRequired();
            Property(t => t.MaxLengthValue).IsRequired();
            Property(t => t.RegexTextValue).HasColumnName("varchar").HasMaxLength(200);
            Property(t => t.Create_user).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings
            ToTable("DATA_DICTIONARY_TYPE");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.LanguageID).HasColumnName("LANGUAGE_ID");
            Property(t => t.TypeCode).HasColumnName("TYPE_CODE");
            Property(t => t.TypeValues).HasColumnName("TYPE_VALUES");
            Property(t => t.ParentCode).HasColumnName("PARENT_CODE");
            Property(t => t.ShowOrder).HasColumnName("SHOW_ORDER");
            Property(t => t.IsEnable).HasColumnName("IS_ENABLE");
            Property(t => t.IsSetHotKey).HasColumnName("IS_SET_HOTKEY");
            Property(t => t.IsSetDefault).HasColumnName("IS_SET_DEFAULT");
            Property(t => t.IsSetParentCode).HasColumnName("IS_SET_PARENTCODE");
            Property(t => t.MaxLengthValue).HasColumnName("MAX_LENGTH_VALUE");
            Property(t => t.RegexTextValue).HasColumnName("MAX_REGEX_VALUE"); ;
            Property(t => t.IsChildCanDelete).HasColumnName("IS_CHILD_CAN_DELETE");
            Property(t => t.Create_user).HasColumnName("CREATE_USER");
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");

            // Relationships
        }
    }

    /// <summary>
    /// 基础信息信息
    /// </summary>
    public class DataDictionaryInfoModelMap : EntityTypeConfiguration<DataDictionaryInfoModel>
    {
        public DataDictionaryInfoModelMap()
        {
            // Primary Key

            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.Code).HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.LanguageID).IsRequired();
            Property(t => t.Values).IsRequired().HasColumnType("varchar").HasMaxLength(200);
            Property(t => t.ParentCode);
            Property(t => t.HotKey).HasColumnName("varchar").HasMaxLength(50);
            Property(t => t.ShowOrder).IsRequired();
            Property(t => t.IsEnable).IsRequired();
            Property(t => t.IsDefault).IsRequired();
            Property(t => t.CodeGroupID).IsRequired();
            Property(t => t.Create_user).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings
            ToTable("DATA_DICTIONARY_INFO");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.Code).HasColumnName("CODE");
            Property(t => t.LanguageID).HasColumnName("LANGUAGE_ID");
            Property(t => t.Values).HasColumnName("CODE_VALUES");
            Property(t => t.ParentCode).HasColumnName("PARENT_CODE");
            Property(t => t.HotKey).HasColumnName("HOT_KEY");
            Property(t => t.ShowOrder).HasColumnName("SHOW_ORDER");
            Property(t => t.IsEnable).HasColumnName("IS_ENABLE");
            Property(t => t.IsDefault).HasColumnName("IS_DEFAULT");
            Property(t => t.CodeGroupID).HasColumnName("CODE_GROUP_ID");
            Property(t => t.Create_user).HasColumnName("CREATE_USER");
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");

            // Relationships
        }
    }
}
