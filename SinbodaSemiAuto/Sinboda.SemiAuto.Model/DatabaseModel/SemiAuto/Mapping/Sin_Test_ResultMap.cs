using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto.Mapping
{
    public partial class Sin_Test_ResultMap : EntityTypeConfiguration<Sin_Test_Result>
    {
        public Sin_Test_ResultMap() 
        {
            // Properties
            Property(t => t.Id).IsRequired();
            Property(t => t.Sample_id).IsRequired();
            //Property(t => t.Result_hint).IsRequired();
            Property(t => t.Test_state).IsRequired();
            Property(t => t.Result_update_flag).IsRequired();
            Property(t => t.Recheck_flag).IsRequired();
            Property(t => t.Create_user).IsRequired();
            Property(t => t.Create_time).IsRequired();
            Property(t => t.Item_type).IsRequired();

            // Table & Column Mappings
            ToTable("SIN_TEST_RESULT");
            Property(o => o.Id).HasColumnName("ID");
            Property(o => o.Sample_id).HasColumnName("SAMPLE_ID");
            Property(o => o.Test_num).HasColumnName("TEST_NUM");
            Property(o => o.Item_test_name).HasColumnName("ITEM_TEST_NAME").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Item_type).HasColumnName("ITEM_TYPE");
            Property(o => o.Unit).HasColumnName("UNIT").HasColumnType("VARCHAR").HasMaxLength(20);
            Property(o => o.Item_reagent_lotno).HasColumnName("ITEM_REAGENT_LOTNO");
            Property(o => o.Result).HasColumnName("RESULT");
            Property(o => o.Result_original).HasColumnName("RESULT_ORIGINAL");
            Property(o => o.Reference_range).HasColumnName("REFERENCE_RANGE").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Linear_range).HasColumnName("LINEAR_RANGE").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Crisis_range).HasColumnName("CRISIS_RANGE").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Sample_send_date).HasColumnName("SAMPLE_SEND_DATE");
            Property(o => o.Sample_test_time).HasColumnName("SAMPLE_TEST_TIME");
            Property(o => o.Sample_test_end_time).HasColumnName("SAMPLE_TEST_END_TIME");
            Property(o => o.Test_state).HasColumnName("TEST_STATE");
            Property(o => o.ResultRangeMark).HasColumnName("RESULT_RANGE_MARK");
            Property(o => o.ResultErrorMark).HasColumnName("RESULT_ERROR_MARK");
            Property(o => o.Test_result_type).HasColumnName("TEST_RESULT_TYPE");
            Property(o => o.Result_update_flag).HasColumnName("RESULT_UPDATE_FLAG");
            Property(o => o.Recheck_reason).HasColumnName("RECHECK_REASON").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Recheck_flag).HasColumnName("RECHECK_FLAG");
            Property(o => o.Test_file_name).HasColumnName("TEST_FILE_NAME").HasColumnType("VARCHAR").HasMaxLength(200);
            Property(o => o.Digits).HasColumnName("DIGITS");
            Property(o => o.NonEditable).HasColumnName("NON_EDITABLE");
            Property(o => o.Create_time).HasColumnName("CREATE_TIME");
            Property(o => o.Create_user).HasColumnName("CREATE_USER").HasColumnType("VARCHAR").HasMaxLength(50);
        }
    }
}
