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
        string tst = "";
        Ellipse Ellipse { get; set; }
        Router selectedRouter = new Router();
        public MainWindow()
        {
            InitializeComponent();
            AddHandler(DragDrop.DragOverEvent, DragOver);
            for (int i = 0; i < 3; i++)
            {
                Ellipse test = new Ellipse() { Fill = Brush.Parse("Green"), Width = 200, Height = 200, Opacity = 0.5, ZIndex = 3 };
                test.PointerPressed += OnPointerPressed;
                test.Tag = i;
                canvas.Children.Add(test);
                TData.Routers.Add(new Router() { Id = i, yCoordinate = 0, xCoordinate = 0, Radius = 100 });
                test.DoubleTapped += DoubleTapped;
            }
            Ellipse rec = new Ellipse() { Fill = Brush.Parse("Red"), Width = 10, Height = 10, Opacity = 0.5, ZIndex = 4, Tag = -1 };
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
            canvas.Children.RemoveAll(canvas.Children.Where(c => c.GetType().Name == "Line"));
            TData.ChangeDictance();
            for (int i = 0; i < TData.Routers.Count; i++)
            {
                Router router1 = TData.Routers[i];
                Router router2;
                if (i + 1 == TData.Routers.Count)
                    router2 = TData.Routers[0];
                else
                    router2 = TData.Routers[i + 1];
                Line line = new Line()
                {

                    StartPoint = new Avalonia.Point(router1.xReal, router1.yReal),
                    EndPoint = new Avalonia.Point(router2.xReal, router2.yReal),
                    Stroke = Brush.Parse("Blue"),
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);

                tst += TData.Routers[i].Distance + "|";
                Line lineDot = new Line()
                {
                    StartPoint = new Avalonia.Point(router1.xReal, router1.yReal),
                    EndPoint = new Avalonia.Point(TData.Receiver.xCoordinate + 5, TData.Receiver.yCoordinate + 5),
                    Stroke = Brush.Parse("Blue"),
                    StrokeThickness = 2
                };
                canvas.Children.Add(lineDot);
            }
            coord.Text = mousePos.X.ToString() + "|" + mousePos.Y.ToString();
            coord2.Text = string.Join(" ", Solution(TData.Routers));
            tst = "";
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
            int A1 = 2 * (routers[0].xReal  - routers[1].xReal);
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
            await new EditRouterWindow(int.Parse((sender as Ellipse).Tag.ToString())).ShowDialog(this);
        }
    }
}