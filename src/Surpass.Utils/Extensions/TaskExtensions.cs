using Surpass.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    /// <summary>
    /// 任务扩展
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// 延迟启动
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="delay">延时时间</param>
        public static void DelayStart(this Task task, TimeSpan delay)
        {
            DelayStart(task, delay, TaskScheduler.Current ?? TaskScheduler.Default);
        }

        /// <summary>
        /// 延迟启动
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="millisecondsDelay">延时毫秒</param>
        public static void DelayStart(this Task task, int millisecondsDelay)
        {
            DelayStart(task, millisecondsDelay, TaskScheduler.Current ?? TaskScheduler.Default);
        }

        /// <summary>
        /// 延迟启动
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="delay">延时时间</param>
        /// <param name="taskScheduler">调度对象</param>
        public static void DelayStart(this Task task, TimeSpan delay, TaskScheduler taskScheduler)
        {
            DelayStart(task, delay, taskScheduler, new CancellationToken());
        }

        /// <summary>
        /// 延迟启动
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="millisecondsDelay">延时毫秒</param>
        /// <param name="taskScheduler">调度对象</param>
        public static void DelayStart(this Task task, int millisecondsDelay, TaskScheduler taskScheduler)
        {
            DelayStart(task, millisecondsDelay, taskScheduler, new CancellationToken());
        }

        /// <summary>
        /// 延迟启动
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="delay">延时时间</param>
        /// <param name="taskScheduler">调度对象</param>
        /// <param name="cancellationToken">传播取消标记</param>
        public static void DelayStart(this Task task, TimeSpan delay, TaskScheduler taskScheduler,
            CancellationToken cancellationToken)
        {
            long totalMilliseconds = (long)delay.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException(nameof(task), nameof(task) + " 为负数或大于最大值。");
            }
            DelayStart(task, (int)totalMilliseconds, taskScheduler, cancellationToken);
        }

        /// <summary>
        /// 延迟启动
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="millisecondsDelay">延时毫秒</param>
        /// <param name="taskScheduler">调度对象</param>
        /// <param name="cancellationToken">传播取消标记</param>
        public static void DelayStart(this Task task, int millisecondsDelay, TaskScheduler taskScheduler,
            CancellationToken cancellationToken)
        {            
            ExceptionUtils.CheckNotNull(task, nameof(task));
            ExceptionUtils.CheckNotNull(taskScheduler, nameof(taskScheduler));
            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), nameof(millisecondsDelay) + " 小于-1。");
            }
            if (task.Status != TaskStatus.Created)
            {
                throw new InvalidOperationException("已开始的任务不能再开始");
            }
            Task delayTask = new Task(() =>
            {
                Thread.Sleep(millisecondsDelay);
                task.Start(taskScheduler);
            }, cancellationToken);
            delayTask.Start(taskScheduler);
        }
    }
}
