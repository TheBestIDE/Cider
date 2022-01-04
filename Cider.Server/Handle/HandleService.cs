using Cider.IO;
using Cider.Math;

namespace Cider.Server.Handle
{
    public class HandleService : HandleServiceBase
    {
        public override void HandleDirtyBlock(BlockedFile file)
        {
            throw new NotImplementedException();
        }

        public override int HandleHashList(string[] hashs)
        {
            throw new NotImplementedException();
        }

        public override FileBlock[] HandleLinearResult(GFMatrix matrix)
        {
            throw new NotImplementedException();
        }
    }
}