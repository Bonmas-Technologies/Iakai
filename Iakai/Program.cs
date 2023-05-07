using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Iakai
{
    internal class Program
    {
        static void Main()
        {
            var gameSettings = GameWindowSettings.Default;
            var nativeSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                StartVisible = true,
                StartFocused = true,
            };

            var game = new Game(gameSettings, nativeSettings);

            game.Run();
        }
    }
}