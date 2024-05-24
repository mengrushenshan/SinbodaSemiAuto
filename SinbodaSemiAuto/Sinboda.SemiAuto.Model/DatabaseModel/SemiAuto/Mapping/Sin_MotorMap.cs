using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto.Mapping
{
    public partial class Sin_MotorMap : EntityTypeConfiguration<Sin_Motor>
    {
        public Sin_MotorMap() 
        {
            ToTable("SIN_MOTOR");

            Property(o => o.Id).HasColumnName("ID");
            Property(o => o.MotorId).HasColumnName("MOTOR_ID");
            Property(o => o.Dir).HasColumnName("DIR");
            Property(o => o.UseFastSpeed).HasColumnName("USE_FAST_SPEED");
            Property(o => o.Steps).HasColumnName("STEPS");
            Property(o => o.OriginPoint).HasColumnName("ORIGIN_POINT");
            Property(o => o.TargetPos).HasColumnName("TARGET_POS");
            Property(o => o.Create_user).HasColumnName("CREATE_USER").HasColumnType("VARCHAR").HasMaxLength(50);
            Property(o => o.Create_time).HasColumnName("CREATE_TIME");
        }
    }
}
