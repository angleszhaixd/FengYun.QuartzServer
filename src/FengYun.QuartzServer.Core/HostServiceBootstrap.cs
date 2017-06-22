using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.Logging;
using Topshelf;

namespace FengYun.QuartzServer.Core
{
    /// <summary>
    /// windwos服务启动设置
    /// Author:zhaixd
    /// date:2016.01.17
    /// </summary>
    public static class HostServiceBootstrap
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HostServiceBootstrap));

        /// <summary>
        /// 启动windows服务自动配置
        /// </summary>
        /// <typeparam name="T">自定义任务调度器类型,必须继承自<see cref="BaseHostSchedulerTaskServer"/></typeparam>
        /// <param name="nlogConfigPath">nlog配置文件路径,相对应AppDomain.CurrentDomain.BaseDirectory(eg:config/nlog.config)</param>
        /// <returns></returns>
        public static TopshelfExitCode AutoRun<T>(string nlogConfigPath = null)
        {
            var typeObject = Activator.CreateInstance<T>().GetType();
            if (!typeObject.IsSubclassOf(typeof(BaseHostSchedulerTaskServer)))
            {
                logger.FatalFormat("无效的IHostServer类型:{0},必须继承自:BaseHostSchedulerTaskServer!", typeObject.FullName);
                throw new ArgumentException(string.Format("无效的IHostServer类型:{0},必须继承自:BaseHostSchedulerTaskServer!", typeObject.FullName));
            }
            return AutoRun(typeObject, nlogConfigPath);
        }

        /// <summary>
        /// 启动windows服务自动配置
        /// </summary>
        /// <param name="QuartzServerType">任务调度器实例类型,必须继承自<see cref="BaseHostSchedulerTaskServer"/>,默认读取quartz.host.serverImplementationTypeName</param>
        /// <param name="nlogConfigPath">nlog配置文件路径,相对应AppDomain.CurrentDomain.BaseDirectory(eg:config/nlog.config)</param>
        /// <returns></returns>
        public static TopshelfExitCode AutoRun(Type QuartzServerType = null, string nlogConfigPath = null)
        {
            var baseAppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            Directory.SetCurrentDirectory(baseAppPath);
            var code = HostFactory.Run(x => {
                x.StartAutomatically(); //自启动服务
                x.EnablePauseAndContinue(); //服务可以暂停和继续
                x.EnableShutdown();  //服务可以关闭
                //NLog配置
                if (!string.IsNullOrWhiteSpace(nlogConfigPath))
                {
                    var nlogFile = Path.Combine(baseAppPath, nlogConfigPath.Replace("/", "\\"));
                    x.UseNLog(new NLog.LogFactory(new NLog.Config.XmlLoggingConfiguration(nlogConfigPath, true))); 
                }
                else
                {
                    x.UseNLog();  
                }
                x.BeforeUninstall(() =>
                {
                    ////卸载之前执行
                    //if (MessageBox.Show("服务卸载后数据会出错,请知悉!", "服务卸载", MessageBoxButtons.OK) == DialogResult.OK)
                    //{ 

                    //}
                    //string M = Interaction.InputBox("请输入卸载密码", "输入", "", 100, 100);
                    //if (M != "fyxmt")
                    //{
                    //    MessageBox.Show("密码输入错误！");
                    //    logger.Error("服务卸载失败,密码出错!");
                    //    throw new Exception("服务卸载失败,密码出错!");
                    //}
                });
                x.AfterUninstall(() =>
                {
                    //卸载后记录日志
                    logger.Debug("服务卸载完成!");
                });
                x.RunAsLocalSystem();

                x.SetDescription(HostServiceConfigurationLoader.HostServiceCfg.serviceDescription);
                x.SetDisplayName(HostServiceConfigurationLoader.HostServiceCfg.serviceDisplayName);
                x.SetServiceName(HostServiceConfigurationLoader.HostServiceCfg.serviceName);

                x.Service<IHostServer>(sc =>
                {

                    sc.ConstructUsing(() =>
                    {
                        var typeName = QuartzServerType == null ? null : QuartzServerType.FullName;
                        IHostServer server = SchedulerTaskFactory.CreateHostSchedulerServer(typeName);
                        server.Initialize();
                        return server;
                    });
                    sc.WhenStarted((s, hostControl) => s.Start(hostControl));
                    sc.WhenStopped((s, hostControl) => s.Stop(hostControl));
                    sc.WhenPaused((s, hostControl) => s.Pause(hostControl));
                    sc.WhenContinued((s, hostControl) => s.Continue(hostControl));
                });

            });
            return code;
        }
    }
}
