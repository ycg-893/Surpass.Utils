using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surpass.Utils
{
    /// <summary>
    /// 任务帮助
    /// </summary>
    public static class TaskUtils
    {
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="action">委托</param>
        /// <returns></returns>
        public static Task Run(Action action)
        {
            ExceptionUtils.CheckNotNull(action, nameof(action));
            return Task.Factory.StartNew(action);
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="action">委托</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns></returns>
        public static Task Run(Action action, CancellationToken cancellationToken)
        {
            ExceptionUtils.CheckNotNull(action, nameof(action));
            return Task.Factory.StartNew(action, cancellationToken);
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="function">函数</param>
        /// <returns></returns>
        public static Task Run(Func<Task> function)
        {
            ExceptionUtils.CheckNotNull(function, nameof(function));
            return Task<Task>.Factory.StartNew(function);
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="function">函数</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns></returns>
        public static Task Run(Func<Task> function, CancellationToken cancellationToken)
        {
            ExceptionUtils.CheckNotNull(function, nameof(function));
            return Task<Task>.Factory.StartNew(function, cancellationToken);
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="function">函数</param>
        /// <returns></returns>
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            ExceptionUtils.CheckNotNull(function, nameof(function));
            return Task<TResult>.Factory.StartNew(function);
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="function">函数</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns></returns>
        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            ExceptionUtils.CheckNotNull(function, nameof(function));
            return Task<TResult>.Factory.StartNew(function, cancellationToken);
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="function">函数</param>
        /// <returns></returns>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
        {
            ExceptionUtils.CheckNotNull(function, nameof(function));
            var tssk = function();
            if (tssk == null)
            {
                ExceptionUtils.CheckNotNull(tssk, nameof(tssk));
            }
            tssk.Start();
            return tssk;
        }
    }
}
