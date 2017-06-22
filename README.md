# FengYun.QuartzServer

计划任务调度器，基于quartz，tophselp，Common.Logging.NLog实现快速开发计划任务或windows服务。


## 使用说明

一、windows服务或控制台应用程序安装及使用
1、通过vs nuget包管理器安装 FengYun.QuartzServer.WindowsHost，命令：PM> Install-Package FengYun.QuartzServer.WindowsHost。
2、编写具体的任务执行代码，参考 /QuartzServerDemos/MyTestJob.cs，实现抽象方法ExecuteJob。[quartz任务文档](https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/more-about-jobs.html)
3、编写计划任务调度器，参考 /QuartzServerDemos/MyTestHostServer.cs，实现抽象方法initScheduleJobs，添加步骤2中编写的计划任务详情，具体使用CreateJob<TJob>()方法进行任务的添加，任务触发器的使用可参考官方文档。[quartz触发器文档](https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/crontriggers.html)
4、windows服务启动配置调用，参考 Program.cs。
5、App.Config配置文件，🌹类库安装后，App.Config中会自动添加<quartz.host></quartz.host>,此配置项主要用户设置windows服务安装的基本信息,可以根据实际情况自行设置,注意quartz.host.serverImplementationTypeName的值。

二、web应用程序安装及使用
1、通过vs nuget包管理器安装 FengYun.QuartzServer.WebHost，命令：PM> Install-Package FengYun.QuartzServer.WebHost。
2、编写具体的任务执行代码，参考 /QuartzServerDemos/MyTestJob.cs，实现抽象方法ExecuteJob。[quartz任务文档](https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/more-about-jobs.html)
3、编写计划任务调度器，参考 /QuartzServerDemos/MyTestServer.cs，实现抽象方法initScheduleJobs，添加步骤2中编写的计划任务详情，具体使用CreateJob<TJob>()方法进行任务的添加，任务触发器的使用可参考官方文档。[quartz触发器文档](https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/crontriggers.html)
4、安装类库后，项目中会自动添加App_Start文件夹，其中QuartzServerConfig.cs为任务调度器启动配置代码，Register方法会在应用程序启动时自动调用，可根据实际情况自行修改需要启动的任务调度器实例。

# 文件目录

```
.
├── App_Start									// web程序自启动配置
│   ├── QuartzServerConfig.cs					// web应用程序启动时自动注册默认的计划任务调度器
├── NLogNetworkTools                             // NLog日志查看器
│   ├── NLogMessageForm.exe						// NLog日志查看器主程序
│   ├── NLogMessageForm.exe.config				// NLog日志查看器主程序配置
├── QuartzServerDemos                            // 任务调度器及任务详情代码模板
│   ├── MyTestJob.cs							// 测试任务详情,需要继承自BaseSchedulerJob
│   ├── MyTestHostServer.cs						// 测试系统服务计划任务调度器(一般用于windows服务)
│   ├── MyTestServer.cs							// 测试普通计划任务调度器类型(一般用于手动触发计划任务)
├── SericeInstallerBats                          // 服务安装批处理文件，🌹具体使用见“说明.txt”
│   ├── 安装.bat								// windows服务安装脚本
│   ├── 启动~停止~重启.bat						// windows服务启动、停止、重启脚本
│   ├── 卸载.bat								// windows服务卸载脚本
│   ├── 说明.txt								// 批处理脚本使用说明
├── NLog.config                                  // NLog日志组件配置,🌹需要设置此文件的属性:复制到输出目录=始终复制,生成操作=内容
├── App.config/Web.config                        // 系统配置,🌹需要设置配置项<quartz.host ></quartz.host>
├── Program.cs									// windwos服务启动配置，🌹注意：HostServiceBootstrap.AutoRun<THostServer>()

# License

[GPL](https://github.com/angleszhaixd/FengYun.QuartzServer/master/COPYING)