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
        public const string Ground = "Ground";
    }

    public static class Scene
    {
        public const string MainMenu = "MainMenu";
        public const string Game = "Game";
    }

    public static class Animation
    {
        public const string HorizontalMove = "Horizontal(X)";
        public const string CatJumping = "Cat Jump";
        public const string CatAttacking = "Cat Attack";
    }

    public static class AnimationTest
    {
        public const string CatJumping = "Cat Jumping";
        public const string CatDoubleJumping = "Cat Double Jumping";
        public const string CatOnGround = "Cat On Ground";
        public const string CatFalling = "Cat Is Falling";
    }

    public static class Input
    {
        public const string Movement = "PlayerMoveSystem";
        public const string Run = "Run";
        public const string Jump = "Jump";
        public const string Quest = "Quest";
    }
}