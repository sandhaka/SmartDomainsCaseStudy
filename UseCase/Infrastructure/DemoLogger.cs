using System;

namespace UseCase.Infrastructure
{
    public static class DemoLogger
    {
        public static void DomainLog(string text, bool line = true)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            if (line) Console.WriteLine($"[DOMAIN] {text}");
            else Console.Write(text);
            Console.ResetColor();
        }

        public static void InfLog(string text, bool line = true)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            if (line) Console.WriteLine($"[INFRASTRUCTURE] {text}");
            else Console.Write(text);
            Console.ResetColor();
        }
    }
}