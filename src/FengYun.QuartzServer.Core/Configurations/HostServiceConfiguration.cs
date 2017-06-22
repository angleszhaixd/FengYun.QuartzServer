using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace FengYun.QuartzServer.Core
{
    /// <summary>
    /// Quartz windows服务承载配置文件
    /// Author:zhaixd
    /// date:2016.01.17
    /// </summary>
    public class HostServiceConfiguration : ConfigurationSection
    {
        public const string ConfigurationSectionName = "quartz.host";

        /// <summary>
        /// Windows服务名称
        /// </summary>
        [ConfigurationProperty("serviceName", IsRequired = true, DefaultValue = "quartzHostServiceName")]
        public string serviceName
        {
            get { return (string)base["serviceName"]; }
            set { base["serviceName"] = value; }
        }

        /// <summary>
        /// Windows服务显示的中文名称
        /// </summary>
        [ConfigurationProperty("serviceDisplayName", IsRequired = true, DefaultValue = "quartzHostServiceDisplayName")]
        public string serviceDisplayName
        {
            get { return (string)base["serviceDisplayName"]; }
            set { base["serviceDisplayName"] = value; }
        }

        /// <summary>
        /// Windows服务描述
        /// </summary>
        [ConfigurationProperty("serviceDescription", IsRequired = true, DefaultValue = "quartzHostServiceDescription")]
        public string serviceDescription
        {
            get { return (string)base["serviceDescription"]; }
            set { base["serviceDescription"] = value; }
        }

        /// <summary>
        /// windows服务默认承载的计划任务调度类型名称
        /// </summary>
        [ConfigurationProperty("serverImplementationTypeName", IsRequired = true, DefaultValue = "FengYun.QuartzServer.Core.BaseHostSchedulerTaskServer")]
        public string serverImplementationTypeName
        {
            get { return (string)base["serverImplementationTypeName"]; }
            set { base["serverImplementationTypeName"] = value; }
        }

        [ConfigurationProperty("sericeSQL", IsRequired = false)]
        public CDataElement sericeSQL
        {
            get { return (CDataElement)base["sericeSQL"]; }
            set { base["sericeSQL"] = value; }
        }
    }
    /// <summary>
    /// <![CDATA[]]>属性元素
    /// </summary>
    public class CDataElement : ConfigurationElement
    {
        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            CommandText = reader.ReadElementContentAs(typeof(string), null) as string;
        }
        protected override bool SerializeElement(System.Xml.XmlWriter writer, bool serializeCollectionKey)
        {
            if (writer != null)
            {
                writer.WriteCData(CommandText);
            }
            return true;
        }

        [ConfigurationProperty("data", IsRequired = false)]
        public string CommandText
        {
            get { return this["data"].ToString(); }
            set { this["data"] = value; }
        }
    }
}
