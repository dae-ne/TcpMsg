namespace TcpMsg.Server
{
    class EntryPoint
    {
        static void Main()
        {
            var server = new Server();
            server.Run("127.0.0.1", 11000);
        }
    }
}
