using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto.Mapping
{
    public partial class Sin_PatientMap : EntityTypeConfiguration<Sin_Patient>
    {
        public Sin_PatientMap() 
        {
            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.Create_user).IsRequired();
            Property(t => t.Create_time).IsRequired();

            // Table & Column Mappings
            ToTable("SIN_PATIENT");
            Property(o => o.Id).HasColumnName("ID");
            Property(o => o.Name).HasColumnName("NAME").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Sex).HasColumnName("SEX");
            Property(o => o.Age).HasColumnName("AGE");
            Property(o => o.Age_unit).HasColumnName("AGE_UNIT");
            Property(o => o.Medical_num).HasColumnName("MEDICAL_NUM").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Treatment_type).HasColumnName("TREATMENT_TYPE").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Nation).HasColumnName("NATION").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Charge_type).HasColumnName("CHARGE_TYPE").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Area).HasColumnName("AREA").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Ward).HasColumnName("WARD").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Bed).HasColumnName("BED").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Regist_date).HasColumnName("REGIST_DATE");
            Property(o => o.Regist_datetime).HasColumnName("REGIST_DATETIME");
            Property(o => o.Delete_flag).HasColumnName("DELETE_FLAG");
            Property(o => o.Create_user).HasColumnName("CREATE_USER").HasColumnType("VARCHAR").HasMaxLength(50);
            Property(o => o.Create_time).HasColumnName("CREATE_TIME");
        }
    }
}
