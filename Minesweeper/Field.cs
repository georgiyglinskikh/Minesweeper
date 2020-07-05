using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace Minesweeper
{
    internal class Cell : Transformable, Drawable
    {
        /**Открыто? Есть флаг? Мина? Число (0 - пустая клетка)
         * |        |          |     |
         * 0________0__________0_____0000 */
        public enum States
        {
            Open = 0x1000000,
            Flag = 0x0100000,
            Mine = 0x0010000
        }

        // --- Параметры (как часть состояния) ---
        public static Vector2f CellSize;

        // --- Константы ---
        private static readonly Color ColorMine = Color.Black;
        private static readonly Color ColorFlag = Color.Red;
        private static readonly Color ColorClosed = new Color(127, 127, 127);
        private static readonly Color ColorNumber = Color.Blue;

        // --- Поля ---
        private int State;

        // --- Свойства ---
        /** Является ли клетка минкой? */
        public bool IsMine
        {
            get => IsState(States.Mine);
            set => ChangeState(States.Mine);
        }

        /** Открыта ли клетка (по умолчанию - нет)? */
        public bool IsOpened
        {
            get => IsState(States.Open);
            set => ChangeState(States.Open);
        }

        /** Поставлен ли флаг на клетку? */
        public bool IsFlagged
        {
            get => IsState(States.Flag);
            set => ChangeState(States.Open);
        }

        /** Число минок расположеных по близости 0 = Пустая клетка До 8 */
        public int Number
        {
            get => State & 0x1111;
            set
            {
                State &= 0x0;
                State |= value;
            }
        }

        // --- Методы ---
        public bool IsState(States state) => (State & (int) state) == (int) state;

        public void ChangeState(States state)
        {
            State ^= (int) state;
        }

        /** Отрисовка примитивов, формирующих клетку */
        public void Draw(RenderTarget target, RenderStates states)
        {
            // Установка цвета в зависимости от состояния клетки
            Content.RectangleShape.FillColor = Color.Magenta;
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

    /** Поле, для которого реализована отрисовка и генерация */
    internal class Field : Transformable, Drawable
    {
        // --- Константы ---
        private const float RadiusWithoutMines = 50;
        private static readonly Random Random = new Random();

        // Свойства
        /** Снегерировано поле или пустое? */
        public bool WasGenerated { get; private set; }

        /** 2-d массив клеток, представляющий поле */
        private Cell[][] Cells { get; set; }

        private Vector2i CellsSize { get; set; }

        // --- Вспомогательные функции ---
        private static bool IsMineInRadius(float r, Vector2i clickPosition, Vector2i minePosition)
        {
            var delta = new Vector2f(Math.Abs(clickPosition.X - minePosition.X),
                Math.Abs(clickPosition.X - minePosition.X));

            var hypo = Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2));

            return hypo <= r;
        }

        private IEnumerable<Vector2i> GenerateMines(int n, Vector2i clickPosition)
        {
            var res = new Vector2i[n];

            // Логика создания
            for (var i = 0; i < n; i++)
            {
                var newCoefficient = new Vector2f((float) Random.NextDouble(), (float) Random.NextDouble());

                var newCoordinates = new Vector2i(
                    (int) (CellsSize.X * newCoefficient.X),
                    (int) (CellsSize.Y * newCoefficient.Y)
                );

                var isCollision = false;
                for (var j = 0; j < i; j++)
                    if (res[j].X == newCoordinates.X && res[j].Y == newCoordinates.Y)
                        isCollision = true;

                if (isCollision || IsMineInRadius(RadiusWithoutMines, clickPosition, newCoordinates))
                    i--;
                else
                    res[i] = newCoordinates;
            }

            return res;
        }

        private int GetNumberOf(Vector2i center, int number)
        {
            var res = 0;
            if (Cells[center.X][center.Y].Number == number)
                return -1;

            for (var i = -1; i <= 1; i++)
            for (var j = -1; j <= 1; j++)
                if (IsPositionInField(new Vector2i(center.X + i, center.Y + j)) &&
                    Cells[center.X + i][center.Y + j].Number == number)
                    res++;

            return res;
        }

        private void ForEachInCells(Func<int, int, int> func)
        {
            for (var i = 0; i < CellsSize.X; i++)
            for (var j = 0; j < CellsSize.Y; j++)
                func(i, j);
        }

        public Vector2i ToFieldCoordinates(Vector2i position, Vector2u windowSize) =>
            new Vector2i(position.X * CellsSize.X / (int) windowSize.X, position.Y * CellsSize.Y / (int) windowSize.Y);

        public Vector2i ToScreenCoordinates(Vector2i position, Vector2u windowSize) =>
            new Vector2i(
                (int) ((float) position.X * windowSize.X / CellsSize.X),
                (int) ((float) position.Y * windowSize.Y / CellsSize.Y)
            );

        public bool IsPositionInField(Vector2i position) =>
            position.X >= 0 && position.Y >= 0 && position.X < CellsSize.X &&
            position.Y < CellsSize.Y;

        // --- Методы ---
        /** Создание поля после клика */
        public void Generate(Vector2i size, Vector2i clickPosition, Vector2u windowSize)
        {
            Cells = new Cell[size.X][];
            for (var i = 0; i < size.X; i++)
                Cells[i] = new Cell[size.Y];

            CellsSize = size;

            var scaledClickPosition = ToFieldCoordinates(clickPosition, windowSize);

            // Вставка мин на поле
            var mines = GenerateMines((int) (CellsSize.X * CellsSize.Y / 6.4f), clickPosition);
            foreach (var mine in mines)
                Cells[mine.X][mine.Y] = new Cell {IsMine = true, Number = -1};

            ForEachInCells((i, j) =>
            {
                Cells[i][j] = new Cell();
                return 0;
            });

            // Устанавливаем цифры в клетках по количеству мин поблизости
            ForEachInCells((i, j) =>
            {
                Cells[i][j].Number = GetNumberOf(new Vector2i(i, j), -1);
                Cells[i][j].IsOpened = false;
                return 0;
            });

            // Открываем клетки в месте клика
            Open(scaledClickPosition);

            WasGenerated = true;
        }

        /** Изменения размера клеток после изменения размера окна */
        public void UpdateCellSizes(Vector2u windowSize)
        {
            Cell.CellSize = new Vector2f(
                windowSize.X / (float) CellsSize.X,
                windowSize.Y / (float) CellsSize.Y
            ); // По пропорции вычисляем нужный размер

            ForEachInCells((i, j) =>
            {
                Cells[i][j].Position = new Vector2f(
                    Cell.CellSize.X * i,
                    Cell.CellSize.Y * j); // TODO: Сделать зазор
                return 0;
            }); // Изменяем положение клеток на основе нового размера
        }

        /** Рекурсивное открытие пустых клеток, пока рядом не оказываються минимум 2 других цифры */
        private void Open(Vector2i clickPosition)
        {
            var next = new Queue<Vector2i>();
            next.Enqueue(clickPosition);

            var used = new List<Vector2i>();

            while (next.Count > 0)
            {
                var current = next.Dequeue();
                
                Cells[current.X][current.Y].IsOpened = true;
                
                // Перебор ближайших клеток
                for (var i = -1; i <= 1; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        var resPosition =
                            new Vector2i(current.X + i, current.Y + j);

                        if (IsPositionInField(resPosition) && 
                            8 - GetNumberOf(resPosition, 0) >= 3 &&
                            Cells[resPosition.X][resPosition.Y].Number != -1 && 
                            !used.Contains(resPosition))
                        {
                            next.Enqueue(resPosition); // Рекурсивный вызов
                            used.Add(resPosition);
                        }

                    }
                }
            }
            
            
        }

        /** Отрисовка всех клеток на поле */
        public void Draw(RenderTarget target, RenderStates states)
        {
            if (WasGenerated)
            {
                ForEachInCells((i, j) =>
                {
                    target.Draw(Cells[i][j]);
                    return 0;
                }); // Вызов метода отрисовки у каждой клетки
            }
        }
    }
}