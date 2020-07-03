using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace Minesweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Создание окна
            var window = new RenderWindow(new VideoMode(800, 600), "Minesweeper");
            window.SetVerticalSyncEnabled(true);

            window.Closed += (a, e) => { window.Close(); };

            var rect = new RectangleShape()
            {
                Size = new Vector2f(10, 10),
                FillColor = Color.Yellow,
                Position = new Vector2f(10, 10)
            };

            var circle = new CircleShape()
            {
                Radius = 10,
                Position = new Vector2f(20,20),
                FillColor = new Color(130,50,80, 200)
            };

            while (window.IsOpen)
            {
                window.DispatchEvents();

                window.Clear();

                // Отрисовка
                window.Draw(rect);
                window.Draw(circle);

                window.Display();
            }
        }
    }
}