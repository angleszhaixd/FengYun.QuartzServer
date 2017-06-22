using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;

namespace FengYun.QuartzServer.Core
{
    /// <summary>
    /// 基于Topshelf的服务接口
    /// Author:zhaixd
    /// date:2016.06.16
    /// </summary>
    public interface IHostServer :IQuartzServer, ServiceControl, ServiceSuspend
    {

    }
}
