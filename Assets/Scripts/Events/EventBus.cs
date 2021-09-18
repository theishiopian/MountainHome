public delegate void GameEvent(object caller, params object[] eventParameters);

public class EventBus
{
    public static class BlockEvents
    {
        public static GameEvent OnBlockBreak;
        public static GameEvent OnBlockPlace;
        public static GameEvent OnBlockReplace;
    }

    public static class MinecartEvents
    {
        public static GameEvent OnMinecartPlaced;
        public static GameEvent OnMinecartReversed;
    }

    public static class PlayerEvents
    {
        public static GameEvent OnPlayerDamageBlock;
        public static GameEvent OnPlayerDestroyBlock;
        public static GameEvent OnPlayerJump;
        public static GameEvent OnPlayerLand;
        public static GameEvent OnPlayerStartRiding;
        public static GameEvent OnPlayerStopRiding;
    }

    public static class GenerationEvents
    {
        public static GameEvent OnGenerationStart;
        public static GameEvent OnGenerationEnd;
    }
}
