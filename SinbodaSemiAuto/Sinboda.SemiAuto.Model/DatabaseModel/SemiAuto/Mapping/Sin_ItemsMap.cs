using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto.Mapping
{
    public partial class Sin_ItemsMap : EntityTypeConfiguration<Sin_Item>
    {
        public Sin_ItemsMap()
        {
            // Primary Key
            ToTable("SIN_ITEMS");
            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.ItemName).IsRequired();
            Property(t => t.Full_name).IsRequired();
            Property(t => t.Item_type).IsRequired();
            Property(t => t.Barcode_code).IsRequired();
            Property(t => t.Print_code).IsRequired();
            Property(t => t.Test_order).IsRequired();
            Property(t => t.Enabled).IsRequired();
            Property(t => t.Param_enabled).IsRequired();
            Property(t => t.Create_user).IsRequired();
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings

            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.Item_id).HasColumnName("ITEM_ID");
            Property(t => t.ItemName).HasColumnName("SHORT_NAME").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(t => t.Full_name).HasColumnName("FULL_NAME").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(t => t.LangID).HasColumnName("LANGID").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(t => t.Item_type).HasColumnName("ITEM_TYPE");
            Property(t => t.Barcode_code).HasColumnName("BARCODE_CODE").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(t => t.Print_code).HasColumnName("PRINT_CODE");
            Property(t => t.Lis_code).HasColumnName("LIS_CODE").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(t => t.Test_order).HasColumnName("TEST_ORDER");
            Property(t => t.Enabled).HasColumnName("ENABLED");
            Property(t => t.Param_enabled).HasColumnName("PARAM_ENABLED");
            Property(t => t.Item_is_calibrated).HasColumnName("ITEM_IS_CALIBRATED");
            Property(t => t.Create_user).HasColumnName("CREATE_USER").HasColumnType("VARCHAR").HasMaxLength(50);
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");
        }
    }
}
