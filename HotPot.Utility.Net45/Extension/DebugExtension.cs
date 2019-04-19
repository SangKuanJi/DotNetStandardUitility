using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HotPot.Utility.Net45.Extension
{
    public static class DebugExtension
    {
        public static Action<string> DebugMsgAction { get; set; }
        public static void WriteLine(this string value)
        {
            DebugMsgAction?.Invoke(value);
#if DEBUG
            var methodBase = new StackTrace().GetFrame(1).GetMethod();
            Debug.WriteLine($"---------------|||||||||||||{methodBase.Name} {DateTime.Now} 测试结果 Begin|||||||||||||||------------------");
            Debug.WriteLine(value);
            Debug.WriteLine($"---------------|||||||||||||{methodBase.Name} {DateTime.Now} 测试结果 E n  d|||||||||||||||------------------");
#endif
        }
    }
}
