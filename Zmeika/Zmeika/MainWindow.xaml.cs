using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Zmeika
{
    public partial class MainWindow : Window
    {

        public const int SnakeSquareSize = 20;

        public readonly SolidColorBrush _snakeColor = Brushes.White;
        public enum Direction
        {
            Left, Right, Top, Bottom
        }
        public Direction _direction = Direction.Right;
        public const int TimerInterval = 200;
        public DispatcherTimer _timer;

        public Rectangle _snakeHead;
        public Point _foodPosition;

        public static readonly Random randomPositionFood = new Random();

        public List<Rectangle> _snake = new List<Rectangle>();

        public int _score = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void InitialGame()
        {
            _snakeHead = CreateSnakeSegment(new Point(5, 5));
            _snake.Add(_snakeHead);
            GameCanvas.Children.Add(_snakeHead);

            PlaceFood();

            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromMilliseconds(TimerInterval);
            _timer.Start();
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            Point newHeadPosition = CalcuteNewHeadPosition();

            if(newHeadPosition == _foodPosition)
            {
                EatFood();
                PlaceFood();
            }

            if(newHeadPosition.X < 0 || newHeadPosition.Y < 0
                || newHeadPosition.X >= GameCanvas.ActualWidth / SnakeSquareSize 
                || newHeadPosition.Y >= GameCanvas.ActualWidth / SnakeSquareSize)
            {
                EndGame();
                return;
            }

            if(_snake.Count >= 4)
            {
                for(int i = 0;  i < _snake.Count; i++)
                {
                    Point currentPos = new Point(Canvas.GetLeft(_snake[i]), Canvas.GetTop(_snake[i]));

                    for(int j = i + 1; j < _snake.Count; j++)
                    {
                        Point nextPos = new Point(Canvas.GetLeft(_snake[j]), Canvas.GetTop(_snake[j]));

                        if(currentPos == nextPos)
                        {
                            EndGame();
                            return;
                        }
                    }
                }
            }

            for(int i = _snake.Count - 1; i > 0; i--)
            {
                Canvas.SetLeft(_snake[i], Canvas.GetLeft(_snake[i - 1]));
                Canvas.SetTop(_snake[i], Canvas.GetTop(_snake[i - 1]));
            }

            Canvas.SetLeft(_snakeHead, newHeadPosition.X * SnakeSquareSize);
            Canvas.SetTop(_snakeHead, newHeadPosition.Y * SnakeSquareSize);
        }

        public void EndGame()
        {
            _timer.Stop();
            RestartButton.Visibility = Visibility.Visible;
        }

        public void EatFood()
        {
            _score++;
            ScoreTextBlock.Text = "Score: " + _score.ToString();
            GameCanvas.Children.Remove(GameCanvas.Children.OfType<Image>().FirstOrDefault());
            Rectangle newSnake = CreateSnakeSegment(_foodPosition);
            _snake.Add(newSnake);
            GameCanvas.Children.Add(newSnake);
        }

        public Point CalcuteNewHeadPosition()
        {
            double left = Canvas.GetLeft(_snakeHead) / SnakeSquareSize;
            double top = Canvas.GetTop(_snakeHead) / SnakeSquareSize;

            Point headCurrentPosition = new Point(left, top);
            Point newHeadPosition = new Point();

            switch(_direction)
            {
                case Direction.Left:
                    newHeadPosition = new Point(headCurrentPosition.X - 1, headCurrentPosition.Y); 
                    break;
                case Direction.Right:
                    newHeadPosition = new Point(headCurrentPosition.X + 1, headCurrentPosition.Y);
                    break;
                case Direction.Top:
                    newHeadPosition = new Point(headCurrentPosition.X, headCurrentPosition.Y - 1);
                    break;
                case Direction.Bottom:
                    newHeadPosition = new Point(headCurrentPosition.X, headCurrentPosition.Y + 1);
                    break;
            }

            return newHeadPosition;
        }

        public void PlaceFood()
        {
            int maxX = (int)(GameCanvas.ActualWidth / SnakeSquareSize);
            int maxY = (int)(GameCanvas.ActualHeight / SnakeSquareSize);

            int foodX = randomPositionFood.Next(0, maxX);
            int foodY = randomPositionFood.Next(0, maxY);

            _foodPosition = new Point(foodX, foodY);

            Image foodImage = new Image
            {
                Width = SnakeSquareSize, Height = SnakeSquareSize,
                Source = new BitmapImage(new Uri("apple.png", UriKind.Relative))
            };

            Canvas.SetLeft(foodImage, foodX * SnakeSquareSize);
            Canvas.SetTop(foodImage, foodY * SnakeSquareSize);

            GameCanvas.Children.Add(foodImage);
        }

        public Rectangle CreateSnakeSegment(Point position)
        {
            Rectangle rectangle = new Rectangle { 
                Width = SnakeSquareSize, Height = SnakeSquareSize,
                Fill = _snakeColor
            };

            Canvas.SetLeft(rectangle, position.X * SnakeSquareSize);
            Canvas.SetTop(rectangle, position.Y * SnakeSquareSize);

            return rectangle;
        }

        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Up:
                    if(_direction != Direction.Bottom)
                        _direction = Direction.Top; 
                    break;
                case Key.Down:
                    if (_direction != Direction.Top)
                        _direction = Direction.Bottom;
                    break;
                case Key.Left:
                    if (_direction != Direction.Right)
                        _direction = Direction.Left;
                    break;
                case Key.Right:
                    if (_direction != Direction.Left)
                        _direction = Direction.Right;
                    break;
            }
        }

        public void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            _score = 0;
            ScoreTextBlock.Text = "Score: 0";

            GameCanvas.Children.Clear();
            _snake.Clear();

            RestartButton.Visibility = Visibility.Collapsed;
            InitialGame();
        }

        public void StartGame_Click(object sender, RoutedEventArgs e)
        {
            InitialGame();
            StartGame.Visibility = Visibility.Collapsed;
        }
    }
}
