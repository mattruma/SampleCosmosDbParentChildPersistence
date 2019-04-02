using System;

namespace Demo2ConsoleApp.Helpers
{
    public class ConsoleHelper
    {
        public static void WriteLine(
            string text,
            bool autoRead = false)
        {
            Console.WriteLine(text);

            if (autoRead) Console.Read();
        }

        public static void WriteLine(
            string text,
            ConsoleColor color,
            bool autoRead = false)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();

            if (autoRead) Console.Read();
        }
    }
}
