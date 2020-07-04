using SFML.Graphics;

namespace Minesweeper
{
    /** Класс содержит контент, нужный для отрисовки */
    internal static class Content
    {
        // --- Примитивы ---
        
        /** Квадрат (используеться для отрисовки) */
        public static RectangleShape RectangleShape;
        
        
        // --- Методы ---
        /** Загрузить в память текстуры, примитивы и тд. */
        public static void Load()
        {
            RectangleShape = new RectangleShape();
        }
    }
}