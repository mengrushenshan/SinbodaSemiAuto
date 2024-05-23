using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Control.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class MayBe
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TI"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="input"></param>
        /// <param name="evaluator"></param>
        /// <returns></returns>
        public static TR With<TI, TR>(this TI input, Func<TI, TR> evaluator) where TI : class where TR : class
        {
            if (input == null)
            {
                return default(TR);
            }
            return evaluator(input);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        /// <param name="input"></param>
        /// <param name="evaluator"></param>
        /// <returns></returns>
        public static TR WithString<TR>(this string input, Func<string, TR> evaluator) where TR : class
        {
            if (string.IsNullOrEmpty(input))
            {
                return default(TR);
            }
            return evaluator(input);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TI"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="input"></param>
        /// <param name="evaluator"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static TR Return<TI, TR>(this TI? input, Func<TI?, TR> evaluator, Func<TR> fallback) where TI : struct
        {
            if (input.HasValue)
            {
                return evaluator(new TI?(input.Value));
            }
            if (fallback == null)
            {
                return default(TR);
            }
            return fallback();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TI"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="input"></param>
        /// <param name="evaluator"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static TR Return<TI, TR>(this TI input, Func<TI, TR> evaluator, Func<TR> fallback) where TI : class
        {
            if (input != null)
            {
                return evaluator(input);
            }
            if (fallback == null)
            {
                return default(TR);
            }
            return fallback();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TI"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ReturnSuccess<TI>(this TI input) where TI : class
        {
            return input != null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TI"></typeparam>
        /// <param name="input"></param>
        /// <param name="evaluator"></param>
        /// <returns></returns>
        public static TI If<TI>(this TI input, Func<TI, bool> evaluator) where TI : class
        {
            if (input == null)
            {
                return default(TI);
            }
            if (!evaluator(input))
            {
                return default(TI);
            }
            return input;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TI"></typeparam>
        /// <param name="input"></param>
        /// <param name="evaluator"></param>
        /// <returns></returns>
        public static TI IfNot<TI>(this TI input, Func<TI, bool> evaluator) where TI : class
        {
            if (input == null)
            {
                return default(TI);
            }
            if (!evaluator(input))
            {
                return input;
            }
            return default(TI);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TI"></typeparam>
        /// <param name="input"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TI Do<TI>(this TI input, Action<TI> action) where TI : class
        {
            if (input == null)
            {
                return default(TI);
            }
            action(input);
            return input;
        }
    }
}
