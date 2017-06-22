using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Common.Logging;
using Quartz.Impl;
using System.Collections.Concurrent;
using Topshelf;

namespace FengYun.QuartzServer.Core
{
    /// <summary>
    /// windows服务 计划任务调度实例基础实现
    /// Author:zhaixd
    /// date:2016.01.17
    /// </summary>
    public abstract class BaseHostSchedulerTaskServer : BaseSchedulerTaskServer, IHostServer,IDisposable
    {

        #region topself服务接口实现
        /// <summary>
        /// windwos服务启动 委托于 <see cref="Start()"/>.
        /// </summary>
        public bool Start(HostControl hostControl)
        {
            Start();
            logger.Info("windwos服务启动成功");
            return true;
        }

        /// <summary>
        /// windwos服务停止 委托于 <see cref="Stop()"/>.
        /// </summary>
        public bool Stop(HostControl hostControl)
        {
            Stop();
            logger.Info("windwos服务停止成功");
            return true;
        }

        /// <summary>
        /// windwos服务暂停 委托于 <see cref="Pause()"/>.
        /// </summary>
        public bool Pause(HostControl hostControl)
        {
            Pause();
            logger.Info("windwos服务暂停成功");
            return true;
        }

        /// <summary>
        /// windwos服务继续 委托于 <see cref="Resume()"/>.
        /// </summary>
        public bool Continue(HostControl hostControl)
        {
            Resume();
            logger.Info("windwos服务恢复成功");
            return true;
        }
        #endregion
    }
}
