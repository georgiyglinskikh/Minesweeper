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
            // region Инициализация
            
            var fieldSize = new Vector2i(
                Convert.ToInt32(Console.ReadLine()),
                Convert.ToInt32(Console.ReadLine())
            ); // Запрос размера поля из консоли

            // Создание окна
            var window = new RenderWindow(new VideoMode(800, 600), "Minesweeper");
            window.SetVerticalSyncEnabled(true);

            var field = new Field(); // Создаем поле
            
            // region Добавление обработчиков событий
            
            // Обработчик закрытия окна
            window.Closed += (a, e) => { window.Close(); /* Закрытие окна */ };
            
            // Обработчик изменения размера окна
            window.Resized += (sender, eventArgs) =>
            {
                window.SetView(
                    new View(new FloatRect(0, 0, eventArgs.Width, eventArgs.Height))
                ); // Изменение размера области для отрисовки внутри окна

                if (field.WasGenerated) // Если поле уже сгенерированно, то обновить размеры клеток
                    field.UpdateCellSizes(window.Size);
            };

            // Обработчик нажатия клавиш мышки
            window.MouseButtonPressed += (sender, eventArgs) =>
            {
                if (!field.WasGenerated && eventArgs.Button == Mouse.Button.Left)
                    field.Generate(fieldSize, new Vector2i(eventArgs.X, eventArgs.Y)); // Если нажали ЛКМ, то генерируется поле 
            };
            
            // endregion
            
            // endregion

            // Главный игровой цикл
            while (window.IsOpen)
            {
                window.DispatchEvents(); // Запуск работы обработчиков событий

                window.Clear(); // Очистка экрана (заливка черным цветом)

                // Отрисовка в буфер
                window.Draw(field); // Отрисовка поля и его составляющих

                window.Display(); // Смена буфера = вывод на экран
            }
        }
    }
}