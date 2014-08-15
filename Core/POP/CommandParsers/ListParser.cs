using System;
using System.Collections.Generic;

using Utils;

namespace Core.POP.CommandParsers
{
    public static class ListParser
    {
        public static Dictionary<int, int> Parse(List<string> messageList)
        {
            var dic = new Dictionary<int, int>();
            foreach(var s in messageList)
            {
                // A single message is like this: "ID SizeInBytes".
                string[] elements = s.Split(' ');
                int id, size = 0;

                int.TryParse(elements[0], out id);
                int.TryParse(elements[1], out size);

                dic.Add(id, size);
            }
            return dic;
        }

        public static void Display(Dictionary<int, int> messages)
        {
            foreach(var kv in messages)
            {
                Logger.Inbox("{0} - {1} bytes", kv.Key, kv.Value);
            }
        }
    }
}
