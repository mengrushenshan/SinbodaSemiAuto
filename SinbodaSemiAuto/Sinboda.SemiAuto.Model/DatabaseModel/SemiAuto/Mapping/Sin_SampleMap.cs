using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto.Mapping
{
    public partial class Sin_SampleMap : EntityTypeConfiguration<Sin_Sample>
    {
        public Sin_SampleMap()
        {
            ToTable("SIN_SAMPLE");

            Property(o => o.Id).HasColumnName("ID");
            Property(o => o.Patient_id).HasColumnName("PATIENT_ID");
            Property(o => o.SampleCode).HasColumnName("SAMPLE_CODE");
            Property(o => o.Barcode).HasColumnName("BARCODE").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.RackDish).HasColumnName("RACK_DISH");
            Property(o => o.Position).HasColumnName("SAMPLE_POSITION");
            Property(o => o.Sample_date).HasColumnName("SAMPLE_DATE");
            Property(o => o.Test_state).HasColumnName("TEST_STATE");
            Property(o => o.Delete_flag).HasColumnName("DELETE_FLAG");
            Property(o => o.Expiry_date).HasColumnName("EXPIRY_DATE");
            Property(o => o.Send_office).HasColumnName("SEND_OFFICE").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Send_time).HasColumnName("SEND_TIME");
            Property(o => o.Send_doctor).HasColumnName("SEND_DOCTOR").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Sampling_time).HasColumnName("SAMPLING_TIME");
            Property(o => o.Test_doctor).HasColumnName("TEST_DOCTOR").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Test_time).HasColumnName("TEST_TIME");
            Property(o => o.Remark).HasColumnName("REMARK").HasColumnType("VARCHAR").HasMaxLength(500);
            Property(o => o.BoardId).HasColumnName("BOARD_ID");
            Property(o => o.Create_user).HasColumnName("CREATE_USER").HasColumnType("VARCHAR").HasMaxLength(50);
            Property(o => o.Create_time).HasColumnName("CREATE_TIME");
        }
    }
}
