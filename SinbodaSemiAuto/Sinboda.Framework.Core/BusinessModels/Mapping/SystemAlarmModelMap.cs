using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels.Mapping
{
    /// <summary>
    /// ◆单元名称：报警数据实体部署关系
    /// ◆功能描述：
    /// ◆创建人  ： sunch
    /// ◆创建日期： 2019/3/13 14:00
    /// ◆修改人  ：
    /// ◆修改日期：
    /// ◆修改原因：
    /// </summary>
    public class AlarmHistoryInfoModelMap : EntityTypeConfiguration<AlarmHistoryInfoModel>
    {
        public AlarmHistoryInfoModelMap()
        {
            // Primary Key

            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.Code).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.CodeLevel).IsRequired();
            Property(t => t.Info).IsRequired().HasColumnType("varchar").HasMaxLength(2000);
            //Property(t => t.DetailInfo).HasColumnType("varchar").HasMaxLength(2000);
            //Property(t => t.Solution).HasColumnType("varchar").HasMaxLength(2000);
            Property(t => t.AlarmTime).IsRequired();
            Property(t => t.AlarmStyle).IsRequired();
            Property(t => t.ModuleID).IsRequired();
            Property(t => t.ModuleType).IsRequired();
            Property(t => t.DeletedFlag);
            Property(t => t.Parameters).HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.InstrumentModuleId);
            Property(t => t.Create_user).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings
            ToTable("ALARM_HISTORY_INFO");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.Code).HasColumnName("CODE");
            Property(t => t.CodeLevel).HasColumnName("CODE_VALUE");
            Property(t => t.Info).HasColumnName("INFO");
            //Property(t => t.DetailInfo).HasColumnName("DETAIL_INFO");
            //Property(t => t.Solution).HasColumnName("SOLUTION");
            Property(t => t.AlarmTime).HasColumnName("ALARM_TIME");
            Property(t => t.AlarmStyle).HasColumnName("ALARM_STYLE");
            Property(t => t.ModuleID).HasColumnName("MODULE_ID");
            Property(t => t.ModuleType).HasColumnName("MODULE_TYPE_ID");
            Property(t => t.DeletedFlag).HasColumnName("DELETE_FLAG");
            Property(t => t.Parameters).HasColumnName("PARAMETERS");
            Property(t => t.InstrumentModuleId).HasColumnName("INSTRUMENT_ID");
            Property(t => t.Create_user).HasColumnName("CREATE_USER");
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");

            // Relationships
        }
    }
}
