using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Triangulation.Models;

namespace Triangulation
{
    public partial class MainWindow : Window
    {
        Ellipse Ellipse { get; set; }
        Router selectedRouter = new Router();

        public MainWindow()
        {
            InitializeComponent();
            AddHandler(DragDrop.DragOverEvent, DragOver);
            // Добавляет начальные круги
            for (int i = 0; i < 3; i++)
            {
                Ellipse router = new Ellipse() { Fill = Brush.Parse("Green"), Width = 200, Height = 200, Opacity = 0.5, ZIndex = 3 };
                router.PointerPressed += OnPointerPressed;
                router.Tag = i;
                canvas.Children.Add(router);
                TData.Routers.Add(new Router() { Id = i, yCoordinate = 0, xCoordinate = 0, Radius = 100, Frequency = float.Parse("2,4")});
                router.DoubleTapped += DoubleTapped;
            }
            // Добавляет датчик
            Ellipse recover = new Ellipse() { Fill = Brush.Parse("Red"), Width = 10, Height = 10, Opacity = 0.5, ZIndex = 5, Tag = -1 };
            recover.PointerPressed += OnPointerPressed;
            canvas.Children.Add(recover);
            TData.Receiver = new Receiver() { yCoordinate = 0, xCoordinate = 0 };
        }

        // Метод, когда пользователь отпускает обьект
        private async void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var mousePos = e.GetPosition(canvas);
            var dragData = new DataObject();
            var result = await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
            Ellipse = null;
            selectedRouter = new Router();
        }

        // Метод, когда пользователь берёт обьект мышкой
        private void DragOver(object? sender, DragEventArgs e)
        {
            // Эта часть кода перемщает обьект по канвасу
            e.DragEffects = DragDropEffects.Move;
            if (Ellipse == null)
                Ellipse = e.Source as Ellipse;
            var mousePos = e.GetPosition(canvas);
            
            if (Int32.Parse(Ellipse.Tag.ToString()) != -1)
            {
                selectedRouter = TData.Routers.FirstOrDefault(r => r.Id == int.Parse(Ellipse.Tag.ToString()));
                if (mousePos.X >= 0)
                {
                    Canvas.SetLeft(Ellipse, mousePos.X - Ellipse.Width / 2);
                    selectedRouter.xCoordinate = ((int)(mousePos.X - Ellipse.Width / 2));
                }
                if (mousePos.Y >= 0)
                {
                    Canvas.SetTop(Ellipse, mousePos.Y - Ellipse.Width / 2);
                    selectedRouter.yCoordinate = ((int)(mousePos.Y - Ellipse.Width / 2));
                }
            }
            else
            {
                if (mousePos.X >= 0)
                {
                    Canvas.SetLeft(Ellipse, mousePos.X - Ellipse.Width / 2);
                    TData.Receiver.xCoordinate = ((int)(mousePos.X - Ellipse.Width / 2));
                }
                if (mousePos.Y >= 0)
                {
                    Canvas.SetTop(Ellipse, mousePos.Y - Ellipse.Width / 2);
                    TData.Receiver.yCoordinate = ((int)(mousePos.Y - Ellipse.Width / 2));
                }
            }
            //Метод, рисующий линии между роутарами и получатеелем.
            DrawLines();
            List<Router> routers = TData.Routers.OrderBy(r => r.Distance).Take(3).OrderBy(r => r.Id).ToList();
            coord.Text = "";
            if (Int32.Parse(Ellipse.Tag.ToString()) != -1)
            {
                coord.Text = mousePos.X.ToString() + "|" + mousePos.Y.ToString();
            }
            else
            {
                coord.Text = Solution(routers).ToString();
            }
            for (int i = 0; i < 3; i++)
            {
                (Bars.Children.ToList()[i] as ProgressBar).Value = routers[i].Coof;
                string d = "Вне зоны доступа";
                if (routers[i].Distance < routers[i].Radius) 
                    d = routers[i].Distance.ToString();
                (Info.Children.ToList()[i] as TextBlock).Text = $" ID вышки: {routers[i].Id}, частота: {routers[i].Frequency}, расстояние до точки: {d}, качество связи:";
            }
        }

        //Метод для расчёта координат получателя
        public (int, int)? Solution(List<Router> routers)
        {
            // Если дистанция больше радиуса, то ничего не возвращает
            for (int i = 0; i < 3; i++)
            {
                if (routers[i].Distance > routers[i].Radius)
                {
                    return (null);
                }
            }
            // Формула расчёта по крамеру
            int A1 = 2 * (routers[0].xReal - routers[1].xReal);
            int B1 = 2 * (routers[0].yReal - routers[1].yReal);
            int C1 = (routers[0].xReal * routers[0].xReal - routers[1].xReal * routers[1].xReal)
                + (routers[0].yReal * routers[0].yReal - routers[1].yReal * routers[1].yReal)
                - (routers[0].Distance * routers[0].Distance - routers[1].Distance * routers[1].Distance);

            int A2 = 2 * (routers[0].xReal - routers[2].xReal);
            int B2 = 2 * (routers[0].yReal - routers[2].yReal);
            int C2 = (routers[0].xReal * routers[0].xReal - routers[2].xReal * routers[2].xReal)
                + (routers[0].yReal * routers[0].yReal - routers[2].yReal * routers[2].yReal)
                - (routers[0].Distance * routers[0].Distance - routers[2].Distance * routers[2].Distance);

            int det = (A1 * B2 - B1 * A2);
            if (det != 0)
            {
                int x = (C1 * B2 - C2 * B1) / det;
                int y = (A1 * C2 - A2 * C1) / det;
                return (x, y);
            }
            return null;
        }

        // Метод, открывающий редактирование для роутера
        private async void DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            int Id = int.Parse((sender as Ellipse).Tag.ToString());
            await new EditRouterWindow(Id).ShowDialog(this);
            if (canvas.Children.Where(e => e.GetType().Name == "Ellipse" && (e as Ellipse).Fill != Brush.Parse("Red")).Count() > TData.Routers.Count() )
            {
                canvas.Children.Remove(canvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == Id));
            }
            else
            {
                Canvas.SetLeft(sender as Ellipse, TData.Routers[Id].xCoordinate); 
                Canvas.SetTop(sender as Ellipse, TData.Routers[Id].yCoordinate);
                (sender as Ellipse).Width = TData.Routers[Id].Radius * 2;
                (sender as Ellipse).Height = TData.Routers[Id].Radius * 2;
            }
            DrawLines();

        }

        // Метод, открывающий добавление роутера
        private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await new EditRouterWindow().ShowDialog(this);
            if (canvas.Children.Where(e => e.GetType().Name == "Ellipse" && (e as Ellipse).Fill != Brush.Parse("Red")).Count() < TData.Routers.Count)
            {
                Router newRouter = TData.Routers.Last();
                Ellipse router = new Ellipse() { Fill = Brush.Parse("Green"), Width = newRouter.Radius*2, Height = newRouter.Radius * 2, Opacity = 0.5, ZIndex = 3, };
                router.PointerPressed += OnPointerPressed;
                router.Tag = newRouter.Id;
                canvas.Children.Add(router);
                Canvas.SetLeft(router, newRouter.xReal);
                Canvas.SetTop(router, newRouter.yReal);
                router.DoubleTapped += DoubleTapped;
            }
            DrawLines();
        }

        // Метод, рисующий линии между роутерами и обьектом, а так же меняет цвет ближайших к получателю роутеры
        public void DrawLines()
        {
            // Сброс линий, а так же цвета всех роутеров 
            foreach (Ellipse ellipse in canvas.Children.Where(e => e.GetType().Name == "Ellipse" && (e as Ellipse).Fill != Brush.Parse("Red")) )
            {
                ellipse.Fill = Brush.Parse("Green");
            }
            canvas.Children.RemoveAll(canvas.Children.Where(c => c.GetType().Name == "Line"));
            // Обновление дистанции до точки
            TData.ChangeDictance();
            // Ближайшие к точке роутеры
            List<Router> routers = TData.Routers.OrderBy(r => r.Distance).Take(3).ToList();
            
            for (int i = 0; i < routers.Count; i++)
            {
                Router router1 = routers[i];
                Router router2;
                if (i + 1 == routers.Count)
                    router2 = routers[0];
                else
                    router2 = routers[i + 1];
                // Линия, которая появляется между роутерами
                Line line = new Line()
                {
                    StartPoint = new Avalonia.Point(router1.xReal, router1.yReal),
                    EndPoint = new Avalonia.Point(router2.xReal, router2.yReal),
                    Stroke = Brush.Parse("Blue"),
                    StrokeThickness = 2,
                    ZIndex = 2
                };
                canvas.Children.Add(line);
                // Линия, идущая  к получателю
                Line lineDot = new Line()
                {
                    StartPoint = new Avalonia.Point(router1.xReal, router1.yReal),
                    EndPoint = new Avalonia.Point(TData.Receiver.xReal, TData.Receiver.yReal),
                    Stroke = Brush.Parse("Blue"),
                    StrokeThickness = 2,
                    ZIndex = 2
                };
                canvas.Children.Add(lineDot);
                // Смена цвета роутера
                Ellipse? ellipse = (canvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == router1.Id) as Ellipse);
                if (router1.Distance < router1.Radius)
                    ellipse.Fill = Brush.Parse("GreenYellow");
                else
                    ellipse.Fill = Brush.Parse("LightGreen");
            }
        }
    }
}