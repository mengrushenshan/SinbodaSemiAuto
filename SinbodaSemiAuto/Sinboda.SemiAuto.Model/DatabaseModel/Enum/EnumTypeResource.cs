using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.Resource
{
    public static class EnumTypeResource
    {
        /// <summary>
        /// 项目类型
        /// </summary>
        public static List<SystemTypeValue<ItemType>> ItemTypeResource { get; private set; }

        /// <summary>
        /// 结果测试类型
        /// </summary>
        public static List<SystemTypeValue<TestResultType>> TestResultTypeResource { get; private set; }

        /// <summary>
        /// 测试状态
        /// </summary>
        public static List<SystemTypeValue<TestState>> TestStateResource { get; } = SystemResources.Instance.GetSystemTypeValueEnum<TestState>(nameof(TestState));

        /// <summary>
        /// 性别
        /// </summary>
        public static List<SystemTypeValue<Sex>> SexResource { get; } = SystemResources.Instance.GetSystemTypeValueEnum<Sex>(nameof(Sex));

        /// <summary>
        /// 年龄单位
        /// </summary>
        public static List<SystemTypeValue<AgeUnit>> AgeUnitResource { get; } = SystemResources.Instance.GetSystemTypeValueEnum<AgeUnit>(nameof(AgeUnit));

        /// <summary>
        /// 校准方法
        /// </summary>
        public static List<SystemTypeValue<Direction>> DirectionSource { get; } = SystemResources.Instance.GetSystemTypeValueEnum<Direction>(nameof(Direction));

        /// <summary>
        /// 校准方法
        /// </summary>
        public static List<SystemTypeValue<Rate>> RateSource { get; } = SystemResources.Instance.GetSystemTypeValueEnum<Rate>(nameof(Rate));
        /// <summary>
        /// 质控/校准状态
        /// </summary>
        public static List<SystemTypeValue<TestState>> CalStatusSource { get; private set; }


        static EnumTypeResource()
        {
            ItemTypeResource = SystemResources.Instance.GetSystemTypeValueEnum<ItemType>(nameof(ItemType));
            CalStatusSource = SystemResources.Instance.GetSystemTypeValueEnum<TestState>(nameof(TestState));
            TestResultTypeResource = SystemResources.Instance.GetSystemTypeValueEnum<TestResultType>(nameof(TestResultType));
        }
    }
}
