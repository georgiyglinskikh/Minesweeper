using SFML.Graphics;

namespace Minesweeper
{
    /** Класс содержит контент, нужный для отрисовки */
    internal static class Content
    {
        // region Объекты
        
        /** Квадрат, который используеться только при отрисовке */
        public static RectangleShape RectangleShape;
        
        // endregion
        
        
        // region Методы
        
        /** Загрузить в память текстуры, примитивы и тд. */
        public static void Load()
        {
            RectangleShape = new RectangleShape(); // Инициализация квадратной фигуры
        }
        
        // endregion
    }
}