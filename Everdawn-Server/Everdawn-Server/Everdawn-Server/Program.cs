using System;

namespace Everdawn_Server
{
    class Program
    {
        private static readonly string __VERSION = "0.0.1";
        private static readonly string ServerTitle = "Everdawn Server v" + __VERSION;
        private static readonly int __PORT = 49368;

        static void Main(string[] args)
        {
            Console.Title = ServerTitle;

            Server.Start(10, __PORT);

            Console.ReadKey();
        }
    }
}
