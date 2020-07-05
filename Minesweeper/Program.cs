using System;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace Minesweeper
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // --- Инициализация ---
            
            var fieldSize = new Vector2i(
                Convert.ToInt32(Console.ReadLine()),
                Convert.ToInt32(Console.ReadLine())
            ); // Запрос размера поля из консоли

            // Создание окна
            var window = new RenderWindow(new VideoMode(800, 600), "Minesweeper");
            window.SetVerticalSyncEnabled(true);

            var field = new Field(); // Создаем поле
            
            Content.Load();
            
            
            // --- Добавление обработчиков событий ---
            
            // Обработчик закрытия окна
            window.Closed += (a, e) => { window.Close(); };
            
            // Обработчик изменения размера окна
            window.Resized += (sender, eventArgs) =>
            {
                window.SetView(
                    new View(new FloatRect(0, 0, eventArgs.Width, eventArgs.Height))
                ); // Изменение размера области для отрисовки внутри окна

                if (field.WasGenerated)
                    field.UpdateCellSizes(window.Size);
            };

            // Обработчик нажатия клавиш мышки
            window.MouseButtonPressed += (sender, eventArgs) =>
            {
                if (!field.WasGenerated && eventArgs.Button == Mouse.Button.Left)
                    field.Generate(fieldSize, new Vector2i(eventArgs.X, eventArgs.Y), window.Size);
            };

            // --- Главный игровой цикл ---
            while (window.IsOpen)
            {
                window.DispatchEvents(); // Запуск работы обработчиков событий

                window.Clear(Cell.ColorClosed); // Очистка экрана (заливка черным цветом)

                // Отрисовка в буфер
                window.Draw(field);

                window.Display(); // Смена буфера = вывод на экран
            }
        }
    }
}