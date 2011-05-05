using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTestTarget
{


    static class StringHelper
    {
        private static bool _initialized;
        private static List<string> _list;

        private static void Initialize()
        {
            _list = new List<string>(16);
            _list.Add("Help");
            _list.Add("Extra");
            _list.Add("Secret");
            _initialized = true;
        }

        public static string Get(int index)
        {
            if (!_initialized)
                Initialize();

            return _list[index];
        }
    }

    class Program
    {

        private static string TestPrivate()
        {
            return "Private";
        }

        private static string TestProtected()
        {
            return "Protected";
        }

        internal static string TestInternal()
        {
            return "Internal";
        }

        public static string TestPublic()
        {
            return "Public";
        }

        public static void HandleSomething(string test)
        {
            var temporary = test.Split(',');
        }

        static void Main()
        {
            try
            {
                HandleSomething("some");
                HandleSomething("test");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
