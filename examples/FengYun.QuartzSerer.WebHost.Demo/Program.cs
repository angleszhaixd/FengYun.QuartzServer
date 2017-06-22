using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FengYun.QuartzServer.Core;

//$rootnamespace$
namespace FengYun.QuartzSerer.WebHost.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            /// <summary>
            /// 启动windows服务自动配置，泛型参数:自定义任务调度器类型,必须继承自<see cref="BaseHostSchedulerTaskServer"/>
            /// </summary>
            /// <remarks>根据实际情况自行替换MyTestHostServer</remarks>
            HostServiceBootstrap.AutoRun<MyTestHostServer>();
        }
    }
}