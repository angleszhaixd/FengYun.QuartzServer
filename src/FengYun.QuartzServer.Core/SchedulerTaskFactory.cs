using Common.Logging;
using Quartz.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FengYun.QuartzServer.Core
{
    /// <summary>
    /// 计划任务调度工厂类
    /// Author:zhaixd
    /// date:2016.01.17
    /// </summary>
    public class SchedulerTaskFactory
    {
        private static readonly string loadAllAssemblyFlag = ConfigurationManager.AppSettings["LoadDomainAssemblys"];
        private static readonly object Locker = new object();
        private static readonly ILog logger = LogManager.GetLogger(typeof(SchedulerTaskFactory));
        private static ConcurrentDictionary<string, Type> _quartzServiceTypes = new ConcurrentDictionary<string, Type>();
        private static ConcurrentDictionary<string, IQuartzServer> _quartzServerInstances = new ConcurrentDictionary<string, IQuartzServer>();
        
        static SchedulerTaskFactory()
        {
            LoadAssemblyForQuartzServer();
        }

        /// <summary>
        /// 初始化缓存所有的IQuartzServer实现
        /// </summary>
        private static void LoadAssemblyForQuartzServer()
        {
            List<Assembly> asms = new List<Assembly>();
            //是否加载当前应用程序域下的所有程序集，默认仅加载默认应用程序域中的进程可执行文件
            bool loadDomainAssemblys = !string.IsNullOrEmpty(loadAllAssemblyFlag) && loadAllAssemblyFlag.ToString().Trim() == "true";
            if (!loadDomainAssemblys)
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                if(entryAssembly!=null)
                    asms.Add(entryAssembly);
            }
            //web应用程序无法通过Assembly.GetEntryAssembly()获取启动程序集
            if (asms.Count<=0)
            {
                asms.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            }
            List<Type> typeList = new List<Type>();
            asms.ForEach(assembly =>
            {
                var assemblyName = assembly.FullName;
                //var ok = typeof(IQuartzServer).UnderlyingSystemType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract;
                var types = from type in assembly.GetTypes()
                            where type.GetInterfaces().Contains(typeof(IQuartzServer)) && type.IsClass && !type.IsAbstract
                            select type;
                typeList.AddRange(types);
            });
            typeList.Sort(new TypeNameComparer());
            typeList.ForEach(t =>
            {
                string typeFullName = t.FullName; // string.Format("{0}.{1}", t.Namespace, t.Name);
                logger.InfoFormat("缓存调度任务类型(继承接口IQuartzServer):{0}", t.FullName);
                if (!_quartzServiceTypes.ContainsKey(typeFullName))
                    _quartzServiceTypes.TryAdd(typeFullName, t);
            });
        }

        /// <summary>
        /// 创建windows服务计划任务实例
        /// </summary>
        /// <param name="typeFullName">计划任务调度类型名称(GetType().FullName),默认读取quartz.host.serverImplementationTypeName</param>
        /// <returns></returns>
        public static IHostServer CreateHostSchedulerServer(string typeFullName = null)
        {
            IHostServer serverInstance = null;
            try
            {
                string typeName = typeFullName ?? HostServiceConfigurationLoader.HostServiceCfg.serverImplementationTypeName;
                //1、创建windows服务计划任务实例
                if (!_quartzServiceTypes.ContainsKey(typeName))
                {
                    lock (Locker)
                    {
                        if (!_quartzServiceTypes.ContainsKey(typeName))
                        {
                            LoadAssemblyForQuartzServer();
                        }
                    }
                }
                logger.Debug(String.Format("==>创建windows服务计划任务实例：'{0}',时间:{1}", typeName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                Type TObject = _quartzServiceTypes[typeName];
                if (!typeof(IHostServer).IsAssignableFrom(TObject))
                {
                    logger.FatalFormat("无效的IHostServer类型:{0},此类型必须继承自:BaseHostSchedulerTaskServer!", TObject.FullName);
                    throw new ArgumentException(string.Format("无效的IHostServer类型:{0},此类型必须继承自:BaseHostSchedulerTaskServer!", TObject.FullName));
                }
                serverInstance = ObjectUtils.InstantiateType<IHostServer>(TObject);   //(IQuartzServer)Activator.CreateInstance(t);
            }
            catch (Exception Ex)
            {
                logger.FatalFormat("==>windows服务计划任务实例创建出错:{0}", Ex.Message);
            }
            return serverInstance;
        }

        /// <summary>
        /// 创建新的任务调度实例
        /// </summary>
        /// <typeparam name="T">计划任务调度类型,必须派生自<see cref="BaseSchedulerTaskServer"/> </typeparam>
        /// <param name="autoStart">是否自动初始化并启动任务实例</param>
        /// <returns></returns>
        public static IQuartzServer CreateSchedulerServer<T>(bool autoStart = false)
        {
            var typeObject = Activator.CreateInstance<T>().GetType();
            string typeName = string.Empty;
            if (typeObject.IsSubclassOf(typeof(BaseSchedulerTaskServer)))
            {
                typeName = typeObject.FullName;
            }
            if (string.IsNullOrEmpty(typeName)) {
                logger.FatalFormat("无效的IQuartzServer类型:{0}", typeObject.FullName);
                throw new ArgumentException(string.Format("无效的IQuartzServer类型:{0}", typeObject.FullName));
            }
            return CreateSchedulerServer(typeName, autoStart);
        }

        /// <summary>
        /// 创建新的任务调度实例
        /// </summary>
        /// <param name="typeFullName">计划任务调度类型名称(GetType().FullName)</param>
        /// <param name="autoStart">是否自动初始化并启动任务实例</param>
        /// <returns></returns>
        public static IQuartzServer CreateSchedulerServer(string typeFullName,bool autoStart = false)
        {
            IQuartzServer serverInstance = null;
            try
            {
                //1、先从缓存中读取同名的任务调度实例
                if (string.IsNullOrWhiteSpace(typeFullName))
                {
                    logger.FatalFormat("==>计划任务调度类型名称不能为空：'{0}'", typeFullName);
                    throw new ArgumentNullException("CreateSchedulerServer.typeFullName 参数不能为空!");
                }
                //typeFullName = typeFullName ?? HostServiceConfigurationLoader.HostServiceCfg.serverImplementationTypeName;
                if (_quartzServerInstances.ContainsKey(typeFullName) && _quartzServerInstances[typeFullName] != null) {
                    serverInstance =  _quartzServerInstances[typeFullName];
                    return serverInstance;
                }
                //2、缓存不存在则重新创建新的任务调度实例
                if (!_quartzServiceTypes.ContainsKey(typeFullName))
                {
                    lock (Locker)
                    {
                        if (!_quartzServiceTypes.ContainsKey(typeFullName))
                        {
                            LoadAssemblyForQuartzServer();
                        }
                    }
                }
                logger.Debug(String.Format("==>创建新的任务服务实例：'{0}',时间:{1}", typeFullName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                Type t = _quartzServiceTypes[typeFullName];
                serverInstance = ObjectUtils.InstantiateType<IQuartzServer>(t);   //(IQuartzServer)Activator.CreateInstance(t);
                if (autoStart)
                {
                    serverInstance.Initialize(true);
                    logger.Debug(String.Format("==>任务服务实例初始化成功! 时间:{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                }
                //3、缓存创建的任务调度实例
                if (serverInstance != null)
                {
                    _quartzServerInstances.AddOrUpdate(typeFullName, serverInstance,(key,oldInstance)=> {
                        return serverInstance;
                    });
                }
            }
            catch (Exception Ex)
            {
                logger.FatalFormat("==>任务服务实例创建出错:{0}", Ex.Message);
            }
            return serverInstance;
        }

        /// <summary>
        /// 获取已创建的任务调度实例(缓存中读取)
        /// </summary>
        /// <typeparam name="T">计划任务调度类型,必须派生自<see cref="BaseSchedulerTaskServer"/> </typeparam>
        /// <returns></returns>
        public static IQuartzServer GetSchedulerServer<T>()
        {
            var typeObject = Activator.CreateInstance<T>().GetType();
            string typeName = string.Empty;
            if (typeObject.IsSubclassOf(typeof(BaseSchedulerTaskServer)))
            {
                typeName = typeObject.FullName;
            }
            if (string.IsNullOrEmpty(typeName))
            {
                logger.FatalFormat("无效的IQuartzServer类型:{0}", typeObject.FullName);
                throw new ArgumentException(string.Format("无效的IQuartzServer类型:{0}", typeObject.FullName));
            }
            return GetSchedulerServer(typeName);
        }
        /// <summary>
        /// 获取已创建的任务调度实例
        /// </summary>
        /// <param name="typeFullName">计划任务调度类型名称(GetType().FullName)</param>
        /// <returns></returns>
        public static IQuartzServer GetSchedulerServer(string typeFullName)
        {
            IQuartzServer serverInstance = null;
            try
            {
                if (_quartzServerInstances.ContainsKey(typeFullName) && _quartzServerInstances[typeFullName] != null)
                {
                    serverInstance = _quartzServerInstances[typeFullName];
                }
            }
            catch (Exception Ex)
            {
                logger.FatalFormat("==>获取任务调度实例<{0}>出错:{1}", typeFullName, Ex.Message);
            }
            return serverInstance;
        }
    }
    public class TypeNameComparer : IComparer<Type>
    {
        public int Compare(Type t1, Type t2)
        {
            if (t1.Namespace.Length > t2.Namespace.Length)
            {
                return 1;
            }

            if (t1.Namespace.Length < t2.Namespace.Length)
            {
                return -1;
            }

            return t1.Namespace.CompareTo(t2.Namespace);
        }
    }
}
