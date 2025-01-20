using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Triangulation.Models;

namespace Triangulation
{
    public partial class MainWindow : Window
    {
        string tst = "";
        Ellipse Ellipse { get; set; }
        List<Router> routers = new List<Router>();
        Receiver receiver = new Receiver();
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
                routers.Add(new Router() { Id = i, yCoordinate = 0, xCoordinate = 0, Radius = 200 }); ;
            }
            Ellipse rec = new Ellipse() { Fill = Brush.Parse("Red"), Width = 10, Height = 10, Opacity = 0.5, ZIndex = 4, Tag = -1 };
            rec.PointerPressed += OnPointerPressed;
            canvas.Children.Add(rec);
            receiver = new Receiver() { yCoordinate = 0, xCoordinate = 0 };
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
                selectedRouter = routers.FirstOrDefault(r => r.Id == Int32.Parse(Ellipse.Tag.ToString()));
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
                    receiver.xCoordinate = ((int)(mousePos.X - Ellipse.Width / 2));
                }
                if (mousePos.Y >= 0)
                {
                    Canvas.SetTop(Ellipse, mousePos.Y - Ellipse.Width / 2);
                    receiver.yCoordinate = ((int)(mousePos.Y - Ellipse.Width / 2));
                }
            }
            canvas.Children.RemoveAll(canvas.Children.Where(c => c.GetType().Name == "Line"));
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

                    StartPoint = new Avalonia.Point(router1.xCoordinate + router1.Radius / 2, router1.yCoordinate + router1.Radius / 2),
                    EndPoint = new Avalonia.Point(router2.xCoordinate + router2.Radius / 2, router2.yCoordinate + router2.Radius / 2),
                    Stroke = Brush.Parse("Blue"),
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);
                routers[i].Distance = (int)Math.Sqrt(Math.Pow((routers[i].xCoordinate + router1.Radius / 2) - (receiver.xCoordinate + 10 / 2), 2) + Math.Pow((routers[i].yCoordinate + router2.Radius / 2) - (receiver.yCoordinate + 10 / 2), 2));
                tst += routers[i].Distance + "|";
                Line lineDot = new Line()
                {

                    StartPoint = new Avalonia.Point(router1.xCoordinate + router1.Radius / 2, router1.yCoordinate + router1.Radius / 2),
                    EndPoint = new Avalonia.Point(receiver.xCoordinate + 10 / 2, receiver.yCoordinate + 10 / 2),
                    Stroke = Brush.Parse("Blue"),
                    StrokeThickness = 2
                };
                canvas.Children.Add(lineDot);
            }
            coord.Text = mousePos.X.ToString() + "|" + mousePos.Y.ToString();
            coord2.Text = string.Join(" ", Solution( routers[0].xCoordinate, routers[1].xCoordinate, routers[2].xCoordinate, 
                routers[0].yCoordinate, routers[1].yCoordinate, routers[2].yCoordinate,
                routers[0].Distance, routers[1].Distance, routers[2].Distance));
            tst = "";
        }


        public (double, double) Solution(int x1, int x2, int x3, int y1, int y2, int y3, int d1, int d2, int d3)
        {

            return ();
        }
    }
}