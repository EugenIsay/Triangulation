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
        Ellipse Ellipse { get; set; }
        List<Router> routers = new List<Router>();
        Router selectedRouter = new Router();
        public MainWindow()
        {
            InitializeComponent(); 
            AddHandler(DragDrop.DragOverEvent, DragOver);
            for (int i = 0; i < 3; i++)
            {
                Ellipse test = new Ellipse() { Fill = Brush.Parse("Green"), Width = 100, Height = 100, Opacity = 0.5, ZIndex = 3};
                test.PointerPressed += OnPointerPressed;
                test.Tag = i;
                canvas.Children.Add(test);
                routers.Add(new Router() { Id = i, yCoordinate = 0, xCoordinate = 0, Distance = 100 }); ;
            }
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
            selectedRouter = routers.FirstOrDefault(r => r.Id == Int32.Parse(Ellipse.Tag.ToString()));
            var mousePos = e.GetPosition(canvas);
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
                    StartPoint = new Avalonia.Point(router1.xCoordinate + router1.Distance / 2, router1.yCoordinate + router1.Distance / 2),
                    EndPoint = new Avalonia.Point(router2.xCoordinate + router2.Distance / 2, router2.yCoordinate + router2.Distance / 2),
                    Stroke = Brush.Parse("Blue"),
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);
            }

            coord.Text = mousePos.X.ToString() + "|" + mousePos.Y.ToString();
        }
    }
}