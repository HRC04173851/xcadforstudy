using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Exceptions;

namespace SwAddInExample
{
    // Custom exception implementing IUserException, which signals to the xCAD framework
    // that this error is a user-friendly message to be shown in a dialog (not a developer crash)
    // 中文：实现 IUserException 的自定义异常，向 xCAD 框架表明这是一条用户友好的错误消息
    // 中文：（以对话框形式显示给用户，而非开发者级别的崩溃错误）
    public class UserException : Exception, IUserException
    {
        // Constructor: passes the user-visible message to the base Exception class
        // 中文：构造函数：将面向用户的错误消息传递给基类 Exception
        public UserException(string msg) : base(msg) 
        {
        }
    }
}
