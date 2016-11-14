using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    /// <summary>
    /// 采用先进先除法的任务队列池
    /// </summary>
    public class TaskSchedulerQueue : TaskScheduler
    {

        private readonly LinkedList<Task> tasks;

        /// <summary>
        /// 正在运行的数量
        /// </summary>
        private long runningTaskCount;

        /// <summary>
        /// 最大的活动任数量
        /// </summary>
        public int MaxActivityTaskCount { get; private set; }

        /// <summary>
        /// 获取运行的任务数
        /// </summary>
        public int RunningTaskCount
        {
            get
            {
                return (int)Interlocked.Read(ref runningTaskCount);
            }
        }

        /// <summary>
        /// 获取正在排队的任务数
        /// </summary>
        public int QueueTaskCount
        {
            get
            {
                lock (tasks)
                {
                    return tasks.Count;
                }
            }
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="maxActivityTaskCount"></param>
        public TaskSchedulerQueue(int maxActivityTaskCount)
        {
            if (maxActivityTaskCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxActivityTaskCount), nameof(maxActivityTaskCount) + " 小于1 。");
            }
            tasks = new LinkedList<Task>();
            MaxActivityTaskCount = maxActivityTaskCount;
            Interlocked.Exchange(ref runningTaskCount, 0);
        }

        /// <summary>
        /// 排队
        /// </summary>
        /// <param name="task"></param>
        protected override void QueueTask(Task task)
        {
            lock (tasks)
            {
                tasks.AddLast(task);
            }
            NotifyThreadPoolWork();
        }

        /// <summary>
        /// 通知线程池工作
        /// </summary>
        private void NotifyThreadPoolWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(o =>
            {
                List<Task> items = null;
                if (Interlocked.Read(ref runningTaskCount) >= MaxActivityTaskCount)
                {
                    return;
                }
                lock (tasks)
                {
                    if (tasks.Count == 0)
                    {
                        return;
                    }
                    while (tasks.Count > 0 && Interlocked.Read(ref runningTaskCount) < MaxActivityTaskCount)
                    {
                        if (items == null)
                        {
                            items = new List<Task>();
                        }
                        items.Add(tasks.First.Value);
                        tasks.RemoveFirst();
                        Interlocked.Increment(ref runningTaskCount);
                    }
                }
                RunTasks(items);
            }, null);
        }

        /// <summary>
        /// 运行任务集合
        /// </summary>
        /// <param name="items"></param>
        private void RunTasks(List<Task> items)
        {
            if (items != null
                && items.Count > 0)
            {
                foreach (var item in items)
                {
                    RunTask(item);
                }
                items.Clear();
            }
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="task"></param>
        private void RunTask(Task task)
        {
            ThreadPool.UnsafeQueueUserWorkItem(o =>
            {
                try
                {
                    base.TryExecuteTask(task);
                }
                finally
                {
                    Interlocked.Decrement(ref runningTaskCount);
                    NotifyThreadPoolWork();
                }
            }, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="taskWasPreviouslyQueued"></param>
        /// <returns></returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (taskWasPreviouslyQueued)
            {
                TryDequeue(task);
            }
            lock (tasks)
            {
                bool isRemove = tasks.Remove(task);
                try
                {
                    return base.TryExecuteTask(task);
                }
                finally
                {
                    if (isRemove)
                    {
                        Interlocked.Decrement(ref runningTaskCount);
                    }
                }
            }
        }

        /// <summary>
        /// 枚举
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(tasks, ref lockTaken);
                if (lockTaken)
                {
                    return tasks.ToArray();
                }
                else
                {
                    throw new NotSupportedException("无法取得锁。");
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(tasks);
                }
            }

        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="task">任务</param>
        /// <returns></returns>
        protected sealed override bool TryDequeue(Task task)
        {
            lock (tasks)
            {
                return tasks.Remove(task);
            }
        }

    }
}
