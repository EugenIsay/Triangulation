using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
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
            for (int i = 0; i < 10; i++)
            {
                Ellipse test = new Ellipse() { Fill = Brush.Parse("Green"), Width = 200, Height = 200, Opacity = 0.5, ZIndex = 3 };
                test.PointerPressed += OnPointerPressed;
                test.Tag = i;
                canvas.Children.Add(test);
                TData.Routers.Add(new Router() { Id = i, yCoordinate = 0, xCoordinate = 0, Radius = 100 });
                test.DoubleTapped += DoubleTapped;
            }
            Ellipse rec = new Ellipse() { Fill = Brush.Parse("Red"), Width = 10, Height = 10, Opacity = 0.5, ZIndex = 5, Tag = -1 };
            rec.PointerPressed += OnPointerPressed;
            canvas.Children.Add(rec);
            TData.Receiver = new Receiver() { yCoordinate = 0, xCoordinate = 0 };
        }

        private async void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var mousePos = e.GetPosition(canvas);

            var dragData = new DataObject();
            var result = await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
            Ellipse = null;
            selectedRouter = new Router();
        }

        private void DragOver(object? sender, DragEventArgs e)
        {
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
            DrawLines();
            coord.Text = mousePos.X.ToString() + "|" + mousePos.Y.ToString();
            List<Router> routers = TData.Routers.OrderBy(r => r.Distance).Take(3).ToList();
            coord2.Text = string.Join(" ", Solution(routers));
            for (int i = 0; i < 3; i++)
            {
                (Bars.Children.ToList()[i] as ProgressBar).Value = routers[i].Coof;
            }
        }

        public (int, int)? Solution(List<Router> routers)
        {
            for (int i = 0; i < 3; i++)
            {
                if (routers[i].Distance > routers[i].Radius)
                {
                    return (null);
                }
            }
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

        private async void DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            int Id = int.Parse((sender as Ellipse).Tag.ToString());
            await new EditRouterWindow(Id).ShowDialog(this);

            Canvas.SetLeft(sender as Ellipse, TData.Routers[Id].xCoordinate);
            Canvas.SetTop(sender as Ellipse, TData.Routers[Id].yCoordinate);
            (sender as Ellipse).Width = TData.Routers[Id].Radius * 2;
            (sender as Ellipse).Height = TData.Routers[Id].Radius * 2;
        }

        private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            new EditRouterWindow().Show();
        }
        public void DrawLines()
        {
            foreach (Ellipse ellipse in canvas.Children.Where(e => e.GetType().Name == "Ellipse" && (e as Ellipse).Fill != Brush.Parse("Red")) )
            {
                ellipse.Fill = Brush.Parse("Green");
            }
            canvas.Children.RemoveAll(canvas.Children.Where(c => c.GetType().Name == "Line"));
            TData.ChangeDictance();
            List<Router> routers = TData.Routers.OrderBy(r => r.Distance).Take(3).ToList();
            for (int i = 0; i < routers.Count; i++)
            {
                Router router1 = routers[i];
                Router router2;
                if (i + 1 == routers.Count)
                    router2 = routers[0];
                else
                    router2 = routers[i + 1];
                Line line = new Line()
                {
                    StartPoint = new Avalonia.Point(router1.xReal, router1.yReal),
                    EndPoint = new Avalonia.Point(router2.xReal, router2.yReal),
                    Stroke = Brush.Parse("Blue"),
                    StrokeThickness = 2,
                    ZIndex = 2
                };
                canvas.Children.Add(line);
                Line lineDot = new Line()
                {
                    StartPoint = new Avalonia.Point(router1.xReal, router1.yReal),
                    EndPoint = new Avalonia.Point(TData.Receiver.xReal, TData.Receiver.yReal),
                    Stroke = Brush.Parse("Blue"),
                    StrokeThickness = 2,
                    ZIndex = 2
                };
                canvas.Children.Add(lineDot);

                (canvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == router1.Id) as Ellipse).Fill = Brush.Parse("GreenYellow");
            }
        }
    }
}