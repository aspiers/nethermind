using System.Threading;

namespace Nevermind.Core.Potocol
{
    public class Byzantium : IEthereumRelease
    {
        private static IEthereumRelease _instance;

        private Byzantium()
        {
        }

        public static IEthereumRelease Instance => LazyInitializer.EnsureInitialized(ref _instance, () => new Byzantium());
        
        public bool IsTimeAdjustmentPostOlympic => true;
        public bool AreJumpDestinationsUsed => false;
        public bool IsEip2Enabled => true;
        public bool IsEip7Enabled => true;
        public bool IsEip100Enabled => true;
        public bool IsEip140Enabled => true;
        public bool IsEip150Enabled => true;
        public bool IsEip155Enabled => true;
        public bool IsEip158Enabled => true;
        public bool IsEip160Enabled => true;
        public bool IsEip170Enabled => true;
        public bool IsEip196Enabled => true;
        public bool IsEip197Enabled => true;
        public bool IsEip198Enabled => true;
        public bool IsEip211Enabled => true;
        public bool IsEip214Enabled => true;
        public bool IsEip649Enabled => true;
        public bool IsEip658Enabled => true;
    }
}