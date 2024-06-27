using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto.Mapping
{
    public class Sin_BoardMap : EntityTypeConfiguration<Sin_Board>
    {
        public Sin_BoardMap() 
        {
            ToTable("SIN_BOARD");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.BoardId).HasColumnName("BOARD_ID");
            Property(t => t.Rack).HasColumnName("RACK");
            Property(t => t.Position).HasColumnName("POSITION");
            Property(t => t.TestType).HasColumnName("TEST_TYPE");
            Property(t => t.ItemName).HasColumnName("ITEM_NAME");
            Property(t => t.IsEnable).HasColumnName("IS_ENABLE");
            Property(t => t.RegistDate).HasColumnName("REGIST_DATE");
            Property(t => t.Create_user).HasColumnName("CREATE_USER").HasColumnType("VARCHAR").HasMaxLength(50);
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");
        }
    }
}
