public static class GameConstant
{
    public static class Layer
    {
        public const int Default = 0;
        public const int TransparentFX = 1;
        public const int IgnoreRaycast = 2;
        public const int Water = 4;
        public const int UI = 5;
        public const int Player = 6;
        public const int Enemy = 7;
        public const int Ground = 8;
        public const int Interactable = 9;
        public const int Quest = 10;
    }

    public static class Tag
    {
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string Interactable = "Interactable";
        public const string Quest = "Quest";
    }

    public static class Scene
    {
        public const string MainMenu = "MainMenu";
        public const string Game = "Game";
    }

    public static class Animation
    {
        public const string IsRunning = "Horizontal(X)";
        public const string IsRunningY = "Vertical(Y)";
        public const string IsJumping = "IsJumping";
    }

    public static class Input
    {
        public const string Movement = "Movement";
        public const string Run = "Run";
        public const string Jump = "Jump";
        public const string Quest = "Quest";
    }
}