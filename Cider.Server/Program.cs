
namespace Cider.Server
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Core.Single<CommunicateServer>.Instance.Run();
            return 0;
        }
    }
}
