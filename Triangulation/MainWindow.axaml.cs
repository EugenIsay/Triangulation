using Avalonia.Controls;
using Avalonia.Input;
using System.Collections.Generic;

namespace Triangulation
{
    public partial class MainWindow : Window
    {
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
            var mousePos = e.GetPosition(canvas);
            Canvas.SetLeft(First, mousePos.X);
            Canvas.SetTop(First, mousePos.Y);
            coord.Text = mousePos.X.ToString() + "|" + mousePos.Y.ToString();
        }
    }
}