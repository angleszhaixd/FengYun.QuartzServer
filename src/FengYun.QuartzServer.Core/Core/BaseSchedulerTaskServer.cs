using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Common.Logging;
using Quartz.Impl;
using System.Collections.Concurrent;

namespace FengYun.QuartzServer.Core
{
    /// <summary>
    /// 计划任务调度实例基础实现
    /// Author:zhaixd
    /// date:2016.01.17
    /// </summary>
    public abstract class BaseSchedulerTaskServer : IQuartzServer,IDisposable
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        protected readonly ILog logger;
        /// <summary>
        /// 当前日志记录器
        /// </summary>
        public ILog Logger
        {
            get { return logger?? GetLogger(); }
        }

        /// <summary>
        /// 计划任务实例工厂
        /// </summary>
        private ISchedulerFactory _schedulerFactory;
        /// <summary>
        /// 返回当前计划任务实例工厂 (创建于 <see cref="Initialize" /> 中的 <see cref="CreateSchedulerFactory" /> 方法).
        /// </summary>
        public ISchedulerFactory SchedulerFactory
        {
            get { return _schedulerFactory; }
        }
        /// <summary>
        /// 计划任务实例
        /// </summary>
        private IScheduler _scheduler;
        /// <summary>
        /// 返回当前任务调度实例 (创建于 <see cref="Initialize" /> 中的 <see cref="GetScheduler" /> 方法).
        /// </summary>
        public IScheduler Scheduler
        {
            get { return _scheduler; }
        }
        /// <summary>
        /// 缓存添加的自定义任务
        /// </summary>
        protected ConcurrentDictionary<string, IJobDetail> jobCaches = new ConcurrentDictionary<string, IJobDetail>();
        public BaseSchedulerTaskServer()
        {
            logger = GetLogger();
        }
        /// <summary>
        /// 获取日志记录器 
        /// </summary>
        /// <returns></returns>
        protected virtual ILog GetLogger()
        {
            return LogManager.GetLogger(GetType());
        }
        /// <summary>
        /// 获取任务调度实例
        /// </summary>
        /// <returns></returns>
        protected virtual IScheduler GetScheduler()
        {
            return _schedulerFactory.GetScheduler();
        }

        /// <summary>
        /// 创建任务调度工厂实例
        /// </summary>
        /// <returns></returns>
        protected virtual ISchedulerFactory CreateSchedulerFactory()
        {
            return new StdSchedulerFactory();
        }

        /// <summary>
        /// 添加计划任务
        /// </summary>
        /// <typeparam name="T">要添加的任务类型</typeparam>
        /// <param name="jobKey">自定义任务标识(不能重复)</param>
        /// <param name="trigger">触发器</param>
        /// <param name="jobData">传递的参数</param>
        /// <returns></returns>
        public virtual DateTimeOffset CreateJob<T>(string jobKey,ITrigger trigger, IDictionary<string, object> jobData = null)
        {
            JobKey identyKey = JobKey.Create(jobKey);
            //string jobKey = string.Empty;
            //var TInstance = Activator.CreateInstance<T>();
            //if (TInstance is BaseSchedulerJob)
            //{
            //    jobKey = (TInstance as BaseSchedulerJob).jobIdentityKey;
            //}
            //else
            //{
            //    jobKey = TInstance.GetType().Name;
            //}
            IJobDetail job = JobBuilder.Create(typeof(T))
                .WithIdentity(identyKey)
                .UsingJobData(new JobDataMap(jobData ?? new Dictionary<string, object>()))
                .Build();
            var result = _scheduler.ScheduleJob(job, trigger);
            //缓存创建的任务
            if (!jobCaches.ContainsKey(jobKey))
            {
                jobCaches.TryAdd(jobKey, job);
            }
            return result;
        }
        /// <summary>
        /// 删除计划任务
        /// </summary>
        /// <param name="jobKey">任务标识</param>
        /// <returns></returns>
        public virtual bool DeleteJob(string jobKey)
        {
            bool delFlag = false;
            JobKey identyKey = JobKey.Create(jobKey);
            if (_scheduler.CheckExists(identyKey))
            {
                delFlag = Scheduler.DeleteJob(identyKey);
                //移除任务缓存
                IJobDetail jobDet;
                jobCaches.TryRemove(jobKey,out jobDet);
                logger.InfoFormat("---删除计划任务:{0},执行结果:{1}", identyKey.ToString(), delFlag);
            }
            return delFlag;
        }
        #region 计划任务接口实现
        /// <summary>
        /// 初始化调用
        /// </summary>
        public virtual void Initialize(bool autoStart = false)
        {
            try
            {
                _schedulerFactory = CreateSchedulerFactory();
                _scheduler = GetScheduler();
                if (autoStart)
                    Start();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("任务调度初始化失败!", ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 开始执行
        /// </summary>
        public virtual void Start()
        {
            try
            {
                //开始前清除所有已存在的任务及触发器
                _scheduler.Clear();
                initScheduleJobs();
                _scheduler.Start();
            }
            catch (Exception ex)
            {
                logger.Fatal(string.Format("任务开始执行失败: {0}", ex.Message), ex);
                throw;
            }

            logger.Info("任务开始执行调用成功!");
        }
        /// <summary>
        /// 开始计划任务前添加自动任务
        /// </summary>
        protected abstract void initScheduleJobs();
        /// <summary>
        /// 停止执行
        /// </summary>
        public virtual void Stop()
        {
            try
            {
                _scheduler.Clear();
                _scheduler.Shutdown(true);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("任务停止失败: {0}", ex.Message), ex);
                throw;
            }

            logger.Info("任务停止调用成功");
        }

        /// <summary>
        /// 暂停执行
        /// </summary>
        public virtual void Pause()
        {
            _scheduler.PauseAll();
            logger.Info("任务暂停调用成功");
        }

        /// <summary>
        /// 恢复任务
        /// </summary>
        public void Resume()
        {
            _scheduler.ResumeAll();
            logger.Info("任务恢复调用成功");
        }
        #endregion

        /// <summary>
        /// 释放或重置非托管资源
        /// </summary>
        public virtual void Dispose()
        {
            
        }
    }
}
