using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels.Mapping;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model
{
    public class Sin_DbContext : DBContextBase
    {
        public Sin_DbContext()
        {
            Configuration.AutoDetectChangesEnabled = false;
        }

        /// <summary>
        /// 样本表
        /// </summary>
        public DbSet<Sin_Sample> Sin_Samples { get; set; }

        /// <summary>
        /// 结果表
        /// </summary>
        public DbSet<Sin_Test_Result> Sin_Test_Results { get; set; }

        /// <summary>
        /// 患者信息表
        /// </summary>
        public DbSet<Sin_Patient> sin_Patients { get; set; }

        /// <summary>
        /// 项目表
        /// </summary>
        public DbSet<Sin_Item> Sin_Items { get; set; }

        /// <summary>
        /// 电机表
        /// </summary>
        public DbSet<Sin_Motor> Sin_Motors { get; set; }

        /// <summary>
        /// 模板表
        /// </summary>
        public DbSet<Sin_BoardTemplate> Sin_BoardTemplates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // 平台使用
            modelBuilder.Configurations.Add(new SysLogModelMap());
            modelBuilder.Configurations.Add(new AlarmHistoryInfoModelMap());
            modelBuilder.Configurations.Add(new SystemTypeModelMap());
            modelBuilder.Configurations.Add(new SystemTypeValueModelMap());
            modelBuilder.Configurations.Add(new DataDictionaryTypeModelMap());
            modelBuilder.Configurations.Add(new DataDictionaryInfoModelMap());
            modelBuilder.Configurations.Add(new ModuleTypeModelMap());
            modelBuilder.Configurations.Add(new ModuleInfoModelMap());
            modelBuilder.Configurations.Add(new VersionModelMap());
            modelBuilder.Configurations.Add(new ModuleVersionModelMap());
            modelBuilder.Configurations.Add(new SysRegisterModelMap());

            // 产品使用
            modelBuilder.Configurations.Add(new Sin_SampleMap());
            modelBuilder.Configurations.Add(new Sin_ItemsMap());
            modelBuilder.Configurations.Add(new Sin_PatientMap());
            modelBuilder.Configurations.Add(new Sin_Test_ResultMap());
            modelBuilder.Configurations.Add(new Sin_MotorMap());
            modelBuilder.Configurations.Add(new Sin_CellMap());
            modelBuilder.Configurations.Add(new Sin_BoardTemplateMap());
        }
    }
}
