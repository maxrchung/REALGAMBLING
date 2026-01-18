namespace GambleCore.Interface
{
    public interface IGamblingBoard
    {
        int Height { get; }
        int Width { get; }

        int[] GetRandomSteps();

        // calling this advances RNG state, be sure to only call it once per roll
        void PerformSteps(int[] steps);

        void AddWheel(IWheel wheel);
        void RemoveWheel(IWheel wheel);
    }
}