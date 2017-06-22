using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;


namespace FengYun.QuartzServer.Core
{
    /// <summary>
    /// Quartz windows服务承载配置文件加载器
    /// Author:zhaixd
    /// date:2016.01.17
    /// </summary>
    public class HostServiceConfigurationLoader
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HostServiceConfigurationLoader));
        private readonly static HostServiceConfiguration _hostServiceCfg;
        /// <summary>
        /// windows服务承载配置文件
        /// </summary>
        public static HostServiceConfiguration HostServiceCfg
        {
            get { return _hostServiceCfg; }
        }

        static HostServiceConfigurationLoader()
        {
            var config = ConfigurationManager.GetSection(HostServiceConfiguration.ConfigurationSectionName) as HostServiceConfiguration;
            VerfiyHostServiceConifurationError(config);
            _hostServiceCfg = config;
        }

        /// <summary>
        /// 检查配置节点是否缺失
        /// </summary>
        /// <param name="config"></param>
        private static void VerfiyHostServiceConifurationError(HostServiceConfiguration config)
        {
            if (config == null)
            {
                logger.FatalFormat("{0} configSections.section缺失！", HostServiceConfiguration.ConfigurationSectionName);
                throw new ConfigurationErrorsException(string.Format("{0} configSections.section缺失！", HostServiceConfiguration.ConfigurationSectionName));
            }
            if (string.IsNullOrEmpty(config.serverImplementationTypeName))
            {
                logger.Fatal("windows服务默认承载的计划任务调度类型名称 未设置,serverImplementationTypeName");
                throw new ConfigurationErrorsException("windows服务默认承载的计划任务调度类型名称 未设置,serverImplementationTypeName");
            }
        }
    }
}
