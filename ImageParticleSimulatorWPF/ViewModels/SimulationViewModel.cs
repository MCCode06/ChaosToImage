using ImageParticleSimulatorWPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

public class BallData
{
    public Vector InitialVelocity { get; set; }
    public Color Color { get; set; }
    public Point FinalPosition {  get; set; }
}

public class SimulationViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Ball> Balls { get; } = new ObservableCollection<Ball>();

    private DispatcherTimer _timer;
    private Random _rand = new();

    private DispatcherTimer _stopTimer;
    private bool _stopTimerStarted = false;

    private double _width;
    private double _height;
    private BitmapImage _image;

    private int _totalBallsToFire;
    private int _ballsFired;
    private int _spawnTickCounter = 1;

    private bool _isRecordingPhase = true;
    private List<BallData> _recordedData = new();


    public SimulationViewModel(double width, double height, int ballCount, BitmapImage image)
    {
        _width = width;
        _height = height;
        _image = image;

        _totalBallsToFire = ballCount;
        _ballsFired = 0;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _timer.Tick += Tick;
        _timer.Start();
    }

    private void Tick(object? sender, EventArgs e)
    {
        if (_isRecordingPhase)
        {
            for (int i = 0; i < _spawnTickCounter && _ballsFired < _totalBallsToFire; i++)
            {
                FireNewBall();
                _ballsFired++;
            }

            _spawnTickCounter++;

            UpdateBalls();

            if (_ballsFired >= _totalBallsToFire && !_stopTimerStarted)
            {
                _stopTimerStarted = true;

                _stopTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(5)
                };
                _stopTimer.Tick += (s, args) =>
                {
                    _stopTimer.Stop();

                    if (_isRecordingPhase)
                    {
                        AssignColorsFromImage();
                        PrepareReplay();
                    }
                    else
                    {
                        _timer.Stop();
                    }
                };
                _stopTimer.Start();
            }
        }
        else
        {
            ReplayBalls();
            UpdateBalls();

            if (_ballsFired >= _recordedData.Count && !_stopTimerStarted)
            {
                _stopTimerStarted = true;

                _stopTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(5)
                };
                _stopTimer.Tick += (s, args) =>
                {
                    _stopTimer.Stop();
                    _timer.Stop(); 
                };
                _stopTimer.Start();
            }
        }
    }


    private void FireNewBall()
    {
        Point center = new Point(_width / 2, _height / 2);
        double angle = _rand.NextDouble() * 2 * Math.PI;

        double progress = (double)_ballsFired / _totalBallsToFire;
        double baseSpeed = 18.0;
        double minSpeed = 2.0;
        double speed = baseSpeed * (1.0 - progress) + minSpeed * progress;

        Vector velocity = new Vector(Math.Cos(angle), Math.Sin(angle)) * speed;

        Balls.Add(new Ball
        {
            Position = center,
            Velocity = velocity,
            FiredVelocity = velocity,
            Radius = 8,
            Color = Colors.White
        });
    }

    private void AssignColorsFromImage()
    {
        var wb = new WriteableBitmap(_image);
        int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 6);
        byte[] pixels = new byte[wb.PixelHeight * stride];
        wb.CopyPixels(pixels, stride, 0);

        _recordedData.Clear();

        foreach (var ball in Balls)
        {
            double normalizedX = Math.Clamp(ball.Position.X / _width, 0, 1);
            double normalizedY = Math.Clamp(ball.Position.Y / _height, 0, 1);

            int imageX = (int)(normalizedX * (wb.PixelWidth - 1));
            int imageY = (int)(normalizedY * (wb.PixelHeight - 1));
            int pixelIndex = imageY * stride + imageX * 4;

            Color color = Colors.White;
            if (pixelIndex + 3 < pixels.Length)
            {
                byte b = pixels[pixelIndex];
                byte g = pixels[pixelIndex + 1];
                byte r = pixels[pixelIndex + 2];
                byte a = pixels[pixelIndex + 3];

                color = Color.FromArgb(a, r, g, b);
            }

            _recordedData.Add(new BallData
            {
                InitialVelocity = ball.FiredVelocity,
                FinalPosition = ball.Position,
                Color = color
            });
        }
    }


    private void PrepareReplay()
    {
        _isRecordingPhase = false;
        _ballsFired = 0;
        _stopTimerStarted = false;
        _spawnTickCounter = 1;
        Balls.Clear();
    }

    private void ReplayBalls()
    {
        for (int i = 0; i < _spawnTickCounter && _ballsFired < _recordedData.Count; i++)
        {
            var data = _recordedData[_ballsFired];

            Balls.Add(new Ball
            {
                Position = new Point(_width / 2, _height / 2),
                Velocity = data.InitialVelocity,
                Radius = 8,
                Color = data.Color
            });

            _ballsFired++;
        }

        _spawnTickCounter++;
    }

    private void UpdateBalls()
    {
        foreach (var ball in Balls)
        {
            ball.Position += ball.Velocity;
            ball.Velocity *= 0.95;

            if (ball.Position.X - ball.Radius <= 0)
            {
                ball.Position = new Point(ball.Radius, ball.Position.Y);
                ball.Velocity = new Vector(-ball.Velocity.X, ball.Velocity.Y);
            }
            else if (ball.Position.X + ball.Radius >= _width)
            {
                ball.Position = new Point(_width - ball.Radius, ball.Position.Y);
                ball.Velocity = new Vector(-ball.Velocity.X, ball.Velocity.Y);
            }

            if (ball.Position.Y - ball.Radius <= 0)
            {
                ball.Position = new Point(ball.Position.X, ball.Radius);
                ball.Velocity = new Vector(ball.Velocity.X, -ball.Velocity.Y);
            }
            else if (ball.Position.Y + ball.Radius >= _height)
            {
                ball.Position = new Point(ball.Position.X, _height - ball.Radius);
                ball.Velocity = new Vector(ball.Velocity.X, -ball.Velocity.Y);
            }
        }

        int passes = 3;
        for (int pass = 0; pass < passes; pass++)
        {
            for (int i = 0; i < Balls.Count; i++)
            {
                for (int j = i + 1; j < Balls.Count; j++)
                {
                    var a = Balls[i];
                    var b = Balls[j];

                    Vector delta = b.Position - a.Position;
                    double distance = delta.Length;
                    double minDistance = a.Radius + b.Radius;

                    if (distance < minDistance && distance > 0.0001)
                    {
                        Vector normal = delta / distance;
                        double overlap = (minDistance - distance);

                        // Push balls apart
                        a.Position -= normal * (overlap / 2);
                        b.Position += normal * (overlap / 2);

                        // Elastic velocity exchange (1D on collision axis)
                        Vector relativeVelocity = b.Velocity - a.Velocity;
                        double velAlongNormal = Vector.Multiply(relativeVelocity, normal);

                        if (velAlongNormal > 0)
                            continue;

                        double restitution = 1.0; // perfectly elastic

                        double impulse = -(1 + restitution) * velAlongNormal / 2;

                        Vector impulseVector = impulse * normal;

                        a.Velocity -= impulseVector;
                        b.Velocity += impulseVector;
                    }
                }
            }
        }

        OnPropertyChanged(nameof(Balls));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
