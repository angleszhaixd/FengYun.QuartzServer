using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Common.Logging;
using System.Threading;

namespace FengYun.QuartzServer.Core
{
    /// <summary>
    /// 计划任务 基础类
    /// Author:zhaixd
    /// date:2016.01.17
    /// </summary>
    public abstract class BaseSchedulerJob : IJob
    {
        private static readonly object LockObj = new object();
        /// <summary>
        /// 日志记录器
        /// </summary>
        protected readonly ILog logger;

        public BaseSchedulerJob()
        {
            logger = LogManager.GetLogger(GetType());
        }
        /// <summary>
        /// 自定义任务标识
        /// </summary>
        //public abstract string jobIdentityKey { get; }

        /// <summary>
        /// 计划任务执行逻辑
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            //加锁,避免重复进入
            if (!Monitor.TryEnter(LockObj, 0))
                return;
            try
            {
                ExecuteJob(context);
            }
            catch (Exception Ex)
            {
                logger.FatalFormat("执行计划任务出错:{0}", Ex.Message);
            }
            finally
            {
                Monitor.Exit(LockObj);
            }
        }
        protected abstract void ExecuteJob(IJobExecutionContext context);
    }
}
