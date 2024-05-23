using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.ModelsOperation
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemManagementModelOperations
    {
        /// <summary>
        /// 获取基础信息类型列表
        /// </summary>
        /// <returns></returns>
        public List<SystemTypeModel> GetSysDataDictionaryTypeList()
        {
            List<SystemTypeModel> t = new List<SystemTypeModel>();
            using (SystemValueContext db = new SystemValueContext())
            {
                t = db.SystemTypeModel.ToList();
            }
            return t;
        }
        /// <summary>
        /// 获取系统基础信息信息列表
        /// </summary>
        /// <returns></returns>
        public List<SystemTypeValueModel> GetSysDataDictionaryInfoList(Guid codeGroup)
        {
            List<SystemTypeValueModel> t = new List<SystemTypeValueModel>();
            using (SystemValueContext db = new SystemValueContext())
            {
                t = db.SystemTypeValueModel.Where(p => p.CodeGroupID == codeGroup).ToList();
            }
            return t;
        }

        /// <summary>
        /// 获取联机模块类型集合
        /// </summary>
        /// <returns></returns>
        public List<ModuleTypeModel> GetModuleTypeList()
        {
            List<ModuleTypeModel> t = new List<ModuleTypeModel>();
            using (DBContextBase db = new DBContextBase())
            {
                t = db.ModuleTypeModel.ToList();
            }
            return t;
        }
        /// <summary>
        /// 获取联机模块信息集合
        /// </summary>
        /// <returns></returns>
        public List<ModuleInfoModel> GetModuleInfoList(ModuleTypeModel moduleType)
        {
            List<ModuleInfoModel> t = new List<ModuleInfoModel>();
            using (DBContextBase db = new DBContextBase())
            {
                t = db.ModuleInfoModel.Where(o => o.ModuleType == moduleType.Id).ToList();
            }
            foreach (var item in t)
            {
                item.ModuleTypeName = moduleType.ModuleTypeName;
            }
            return t;
        }
        /// <summary>
        /// 获取基础信息类型列表
        /// </summary>
        /// <returns></returns>
        public List<DataDictionaryTypeModel> GetDataDictionaryTypeList()
        {
            List<DataDictionaryTypeModel> t = new List<DataDictionaryTypeModel>();
            using (DBContextBase db = new DBContextBase())
            {
                t = db.DataDictionaryTypeModel.ToList();
                foreach (var item in t)
                    item.TypeValues = item.LanguageID < SystemResources.Instance.LanguageArray.Length ? SystemResources.Instance.LanguageArray[item.LanguageID] : item.TypeValues;
            }
            return t;
        }
        /// <summary>
        /// 获取基础信息信息列表
        /// </summary>
        /// <returns></returns>
        public List<DataDictionaryInfoModel> GetDataDictionaryInfoList(Guid codeGroup)
        {
            List<DataDictionaryInfoModel> t = new List<DataDictionaryInfoModel>();
            using (DBContextBase db = new DBContextBase())
            {
                t = db.DataDictionaryInfoModel.Where(p => p.CodeGroupID == codeGroup).ToList();
            }
            return t;
        }

        /// <summary>
        /// 获取子节点信息
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<DataDictionaryInfoModel> GetChildDataDictionaryInfoList(Guid parentId)
        {
            List<DataDictionaryInfoModel> t = new List<DataDictionaryInfoModel>();
            using (DBContextBase db = new DBContextBase())
            {
                t = db.DataDictionaryInfoModel.Where(p => p.ParentCode == parentId).ToList();
            }
            return t;
        }

        /// <summary>
        /// 增删改操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="enums"></param>
        /// <returns></returns>
        public bool OperateT<T>(T t, OperationEnum enums) where T : EntityModelBase
        {
            int result = 0;
            using (DBContextBase _dbTmp = new DBContextBase())
            {
                switch (enums)
                {
                    case OperationEnum.Add:
                        t.Id = Guid.NewGuid();
                        t.Create_time = DateTime.Now;
                        t.Create_user = SystemResources.Instance.CurrentUserName;
                        _dbTmp.Set<T>().Add(t);
                        result = _dbTmp.SaveChanges();
                        break;
                    case OperationEnum.Modify:
                        DbEntityEntry entryModify = _dbTmp.Entry<T>(t);
                        entryModify.State = EntityState.Modified;
                        t.Create_time = DateTime.Now;
                        t.Create_user = SystemResources.Instance.CurrentUserName;
                        result = _dbTmp.SaveChanges();
                        break;
                    case OperationEnum.Delete:
                        DbEntityEntry entryDelete = _dbTmp.Entry<T>(t);
                        entryDelete.State = System.Data.Entity.EntityState.Deleted;
                        result = _dbTmp.SaveChanges();
                        break;
                }
            }
            if (result > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 增删改操作(sqlite SystemValue.db)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="enums"></param>
        /// <returns></returns>
        public bool OperateT_Sqlite<T>(T t, OperationEnum enums) where T : EntityModelBase
        {
            int result = 0;
            using (SystemValueContext _dbTmp = new SystemValueContext())
            {
                switch (enums)
                {
                    case OperationEnum.Add:
                        t.Id = Guid.NewGuid();
                        t.Create_time = DateTime.Now;
                        t.Create_user = SystemResources.Instance.CurrentUserName;
                        _dbTmp.Set<T>().Add(t);
                        result = _dbTmp.SaveChanges();
                        break;
                    case OperationEnum.Modify:
                        DbEntityEntry entryModify = _dbTmp.Entry<T>(t);
                        entryModify.State = EntityState.Modified;
                        t.Create_time = DateTime.Now;
                        t.Create_user = SystemResources.Instance.CurrentUserName;
                        result = _dbTmp.SaveChanges();
                        break;
                    case OperationEnum.Delete:
                        DbEntityEntry entryDelete = _dbTmp.Entry<T>(t);
                        entryDelete.State = System.Data.Entity.EntityState.Deleted;
                        result = _dbTmp.SaveChanges();
                        break;
                }
            }
            if (result > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 业务字典删除子表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool OperateDataDicTypeInfos(List<DataDictionaryInfoModel> list)
        {
            int result = 0;
            using (DBContextBase _dbTmp = new DBContextBase())
            {
                foreach (var item in list)
                {
                    DbEntityEntry entryDelete = _dbTmp.Entry(item);
                    entryDelete.State = System.Data.Entity.EntityState.Deleted;
                }
                result = _dbTmp.SaveChanges();
            }
            if (result > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 系统字典删除子表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool OperateSysDataDicTypeInfos(List<SystemTypeValueModel> list)
        {
            int result = 0;
            using (DBContextBase _dbTmp = new DBContextBase())
            {
                foreach (var item in list)
                {
                    DbEntityEntry entryDelete = _dbTmp.Entry(item);
                    entryDelete.State = System.Data.Entity.EntityState.Deleted;
                }
                result = _dbTmp.SaveChanges();
            }
            if (result > 0)
                return true;
            else
                return false;
        }

        public bool OperateSysDataDicTypeInfos_Sqlite(List<SystemTypeValueModel> list)
        {
            int result = 0;
            using (SystemValueContext _dbTmp = new SystemValueContext())
            {
                foreach (var item in list)
                {
                    DbEntityEntry entryDelete = _dbTmp.Entry(item);
                    entryDelete.State = System.Data.Entity.EntityState.Deleted;
                }
                result = _dbTmp.SaveChanges();
            }
            if (result > 0)
                return true;
            else
                return false;
        }


        /// <summary>
        /// 交换两个<seealso cref="Items.Order"/>字段值
        /// </summary>
        /// <param name="item1">第一个<seealso cref="Items"/>实例</param>
        /// <param name="item2">第二个<seealso cref="Items"/>实例</param>
        /// <returns></returns>
        public void ExchangeOrder(DataDictionaryInfoModel one, DataDictionaryInfoModel two)
        {
            using (var db = new DBContextBase())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    int oldOrder = one.ShowOrder;
                    one.ShowOrder = two.ShowOrder;
                    two.ShowOrder = oldOrder;

                    if (db.Entry(one).State == EntityState.Detached)
                        db.Set<DataDictionaryInfoModel>().Attach(one);
                    one.Create_user = SystemResources.Instance.CurrentUserName;
                    db.Entry(one).State = EntityState.Modified;

                    if (db.Entry(two).State == EntityState.Detached)
                        db.Set<DataDictionaryInfoModel>().Attach(two);
                    two.Create_user = SystemResources.Instance.CurrentUserName;
                    db.Entry(two).State = EntityState.Modified;

                    db.SaveChanges();

                    transaction.Commit();
                }
            }
        }
        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="one"></param>
        public void ExchangeOrderTop(DataDictionaryInfoModel one)
        {
            using (var db = new DBContextBase())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var others = db.Set<DataDictionaryInfoModel>().Where(o => o.Id != one.Id && o.CodeGroupID == one.CodeGroupID).OrderBy(o => o.ShowOrder).ToList();
                    int showOrder = 2;
                    one.ShowOrder = 1;
                    if (db.Entry(one).State == EntityState.Detached)
                        db.Set<DataDictionaryInfoModel>().Attach(one);
                    one.Create_user = SystemResources.Instance.CurrentUserName;
                    db.Entry(one).State = EntityState.Modified;

                    foreach (var item in others)
                    {
                        item.ShowOrder = showOrder;
                        if (db.Entry(item).State == EntityState.Detached)
                            db.Set<DataDictionaryInfoModel>().Attach(item);
                        item.Create_user = SystemResources.Instance.CurrentUserName;
                        db.Entry(item).State = EntityState.Modified;
                        showOrder++;
                    }

                    db.SaveChanges();

                    transaction.Commit();
                }
            }
        }
        /// <summary>
        /// 置底
        /// </summary>
        /// <param name="one"></param>
        public void ExchangeOrderBottom(DataDictionaryInfoModel one)
        {
            using (var db = new DBContextBase())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var others = db.Set<DataDictionaryInfoModel>().Where(o => o.Id != one.Id && o.CodeGroupID == one.CodeGroupID).OrderBy(o => o.ShowOrder).ToList();
                    int showOrder = 1;
                    one.ShowOrder = others.Count + 1;
                    if (db.Entry(one).State == EntityState.Detached)
                        db.Set<DataDictionaryInfoModel>().Attach(one);
                    one.Create_user = SystemResources.Instance.CurrentUserName;
                    db.Entry(one).State = EntityState.Modified;

                    foreach (var item in others)
                    {
                        item.ShowOrder = showOrder;
                        if (db.Entry(item).State == EntityState.Detached)
                            db.Set<DataDictionaryInfoModel>().Attach(item);
                        item.Create_user = SystemResources.Instance.CurrentUserName;
                        db.Entry(item).State = EntityState.Modified;
                        showOrder++;
                    }

                    db.SaveChanges();

                    transaction.Commit();
                }
            }
        }
        /// <summary>
        /// 删除后所有列表里的show order都要重新赋值
        /// </summary>
        /// <param name="others">被动移动项</param>
        /// <returns></returns>
        public void ExchangeDeleteOrder(int showOrder, List<DataDictionaryInfoModel> others)
        {
            using (var db = new DBContextBase())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    foreach (var item in others)
                    {
                        item.ShowOrder = showOrder;
                        if (db.Entry(item).State == EntityState.Detached)
                            db.Set<DataDictionaryInfoModel>().Attach(item);
                        item.Create_user = SystemResources.Instance.CurrentUserName;
                        db.Entry(item).State = EntityState.Modified;
                        showOrder++;
                    }
                    db.SaveChanges();
                    transaction.Commit();
                }
            }
        }
    }
    /// <summary>
    /// 数据操作枚举
    /// </summary>
    public enum OperationEnum
    {
        /// <summary>
        /// 添加
        /// </summary>
        Add,
        /// <summary>
        /// 修改
        /// </summary>
        Modify,
        /// <summary>
        /// 删除
        /// </summary>
        Delete
    }
}
