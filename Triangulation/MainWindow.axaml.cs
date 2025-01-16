using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using System.Collections.Generic;

namespace Triangulation
{
    public partial class MainWindow : Window
    {
        Ellipse Ellipse { get; set; }
        public MainWindow()
        {
            InitializeComponent(); 
            AddHandler(DragDrop.DragOverEvent, DragOver);
        }
        private async void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var mousePos = e.GetPosition(canvas);
            var dragData = new DataObject();
            var result = await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        }
        private void DragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects = DragDropEffects.Move;
            if (e.Source.GetType().Name != "Canvas")
                Ellipse = e.Source as Ellipse;
            var mousePos = e.GetPosition(canvas);
            Canvas.SetLeft(Ellipse, mousePos.X - Ellipse.Width/2);
            Canvas.SetTop(Ellipse, mousePos.Y - Ellipse.Width / 2);

            coord.Text = mousePos.X.ToString() + "|" + mousePos.Y.ToString();
        }
    }
}