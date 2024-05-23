using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Utils
{
    /// <summary>
    /// 异常信息转换
    /// </summary>
    public static class ExceptionsConverter
    {
        /// <summary>
        /// 返回平台的异常信息格式
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ToStringEx(this Exception ex)
        {
            if (ex is DbEntityValidationException)
                return Convert(ex as DbEntityValidationException);
            else
                return Convert(ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static string Convert(Exception exception)
        {
            Exception originalException = exception;
            while (originalException.InnerException != null)
            {
                originalException = originalException.InnerException;
            }
            return originalException.Message;
        }

        /// <summary>
        /// EF 实体验证错误
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static string Convert(DbEntityValidationException exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (DbEntityValidationResult validationResult in exception.EntityValidationErrors)
            {
                foreach (DbValidationError error in validationResult.ValidationErrors)
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.AppendLine();
                    stringBuilder.Append(error.PropertyName + ": " + error.ErrorMessage);
                }
            }
            return stringBuilder.ToString();
        }
    }
}
