using Microsoft.Extensions.Configuration;
using Salaros.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VultPay.Application.Inheritance
{
    public class BaseApplication
    {

        /// <summary>
        /// 
        /// </summary>
        internal IConfiguration Configuration { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal ConfigParser ConfigGlobal { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal bool RegisterSystemLog { get => bool.Parse(ConfigGlobal.GetValue("Application", "RegisterSystemLog")); }

        /// <summary>
        /// 
        /// </summary>
        internal bool RegisterTransactionLog { get => bool.Parse(ConfigGlobal.GetValue("Application", "RegisterTransactionLog")); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public BaseApplication(IConfiguration configuration)
        {
            Configuration = configuration;
            //
            ConfigGlobal = new ConfigParser(Configuration.GetSection("GlobalConfiguration").Value, new ConfigParserSettings
            {
                MultiLineValues = MultiLineValues.Simple | MultiLineValues.AllowValuelessKeys | MultiLineValues.QuoteDelimitedValues | MultiLineValues.AllowEmptyTopSection,
                Culture = new CultureInfo("en-US")
            });
        }
    }
}