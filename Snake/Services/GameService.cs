using Snake.Data.DataAccessLayers;
using Snake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Snake.Services
{
    public delegate void GameDelegate(string text);

    public class GameService
    {
        private event GameDelegate _gameEvent;

        private readonly int _heigth = 0;
        private readonly int _width = 0;

        private readonly int _size = 0;

        private readonly TickService _tickService;
        private CancellationTokenSource _tickServiceCancelationTokenSource;

        private Dirctions _currentDirection;

        private readonly List<Point> _snakePositions = new List<Point>();

        private Point _foodPos = new Point();

        public GameService(int heigth, int width, int size, GameDelegate gameDelegate, TickService tickService)
        {
            _tickServiceCancelationTokenSource = new CancellationTokenSource();

            _heigth = heigth;
            _width = width;
            _size = size;

            _gameEvent = gameDelegate;

            _tickService = tickService;
            _tickService.AddDelegate(Move);
        }

        public void StartGame()
        {
            _currentDirection = Dirctions.Down;

            _snakePositions.Clear();
            _snakePositions.Add(new Point { X = (_width / 2), Y = (_heigth / 2) });
            AddFoot();

            _tickServiceCancelationTokenSource = new CancellationTokenSource();

            _tickService.ResetTickTime();

            _tickService.StartTickTask(_tickServiceCancelationTokenSource.Token);
        }

        public List<Point> GetSnakePositions()
        {
            return _snakePositions;
        }

        public Point GetFoodPosition()
        {
            return _foodPos;
        }

        public void AddFoot()
        {
            var rand = new Random();

            int randX = 0;
            int randY = 0;

            do
            {
                randX = rand.Next((_size / 2), (_width - (_size / 2)));
                randY = rand.Next((_size / 2), (_heigth - (_size / 2)));
            } while (IsSnakePosition(randX, randY));

            _foodPos = new Point { X = randX, Y = randY };
        }

        public void ChangeDircetion(Dirctions dirction)
        {
            _currentDirection = dirction;
        }

        public void Move()
        {
            var head = _snakePositions.First();

            int oldX = head.X;
            int oldY = head.Y;

            switch (_currentDirection)
            {
                case Dirctions.Up:
                    head.Y += _size;
                    break;
                case Dirctions.Down:
                    head.Y -= _size;
                    break;
                case Dirctions.Right:
                    head.X += _size;
                    break;
                case Dirctions.Left:
                    head.X -= _size;
                    break;
                default:
                    break;
            }

            _snakePositions[0] = head;

            if (!IsGameOver())
            {
                CheckFoodConsumtion();
            }

            MoveSnake(oldX, oldY);
        }

        public bool IsGameOver()
        {
            var groups = _snakePositions.GroupBy(x => new { x.X, x.Y });

            var head = _snakePositions.First();

            if (IsSnakePosition(head.X, head.Y, true))
            {
                _gameEvent("Game over!");
                return true;
            }
            else if (head.X - (_size / 2) < 0 || head.X + (_size / 2) > _width || head.Y - (_size / 2) < 0 || head.Y + (_size / 2) > _heigth)
            {
                _gameEvent("Game over!");
                return true;
            }

            return false;
        }

        public void ResetGame()
        {
            _snakePositions.Clear();
            _foodPos = new Point();

            _tickServiceCancelationTokenSource.Cancel();
        }

        private void CheckFoodConsumtion()
        {
            var head = _snakePositions.First();

            if(ComparePosition(head, _foodPos))
            {
                AddTail();
                AddFoot();

                _tickService.ReduceTickTime();
            }
        }

        private void AddTail()
        {
            var end = _snakePositions.Last();

            switch (_currentDirection)
            {
                case Dirctions.Up:
                    _snakePositions.Add(new Point { X = end.X, Y = end.Y + _size });
                    break;
                case Dirctions.Down:
                    _snakePositions.Add(new Point { X = end.X, Y = end.Y - _size });
                    break;
                case Dirctions.Right:
                    _snakePositions.Add(new Point { X = end.X + _size, Y = end.Y });
                    break;
                case Dirctions.Left:
                    _snakePositions.Add(new Point { X = end.X - _size, Y = end.Y });
                    break;
                default:
                    break;
            }
        }

        private bool IsSnakePosition(int x, int y, bool withoutHead = false)
        {
            var testList = new List<Point>(_snakePositions);

            if(withoutHead)
                testList.Remove(testList.First());

            foreach (var item in testList)
            {
                if(ComparePosition(new Point { X = x, Y = y }, item))
                    return true;
            }

            return false;
        }

        private bool ComparePosition(Point pA, Point pB)
        {
            if ((pA.X + _size) > pB.X && (pA.X - _size) < pB.X
                && (pA.Y + _size) > pB.Y && (pA.Y - _size) < pB.Y)
                return true;

            return false;
        }

        private void MoveSnake(int oldX, int oldY)
        {
            for (int i = 1; i < _snakePositions.Count; i++)
            {
                int tempX = _snakePositions[i].X;
                int tempY = _snakePositions[i].Y;

                _snakePositions[i].X = oldX;
                _snakePositions[i].Y = oldY;

                oldX = tempX;
                oldY = tempY;
            }
        }
    }
}
