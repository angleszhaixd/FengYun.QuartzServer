using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FengYun.QuartzServer.Core;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(WebHost.WebForm.Test.QuartzServerConfig), "Register")]
namespace WebHost.WebForm.Test
{
    /// <summary>
    /// 应用程序启动时自动注册默认的计划任务调度器
    /// </summary>
    public class QuartzServerConfig
    {
        public static void Register()
        {
            /// <summary>
            /// 注册计划任务调度器，泛型参数:自定义任务调度器类型,必须继承自<see cref="FengYun.QuartzServer.Core.BaseSchedulerTaskServer"/>
            /// </summary>
            /// <remarks>根据实际情况自行替换MyTestServer</remarks>
            SchedulerTaskFactory.CreateSchedulerServer<MyTestServer>(true);
            //获取已缓存的任务调度器实例
            //var ServerInstance = SchedulerTaskFactory.GetSchedulerServer<MyTestServer>();
        }
    }
}