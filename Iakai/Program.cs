using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
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
                Title = "Iakai - The Game",
                Size = new Vector2i(800, 600)
            };

            var game = new Game(gameSettings, nativeSettings);

            game.Run();
        }
    }
}