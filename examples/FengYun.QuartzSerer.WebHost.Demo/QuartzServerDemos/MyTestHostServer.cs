using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using FengYun.QuartzServer.Core;
using Quartz;
//$rootnamespace$
namespace FengYun.QuartzSerer.WebHost.Demo
{
    /// <summary>
    /// 测试系统服务计划任务调度器(一般用于windows服务)
    /// </summary>
    public class MyTestHostServer : BaseHostSchedulerTaskServer
    {
        public MyTestHostServer()
        {

        }
        /// <summary>
        /// 添加自动任务
        /// </summary>
        protected override void initScheduleJobs()
        {
            //1、程序启动时需要立即添加的自动任务，可以在此方法中添加；
            //2、程序启动后需要人为触发的任务(比如点击触发某个事件)，可以调用SchedulerTaskFactory.GetSchedulerServer<TServer>().CreateJob<TJob>()方法进行添加任务
            //3、任务触发器定义可以参考quartz官网，simpletrigger：https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/simpletriggers.html，
            //crontrigger：https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/crontriggers.html。

            CreateJob<MyTestJob>(MyTestJob.jobIdentityKey, 
                TriggerBuilder.Create().StartNow().WithSimpleSchedule(x => x.WithIntervalInSeconds(5).WithRepeatCount(4)).Build(),
                new Dictionary<string, object> { { "JobSays", "传递的字符串变量值"}, { "myValue", 222 } });
        }
    }
}
