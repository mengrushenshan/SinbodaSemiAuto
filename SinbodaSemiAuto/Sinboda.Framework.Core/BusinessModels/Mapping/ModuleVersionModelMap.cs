using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels.Mapping
{
    public class ModuleVersionModelMap : EntityTypeConfiguration<ModuleVersionModel>
    {
        public ModuleVersionModelMap()
        {
            // Primary Key

            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.ModuleID).IsRequired();
            Property(t => t.SerialNO).IsRequired().HasColumnType("varchar").HasMaxLength(200);
            Property(t => t.ModuleName).IsRequired().HasColumnType("varchar").HasMaxLength(200);
            Property(t => t.VersionInfo).IsRequired().HasColumnType("varchar").HasMaxLength(200);
            Property(t => t.UpdateTime).IsRequired();
            Property(t => t.Create_user).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings
            ToTable("MODULE_VERSION_MODEL");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.ModuleID).HasColumnName("MODULE_ID");
            Property(t => t.SerialNO).HasColumnName("SERIAL_NO");
            Property(t => t.ModuleName).HasColumnName("MODULE_NAMES");
            Property(t => t.VersionInfo).HasColumnName("VERSION_INFO");
            Property(t => t.UpdateTime).HasColumnName("UPDATE_TIME");
            Property(t => t.Create_user).HasColumnName("CREATE_USER");
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");

            // Relationships
        }
    }
}
