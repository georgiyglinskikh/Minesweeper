using System;
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

            while (window.IsOpen)
            {
                window.DispatchEvents();

                window.Clear();
                
                // Отрисовка
                

                window.Display();
            }
        }
    }
}