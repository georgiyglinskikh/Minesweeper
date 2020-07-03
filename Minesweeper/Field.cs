using System;
using SFML.Graphics;
using SFML.System;

namespace Minesweeper
{
    class Cell : Transformable, Drawable
    {
        // Типы
        public enum State
        {
            Open, Close, Flagged
        }

        public enum Type
        {
            Mine, None, Number
        }

        //свойства
        public int PositionX;
        public int PositionY;

        public Type type { get; private set; }
        public State state { get; private set; }
        private int Number;


        //конструктор по умолчанию
        public Cell(int PosX, int PosY)
        {
            PositionX = PosX;
            PositionY = PosY;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            // Отрисовка
        }
    }

    class Field
    {
        // Параметры
        public const int PixelsToNearestMine = 50;

        private Cell[,] Cells;

        private Random random = new Random();

        public Field(Vector2i size)
        {
            Cells = new Cell[size.X, size.Y];
        }

        private bool isMineInClickRadius(Vector2f clickPosition, Vector2i minePosition)
        {
            var dPositions = new Vector2f(
                Math.Abs(minePosition.X - clickPosition.X),
                Math.Abs(minePosition.Y - clickPosition.Y)
            );

            var res = Math.Sqrt(Math.Pow(dPositions.X, 2) + Math.Pow(dPositions.Y, 2));

            return res < Field.PixelsToNearestMine;
        }

        private Vector2i[] GenereateMines(int n, Vector2f clickPosition)
        {
            var res = new Vector2i[n];

            for (int i = 0; i < n; i++)
            {
                var newMinePosition = new Vector2f(
                    (float)random.NextDouble() * Cells.GetLength(0),
                    (float)random.NextDouble() * Cells.GetLength(1)
                );
            }
        }

        public void Generate() { }


    }
}