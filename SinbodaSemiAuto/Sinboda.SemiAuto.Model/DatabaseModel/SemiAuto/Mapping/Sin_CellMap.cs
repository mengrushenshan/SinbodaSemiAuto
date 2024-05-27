using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto.Mapping
{
    public partial class Sin_CellMap : EntityTypeConfiguration<Sin_Cell>
    {
        public Sin_CellMap() 
        {
            ToTable("Sin_Cell");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.Index).HasColumnName("INDEX");
            Property(t => t.X).HasColumnName("X");
            Property(t => t.Y).HasColumnName("Y");
            Property(t => t.Z).HasColumnName("Z");
            Property(t => t.Create_user).HasColumnName("CREATE_USER").HasColumnType("VARCHAR").HasMaxLength(50);
            Property(t => t.Create_time).HasColumnName("CREATE_TIME");
        }
    }
}
