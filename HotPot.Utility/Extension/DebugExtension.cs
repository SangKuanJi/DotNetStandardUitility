using System;
using System.Diagnostics;

namespace HotPot.Utility.Extension
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
