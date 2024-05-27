using Sinboda.Framework.Core.BusinessModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.StaticResource;
using System.Security.Cryptography;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.Framework.Core.Interface;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;

namespace Sinboda.SemiAuto.Model
{
    public class SinDbContextInitialize : IDbContextInitialize
    {
        public Type DatabaseType { get; set; }

        public SinDbContextInitialize()
        {
            DatabaseType = typeof(Sin_DbContext);
        }

        /// <summary>
        /// 如果数据库存在忽略EntityFramework自己的数据结构比较，实现方式为Database.SetInitializer<T>(null);，T为继承自Dirui.Framework.Core.AbstractClass.DBContextBase的实现类
        /// </summary>  
        public void IfExistIgnoreCreate()
        {
            using (var db = new Sin_DbContext())
            {
                Database.SetInitializer<Sin_DbContext>(null);

                if (db.ModuleTypeModel.FirstOrDefault(o => o.ModuleTypeCode == 1) != null)
                {
                    var moduleType = db.ModuleTypeModel.FirstOrDefault(o => o.ModuleTypeCode == 1);
                    DbEntityEntry entryModify = db.Entry<ModuleTypeModel>(moduleType);
                    entryModify.State = EntityState.Modified;
                    moduleType.IsShow = false;
                    db.SaveChanges();
                }

                if (db.ModuleTypeModel.FirstOrDefault(o => o.ModuleTypeCode == 3) != null)
                {
                    var moduleType = db.ModuleTypeModel.FirstOrDefault(o => o.ModuleTypeCode == 3);
                    DbEntityEntry entryModify = db.Entry<ModuleTypeModel>(moduleType);
                    entryModify.State = EntityState.Modified;
                    moduleType.IsShow = false;
                    db.SaveChanges();
                }

                if (db.ModuleTypeModel.FirstOrDefault(o => o.ModuleTypeCode == 4) != null)
                {
                    var moduleType = db.ModuleTypeModel.FirstOrDefault(o => o.ModuleTypeCode == 4);
                    DbEntityEntry entryModify = db.Entry<ModuleTypeModel>(moduleType);
                    entryModify.State = EntityState.Modified;
                    moduleType.IsShow = false;
                    db.SaveChanges();
                }

                if (db.ModuleInfoModel.FirstOrDefault(o => o.ModuleID == 11) != null)
                {
                    var moduleType = db.ModuleInfoModel.FirstOrDefault(o => o.ModuleID == 11);
                    DbEntityEntry entryModify = db.Entry<ModuleInfoModel>(moduleType);
                    entryModify.State = EntityState.Modified;
                    moduleType.IsShow = false;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 数据库创建
        /// </summary>
        public void InitializeDB()
        {
            //数据库实例
            using (Sin_DbContext db = new Sin_DbContext())
            {
                db.Database.Create();
            }
        }

        /// <summary>
        /// 数据库初始化数据
        /// </summary>  
        public void InitializeData()
        {
            CreateDefault();
            InputDatabaseIntoDB();
        }

        /// <summary>
        /// 调用外部exe，初始化项目和项目参数
        /// </summary>
        public void InputDatabaseIntoDB()
        {
            try
            {
                string str = AppDomain.CurrentDomain.BaseDirectory + "InputRealDatabase.exe";
                System.Diagnostics.Process exep = new System.Diagnostics.Process();
                exep.StartInfo.FileName = str;
                exep.StartInfo.CreateNoWindow = true;
                exep.StartInfo.UseShellExecute = false;
                exep.Start();
                exep.WaitForExit();//关键，等待外部程序退出后才能往下执行​
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("初始化数据出现错误", ex);
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void CreateDefault()
        {
            using (Sin_DbContext db = new Sin_DbContext())
            {
                var itemUnit = new DataDictionaryTypeModel()
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 408,
                    TypeCode = "itemUnit",
                    TypeValues = SystemResources.Instance.LanguageArray[408],
                    ShowOrder = 12,
                    IsSetHotKey = false,
                    IsSetDefault = true,
                    IsEnable = true,
                    IsSetParentCode = false,
                    MaxLengthValue = 30,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryTypeModel.Add(itemUnit);
                db.DataDictionaryTypeModel.Add(new DataDictionaryTypeModel
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 1895,
                    TypeCode = "charge",
                    TypeValues = SystemResources.Instance.LanguageArray[1895],
                    ShowOrder = 20,
                    IsSetHotKey = true,
                    IsSetDefault = true,
                    IsEnable = true,
                    IsSetParentCode = false,
                    MaxLengthValue = 30,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                });

                #region 项目单位信息
                var itemUnitKid1 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "1",
                    LanguageID = 0,
                    ShowOrder = 1,
                    IsDefault = true,
                    IsEnable = true,
                    Values = "%",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid1);
                var itemUnitKid2 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "2",
                    LanguageID = 0,
                    ShowOrder = 2,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "g/dL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid2);
                var itemUnitKid3 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "3",
                    LanguageID = 0,
                    ShowOrder = 3,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "g/L",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid3);
                var itemUnitKid4 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "4",
                    LanguageID = 0,
                    ShowOrder = 4,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "IU/L",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid4);
                var itemUnitKid5 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "5",
                    LanguageID = 0,
                    ShowOrder = 5,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "IU/mL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid5);
                var itemUnitKid6 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "6",
                    LanguageID = 0,
                    ShowOrder = 6,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "mg/dL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid6);
                var itemUnitKid7 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "7",
                    LanguageID = 0,
                    ShowOrder = 7,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "mg/L",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid7);
                var itemUnitKid8 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "8",
                    LanguageID = 0,
                    ShowOrder = 8,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "mIU/L",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid8);
                var itemUnitKid9 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "9",
                    LanguageID = 0,
                    ShowOrder = 9,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "mIU/mL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid9);
                var itemUnitKid10 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "10",
                    LanguageID = 0,
                    ShowOrder = 10,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "mmol/L",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid10);
                var itemUnitKid11 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "11",
                    LanguageID = 0,
                    ShowOrder = 11,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "mol/L",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid11);
                var itemUnitKid12 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "12",
                    LanguageID = 0,
                    ShowOrder = 12,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "mU/L",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid12);
                var itemUnitKid13 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "13",
                    LanguageID = 0,
                    ShowOrder = 13,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "ng/dL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid13);
                var itemUnitKid14 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "14",
                    LanguageID = 0,
                    ShowOrder = 14,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "ng/mL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid14);
                var itemUnitKid15 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "15",
                    LanguageID = 0,
                    ShowOrder = 15,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "nmol/L",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid15);
                var itemUnitKid16 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "16",
                    LanguageID = 0,
                    ShowOrder = 16,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "nmol/mL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid16);
                var itemUnitKid17 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "17",
                    LanguageID = 0,
                    ShowOrder = 17,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "pg/mL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid17);
                var itemUnitKid18 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "18",
                    LanguageID = 0,
                    ShowOrder = 18,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "pmol/mL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid18);
                var itemUnitKid19 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "19",
                    LanguageID = 0,
                    ShowOrder = 19,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "U/L",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid19);
                var itemUnitKid20 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "20",
                    LanguageID = 0,
                    ShowOrder = 20,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "U/mL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid20);
                var itemUnitKid21 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "21",
                    LanguageID = 0,
                    ShowOrder = 21,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "μg/dL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid21);
                var itemUnitKid22 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "22",
                    LanguageID = 0,
                    ShowOrder = 22,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "μg/L",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid22);
                var itemUnitKid23 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "23",
                    LanguageID = 0,
                    ShowOrder = 23,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "μg/mL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid23);
                var itemUnitKid24 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "24",
                    LanguageID = 0,
                    ShowOrder = 24,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "μIU/mL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid24);
                var itemUnitKid25 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "25",
                    LanguageID = 0,
                    ShowOrder = 25,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "μmol/mL",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid25);
                var itemUnitKid26 = new DataDictionaryInfoModel()
                {
                    Id = Guid.NewGuid(),
                    CodeGroupID = itemUnit.Id,
                    Code = "26",
                    LanguageID = 0,
                    ShowOrder = 26,
                    IsDefault = false,
                    IsEnable = true,
                    Values = "s",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryInfoModel.Add(itemUnitKid26);
                db.SaveChanges();
                #endregion

                #region 模块类型

                var platformModelType = new ModuleTypeModel
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 6713,
                    ModuleTypeName = ProductType.Platform.ToString(),
                    ModuleTypeCode = (int)ProductType.Platform,
                    Create_time = DateTime.Now,
                    Create_user = "Sinboda"
                };

                var server = new ModuleTypeModel
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 7906,
                    ModuleTypeName = ProductType.Server.ToString(),
                    ModuleTypeCode = (int)ProductType.Server,
                    IsShow = false,
                    Create_time = DateTime.Now,
                    Create_user = "Sinboda"
                };

                var sinboda001ModelType = new ModuleTypeModel
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 1311,
                    ModuleTypeName = ProductType.Sinboda001.ToString(),
                    ModuleTypeCode = (int)ProductType.Sinboda001,
                    Create_time = DateTime.Now,
                    Create_user = "Sinboda"
                };

                db.ModuleTypeModel.Add(platformModelType);
                db.ModuleTypeModel.Add(server);
                db.ModuleTypeModel.Add(sinboda001ModelType);

                db.SaveChanges();

                #endregion

                #region 初始化数据

                #region 模块信息

                db.ModuleInfoModel.Add(new ModuleInfoModel
                {
                    Id = Guid.NewGuid(),
                    ModuleID = 11,
                    ModuleName = "中位机",
                    ModuleType = server.Id,
                    Create_time = DateTime.Now,
                    Create_user = "Sinboda"
                });

                db.SaveChanges();

                #endregion

                #region 项目信息

                db.Sin_Items.Add(new Sin_Item
                {
                    Id = Guid.NewGuid(),
                    Item_id = 1,
                    ItemName = "AD",
                    Full_name = "AD",
                    LangID = "0",
                    Item_type = ItemType.SingleMolecule,
                    Barcode_code = "",
                    Print_code = 1,
                    Lis_code = "AD",
                    Test_order = 1,
                    Enabled = true,
                    Param_enabled = true,
                    Item_is_calibrated = false,
                    Create_time = DateTime.Now,
                    Create_user = "Sinboda"
                });

                db.Sin_Items.Add(new Sin_Item
                {
                    Id = Guid.NewGuid(),
                    Item_id = 2,
                    ItemName = "PD",
                    Full_name = "PD",
                    LangID = "0",
                    Item_type = ItemType.SingleMolecule,
                    Barcode_code = "",
                    Print_code = 2,
                    Lis_code = "PD",
                    Test_order = 2,
                    Enabled = true,
                    Param_enabled = true,
                    Item_is_calibrated = false,
                    Create_time = DateTime.Now,
                    Create_user = "Sinboda"
                });
                db.SaveChanges();

                #endregion

                #region

                db.Sin_Motors.Add(new Sin_Motor
                {
                    Id = Guid.NewGuid(),
                    MotorId = MotorId.Xaxis,
                    Dir = Direction.Forward,
                    UseFastSpeed = Rate.fast,
                    Steps = 1000,
                    OriginPoint = 0,
                    TargetPos = 0
                }) ;

                db.Sin_Motors.Add(new Sin_Motor
                {
                    Id = Guid.NewGuid(),
                    MotorId = MotorId.Yaxis,
                    Dir = Direction.Forward,
                    UseFastSpeed = Rate.fast,
                    Steps = 1000,
                    OriginPoint = 0,
                    TargetPos = 0
                });

                db.SaveChanges();
                #endregion

                #endregion

                #region 初始化枚举

                InitEnums();
                db.SaveChanges();

                #endregion

            }
        }

        private void InitEnums()
        {
            var db = new SystemValueContext();
            List<Type> types = EnumAnnotate.GetEnum();
            foreach (Type type in types)
            {
                InitEnum(type, db);
            }
            db.SaveChanges();
        }
        private string MD5Encrypt(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(str));
            string encryptStr = System.Text.Encoding.Default.GetString(result);
            return encryptStr;
        }
        private void InitEnum(Type e, SystemValueContext db)
        {
            try
            {
                var enumAnnotate = EnumAnnotate.Get(e);
                if (enumAnnotate != null)
                {
                    SystemTypeModel stm = new SystemTypeModel
                    {
                        Id = Guid.NewGuid(),
                        Code = e.Name,
                        IsEnable = true,
                        Create_time = DateTime.Now,
                        Create_user = "Sinboda",
                        Values = enumAnnotate.Annotate,
                        LanguageID = enumAnnotate.Lid.ToString()
                    };
                    db.SystemTypeModel.Add(stm);

                    int i = 0;
                    foreach (var v in e.GetEnumValues())
                    {
                        var enumAnnotate1 = EnumAnnotate.Get(v);
                        if (enumAnnotate1 != null)
                        {
                            SystemTypeValueModel stvm = new SystemTypeValueModel
                            {
                                Id = Guid.NewGuid(),
                                Code = (int)v,
                                DisplayValue = enumAnnotate1.Annotate,
                                LanguageID = enumAnnotate1.Lid,
                                CodeGroupID = stm.Id,
                                IsDefault = (i == 0 ? true : false),
                                IsEnable = true,
                                Order = i++,
                                Create_time = DateTime.Now,
                                Create_user = "Sinboda"
                            };
                            db.SystemTypeValueModel.Add(stvm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("初始化枚举出现错误", ex);
            }
        }
    }
}
