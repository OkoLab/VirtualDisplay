﻿using System;
using System.IO;
using System.Reflection;

namespace VirtualDisplay
{
    static class Logger
    {
            //---------------------------------------------------------
            // Статический метод записи строки в файл лога с переносом
            //---------------------------------------------------------
            public static void WriteLine(string message)
            {
                using (StreamWriter sw = new StreamWriter(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\log.txt", true))
                {
                    sw.WriteLine(String.Format("{0,-23} {1}", DateTime.Now.ToString() + ":", message));
                }
            }
    }
}
