using Microsoft.EntityFrameworkCore;
using SimpleInjector;
using Snake.Data;
using Snake.Data.DataAccessLayers;
using Snake.Services;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int _size;

        private readonly GameService _gameService;
        private readonly LogOnService _logOnService;

        private readonly SnakeContext _context;

        private readonly GameStateDataAccessLayer _gameStates;

        public MainWindow()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase(nameof(SnakeContext));

            _context = new SnakeContext(optionsBuilder.Options);
            _logOnService = LogOnService.GetInstance(_context);

            _gameStates = new GameStateDataAccessLayer(_context);

            _size = 20;

            InitializeComponent();

            ReloadAccess();

            GetScoreData();

            if (snakeCanvas.Height % _size != 0 || snakeCanvas.Width % _size != 0)
                throw new System.Exception("Size is not fitting!");

            var tickService = new TickService();
            _gameService = new GameService((int)snakeCanvas.Height, (int)snakeCanvas.Width, _size, EndGame, tickService);

            tickService.AddDelegate(DrawField);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.W || e.Key == Key.Up)
            {
                _gameService.ChangeDircetion(Models.Dirctions.Up);
            }
            else if(e.Key == Key.S || e.Key == Key.Down)
            {
                _gameService.ChangeDircetion(Models.Dirctions.Down);
            }
            else if(e.Key == Key.A || e.Key == Key.Right)
            {
                _gameService.ChangeDircetion(Models.Dirctions.Right);
            }
            else if(e.Key == Key.A || e.Key == Key.Left)
            {
                _gameService.ChangeDircetion(Models.Dirctions.Left);
            }
        }

        private void DrawField()
        {
            Dispatcher.Invoke(() =>
            {
                snakeCanvas.Children.Clear();

                foreach (var item in _gameService.GetSnakePositions().ToList())
                {
                    var temp = new Ellipse { Width = _size, Height = _size, Stroke = Brushes.Red, StrokeThickness = 2 };

                    snakeCanvas.Children.Add(temp);

                    temp.SetValue(Canvas.LeftProperty, (double)item.X - (_size / 2));
                    temp.SetValue(Canvas.BottomProperty, (double)item.Y - (_size / 2));
                }

                var tempFood = _gameService.GetFoodPosition();
                if (!(tempFood.X == 0 && tempFood.Y == 0))
                {
                    var foodPoint = new Ellipse { Width = _size, Height = _size, Stroke = Brushes.Black, StrokeThickness = 2 };

                    snakeCanvas.Children.Add(foodPoint);

                    foodPoint.SetValue(Canvas.LeftProperty, (double)tempFood.X - (_size / 2));
                    foodPoint.SetValue(Canvas.BottomProperty, (double)tempFood.Y - (_size / 2));
                }
            });
        }

        private async void EndGame(string text)
        {
            DrawField();

            await _gameStates.Add(new Models.GameStateModel
            {
                UserId = _logOnService.LogOnModel.Id,
                Points = ((_gameService.GetSnakePositions().Count - 1) * 100),
                Date = System.DateTime.Now
            });

            MessageBox.Show(text);

            GetScoreData();

            _gameService.ResetGame();
        }

        private void ReloadAccess()
        {
            snakeCanvas.IsEnabled = _logOnService.IsLoggedIn;
            startGameButton.IsEnabled = _logOnService.IsLoggedIn;
            settingsButton.IsEnabled = _logOnService.IsLoggedIn;

            logOnButton.Content = (_logOnService.IsLoggedIn ? "Logout" : "Login");
        }

        private void logOnButton_Click(object sender, RoutedEventArgs e)
        {
            if((sender as Button)?.Content.ToString() == "Login")
            {
                var view = new LogOnWindow(_context);
                view.ShowDialog();
            }
            else
            {
                _logOnService.Logout();
            }

            ReloadAccess();
        }

        private void startGameButton_Click(object sender, RoutedEventArgs e)
        {
            _gameService.StartGame();
            DrawField();
        }

        private async void GetScoreData()
        {
            var data = await _gameStates.GetTopTen();

            Dispatcher.Invoke(() =>
            {
                scoreDataGrid.ItemsSource = data.Select(x => new
                {
                    User = x.Username,
                    Point = x.Points,
                    Date = x.Date
                });
            });
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented!!!");
        }
    }
}
