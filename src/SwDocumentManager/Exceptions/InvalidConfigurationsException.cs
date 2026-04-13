//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SwDocumentManager.Exceptions
{
    /// <summary>
    /// Raised when configuration metadata cannot be resolved or is internally inconsistent.
    /// 当配置元数据无法解析，或配置数据本身不一致时抛出该异常。
    /// </summary>
    public class InvalidConfigurationsException : Exception, IUserException
    {
        public SwDMConfigurationError Error { get; }

        /// <summary>
        /// Converts Document Manager configuration error codes to user-facing messages.
        /// 将 Document Manager 的配置错误码转换为面向用户的描述信息。
        /// </summary>
        private static string GetError(SwDMConfigurationError err) 
        {
            switch (err) 
            {
                case SwDMConfigurationError.SwDMConfigurationError_ComObjectDisconnected:
                    return "Document has been closed";

                case SwDMConfigurationError.SwDMConfigurationError_DataMissing:
                    return "Configuration data missing";

                case SwDMConfigurationError.SwDMConfigurationError_NameNotFound:
                    return "Configuration name not found";

                case SwDMConfigurationError.SwDMConfigurationError_RequiredArgumentNull:
                    return "Required argument is null";

                case SwDMConfigurationError.SwDMConfigurationError_Unknown:
                    return "Unknown error";

                default:
                    return "Generic error";
            }
        }

        internal InvalidConfigurationsException(SwDMConfigurationError err) 
            : base(GetError(err))
        {
            Error = err;
        }

        internal InvalidConfigurationsException(string err) : base(err)   
        {
            Error = SwDMConfigurationError.SwDMConfigurationError_Unknown;
        }

        internal InvalidConfigurationsException(string err, Exception ex) : base(err, ex)
        {
            Error = SwDMConfigurationError.SwDMConfigurationError_Unknown;
        }
    }
}
