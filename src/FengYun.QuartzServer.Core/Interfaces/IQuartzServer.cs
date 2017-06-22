using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FengYun.QuartzServer.Core
{
    /// <summary>
    /// Quartz自动任务接口
    /// Author:zhaixd
    /// date:2016.06.16
    /// </summary>
    public interface IQuartzServer 
    {
        /// <summary>
        /// 获取当前计划任务实例工厂
        /// </summary>
        ISchedulerFactory SchedulerFactory { get; }
        /// <summary>
        /// 返回当前任务调度实例
        /// </summary>
        IScheduler Scheduler { get; }
        /// <summary>
        /// 获取当前Logger实例
        /// </summary>
        ILog Logger { get; }
        /// <summary>
        /// 自动任务实例初始化
        /// </summary>
        /// <param name="autoStart">是否初始化后自动启动服务</param>
        void Initialize(bool autoStart = false);

        /// <summary>
        /// 开始任务
        /// </summary>
        void Start();

        /// <summary>
        /// 停止任务
        /// </summary>
        void Stop();

        /// <summary>
        /// 暂停任务
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复任务
        /// </summary>
        void Resume();

        /// <summary>
        /// 添加自定义任务
        /// </summary>
        /// <typeparam name="T">要添加的任务类型</typeparam>
        /// <param name="jobKey">自定义任务标识(不能重复)</param>
        /// <param name="trigger">触发器</param>
        /// <param name="jobData">传递的参数</param>
        /// <returns></returns>
        DateTimeOffset CreateJob<T>(string jobKey, ITrigger trigger, IDictionary<string, object> jobData = null);

        /// <summary>
        /// 删除计划任务
        /// </summary>
        /// <param name="jobKey">任务标识</param>
        /// <returns></returns>
        bool DeleteJob(string jobKey);
    }
}
