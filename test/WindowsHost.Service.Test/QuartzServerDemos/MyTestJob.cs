using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FengYun.QuartzServer.Core;
using Quartz;

namespace WindowsHost.Service.Test
{
    /// <summary>
    /// 测试任务详情，具体要执行的任务逻辑
    /// </summary>
    public class MyTestJob : BaseSchedulerJob
    {
        /// <summary>
        /// 自定义任务标识
        /// </summary>
        public const string jobIdentityKey = "MyTestJob1";
        /// <summary>
        /// 传递的参数(自动映射)
        /// </summary>
        public string JobSays { private get; set; }

        /// <summary>
        /// 传递的参数(自动映射)
        /// </summary>
        public int myValue { private get; set; }

        protected override void ExecuteJob(IJobExecutionContext context)
        {
            try
            {
                var jobData = context.JobDetail.JobDataMap;
                string msg = string.Format("开始执行测试任务111111111===>{0}", DateTime.Now.ToLongDateString());
                Console.WriteLine(msg);
                logger.Info(msg);
            }
            catch (Exception Ex)
            {
                logger.FatalFormat("执行测试任务出错:{0}", Ex.Message);
            }
        }
    }
}
