using System;
using SFML.Graphics;
using SFML.System;

namespace Minesweeper
{
    class Cell : Transformable, Drawable
    {
        public const Color ColorMine = Color.Black;
        public const Color ColorFlag = Color.Red;
        public const Color ColorClosed = Color.Grey;
        public const Color ColorNumber = Color.Blue;

        public bool isMine { get; set; }
        public bool isOpened { get; set; }
        public bool isFlagged { get; set; }

        /** 0 = Пустая клетка */
        public int Number { get; set; }

        public void Draw(RenderTarget target, RenderStates states)
        {

        }
    }

    class Field : Transformable, Drawable
    {
        // Параметры
        public const float RadiusWithoutMines = 50;

        public Cell[,] Cells { get; set; }

        private static Random rand = new Random();

        // Vector[измерения (2-4)][тип (f = float, i = int, u = uint)]

        public static bool IsMineInRadius(float r, Vector2f clickPosition, Vector2i minePosition)
        {
            var delta = new Vector2f(Math.Abs(clickPosition.X - minePosition.X), Math.Abs(clickPosition.X - minePosition.X));

            var hypo = Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2));

            return hypo <= r;
        }

        public Vector2i[] GenerateMines(int n, Vector2f clickPosition)
        {
            var res = new Vector2i[n];

            // Логика создания
            for (int i = 0; i < n; i++)
            {
                var newCoof = new Vector2f((float)rand.NextDouble(), (float)rand.NextDouble());

                var newCoord = new Vector2i(
                    (int)((float)Cells.GetLength(0) * newCoof.X),
                    (int)((float)Cells.GetLength(1) * newCoof.Y)
                );

                bool isCollision = false;
                for (int j = 0; j < i; j++)
                    if (res[j].X == newCoord.X && res[j].Y == newCoord.Y)
                        isCollision = true;

                if (isCollision && IsMineInRadius(RadiusWithoutMines, clickPosition, newCoord))
                    i--;
                else
                    res[n] = newCoord;
            }

            return res;
        }

        public int GetNumberOf(Vector2i certer, Func<int, int, bool> comp)
        {
            // if (Cells[certer.X, certer.Y].isMine)
            //     return -1;

            // (i, j) => Cells[certer.X + i, certer.Y + j].IsMine
            if (comp(certer.X, certer.Y))
                return -1;

            int res = 0;

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (certer.X + i >= 0 &&
                        certer.Y + j >= 0 &&
                        certer.X + i >= Cells.GetLength(0) &&
                        certer.Y + j >= Cells.GetLength(1) &&
                        comp(certer.X + i, certer.Y + j))
                        res++;

            return res;
        }

        public void Generate(Vector2i size, Vector2f clickPosition)
        {
            Cells = new Cell[size.X, size.Y];

            var mines = GenerateMines(16, clickPosition);
            foreach (var mine in mines)
                Cells[mine.X, mine.Y] = new Cell() { isMine = true };

            for (int i = 0; i < Cells.GetLength(0); i++)
                for (int j = 0; j < Cells.GetLength(1); j++)
                    Cells[i, j] = new Cell()
                    {
                        Number = GetNumberOf(new Vector2i(i, j), (int x, int y) => Cells[x, y].isMine)
                    };

            for (int i = 0; i < Cells.GetLength(0); i++)
                for (int j = 0; j < Cells.GetLength(1); j++)
                    Cells[i, j].isOpened = true;

            var clickedCellPosition =
                new Vector2i((int)Math.Round(clickPosition.X), (int)Math.Round(clickPosition.Y));

            Open(clickedCellPosition);
        }

        public static Vector2f CellSize;

        public void UpdateCellSizes(Vector2u windowSize)
        {
            CellSize = new Vector2f();
        }

        public void Open(Vector2i clickPosition)
        {
            // TODO:
            Cells[clickPosition.X, clickPosition.Y].isOpened = true;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var resPosition =
                        new Vector2i(clickPosition.X + i, clickPosition.Y + j);

                    if (GetNumberOf(resPosition, (int x, int y) => Cells[x, y].Number != 0) <= 3 &&
                        Cells[resPosition.X, resPosition.Y].Number != -1)
                        Open(resPosition);
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    target.Draw(Cells[i, j]);
                }
            }
        }
    }
}