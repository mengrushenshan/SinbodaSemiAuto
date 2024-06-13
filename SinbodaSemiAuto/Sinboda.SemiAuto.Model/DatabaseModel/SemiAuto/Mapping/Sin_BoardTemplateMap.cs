using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto.Mapping
{
    public class Sin_BoardTemplateMap : EntityTypeConfiguration<Sin_BoardTemplate>
    {
        public Sin_BoardTemplateMap()
        {
            ToTable("SIN_BOARDTEMPLATE");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.TemplateName).HasColumnName("TEMPLATE_NAME");
            Property(t => t.Rack).HasColumnName("RACK");
            Property(t => t.Position).HasColumnName("POSITION");
            Property(t => t.TestType).HasColumnName("TEST_TYPE");
            Property(t => t.ItemName).HasColumnName("ITEM_NAME");
            Property(t => t.Create_user).HasColumnName("CREATE_USER").HasColumnType("VARCHAR").HasMaxLength(50);
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");
        }
    }
}
