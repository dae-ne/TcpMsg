using System;
using System.Threading;

namespace Server
{
    class EntryPoint
    {
        static void Main()
        {
            var listener = new Thread(new ThreadStart(Server.Start)) { IsBackground = true };
            listener.Start();
            Console.WriteLine("Press <enter> to quit");
            Console.ReadLine();
        }
    }
}
