using SFML.Graphics;

namespace Minesweeper
{
    static class Content
    {
        public static RectangleShape rectangle;

        public static void Load()
        {
            rectangle = new RectangleShape();
        }
    }
}