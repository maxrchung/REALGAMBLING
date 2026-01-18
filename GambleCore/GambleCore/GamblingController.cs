using GambleCore.Gambling;
using GambleCore.Interface;

namespace GambleCore
{
    public class GamblingController
    {
        private ulong _seed = 0;
        private readonly GambleConfig _config;

        public GamblingController(ulong seed = 0, GambleConfig config = null)
        {
            if (config == null) config = new GambleConfig();
            _config = config;
        }

        public GambleConfig Config => _config;

        public IGamblingBoard CreateBoard(int offset)
        {
            return new Board(this, offset, _seed);
        }
    }
}