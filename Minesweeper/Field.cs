using System;
using SFML.Graphics;
using SFML.System;

namespace Minesweeper
{
    internal class Cell : Transformable, Drawable
    {
        public static Vector2f CellSize;

        // region Константы
        
        /** Цвет заливки для минки */
        public static readonly Color ColorMine = Color.Black;
        
        /** Цвет флага, не видать врага */
        public static readonly Color ColorFlag = Color.Red;
        
        /** Цвет закрытой клетки, как фантик у конфетки */
        public static readonly Color ColorClosed = new Color(127, 127, 127);
        
        /** Цвет номера, ему все рады, ведь он преодолел преграды */
        public static readonly Color ColorNumber = Color.Blue;
        
        // endregion

        /** Является ли клетка минкой? */
        public bool IsMine { get; set; }
        
        /** Открыта ли клетка (по умолчанию - нет)? */
        public bool IsOpened { get; set; }
        
        /** Поставлен ли флаг на клетку? */
        public bool IsFlagged { get; set; }

        /** Число минок расположеных по близости 0 = Пустая клетка */
        public int Number { get; set; }

        /** Отрисовка примитивов, формирующих клетку */
        public void Draw(RenderTarget target, RenderStates states)
        {
            // Установка цвета в зависимости от состояния клетки
            if (IsMine)
                Content.RectangleShape.FillColor = ColorMine;
            if (Number > 0)
                Content.RectangleShape.FillColor = ColorNumber;
            if (!IsOpened)
                Content.RectangleShape.FillColor = ColorClosed;
            if (IsFlagged)
                Content.RectangleShape.FillColor = ColorFlag;

            Content.RectangleShape.Size = CellSize;
            Content.RectangleShape.Position = Position;

            target.Draw(Content.RectangleShape);
        }
    }

    internal class Field : Transformable, Drawable
    {
        // Параметры
        private const float RadiusWithoutMines = 50;

        public bool WasGenerated { get; private set; }

        private Cell[,] Cells { get; set; }

        private static readonly Random Random = new Random();

        // Vector[измерения (2-4)][тип (f = float, i = int, u = uint)]

        private static bool IsMineInRadius(float r, Vector2i clickPosition, Vector2i minePosition)
        {
            var delta = new Vector2f(Math.Abs(clickPosition.X - minePosition.X),
                Math.Abs(clickPosition.X - minePosition.X));

            var hypo = Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2));

            return hypo <= r;
        }

        private Vector2i[] GenerateMines(int n, Vector2i clickPosition)
        {
            var res = new Vector2i[n];

            // Логика создания
            for (var i = 0; i < n; i++)
            {
                var newCoefficient = new Vector2f((float) Random.NextDouble(), (float) Random.NextDouble());

                var newCoordinates = new Vector2i(
                    (int) (Cells.GetLength(0) * newCoefficient.X),
                    (int) (Cells.GetLength(1) * newCoefficient.Y)
                );

                var isCollision = false;
                for (var j = 0; j < i; j++)
                    if (res[j].X == newCoordinates.X && res[j].Y == newCoordinates.Y)
                        isCollision = true;

                if (isCollision || IsMineInRadius(RadiusWithoutMines, clickPosition, newCoordinates))
                    i--;
                else
                    res[n] = newCoordinates;
            }

            return res;
        }

        private int GetNumberOf(Vector2i center, Func<int, int, bool> comp)
        {
            if (comp(center.X, center.Y))
                return -1;

            var res = 0;

            for (var i = -1; i <= 1; i++)
            for (var j = -1; j <= 1; j++)
                if (center.X + i >= 0 &&
                    center.Y + j >= 0 &&
                    center.X + i >= Cells.GetLength(0) &&
                    center.Y + j >= Cells.GetLength(1) &&
                    comp(center.X + i, center.Y + j))
                    res++;

            return res;
        }

        private void ForEachInCells(Func<int, int, int> func)
        {
            for (var i = 0; i < Cells.GetLength(0); i++)
            for (var j = 0; j < Cells.GetLength(1); j++)
                func(i, j);
        }

        public void Generate(Vector2i size, Vector2i clickPosition)
        {
            Cells = new Cell[size.X, size.Y];

            var mines = GenerateMines(16, clickPosition);
            foreach (var mine in mines)
                Cells[mine.X, mine.Y] = new Cell {IsMine = true};

            ForEachInCells((i, j) =>
            {
                Cells[i, j] = new Cell
                {
                    Number = GetNumberOf(new Vector2i(i, j), (x, y) => Cells[x, y].IsMine)
                };
                return 0;
            });

            ForEachInCells((i, j) =>
            {
                Cells[i, j].IsOpened = true;
                return 0;
            });

            Open(clickPosition);

            WasGenerated = true;
        }

        public void UpdateCellSizes(Vector2u windowSize)
        {
            Cell.CellSize = new Vector2f(
                windowSize.X / (float) Cells.GetLength(0),
                windowSize.Y / (float) Cells.GetLength(1)
            );

            ForEachInCells((i, j) =>
            {
                Cells[i, j].Position = new Vector2f(
                    Cell.CellSize.X * i,
                    Cell.CellSize.Y * j); // TODO: Сделать зазор
                return 0;
            });
        }

        private void Open(Vector2i clickPosition)
        {
            // TODO:
            Cells[clickPosition.X, clickPosition.Y].IsOpened = true;

            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    var resPosition =
                        new Vector2i(clickPosition.X + i, clickPosition.Y + j);

                    if (GetNumberOf(resPosition, (x, y) => Cells[x, y].Number != 0) <= 3 &&
                        Cells[resPosition.X, resPosition.Y].Number != -1)
                        Open(resPosition);
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (WasGenerated)
            {
                ForEachInCells((i, j) =>
                {
                    target.Draw(Cells[i, j]);
                    return 0;
                });
            }
        }
    }
}