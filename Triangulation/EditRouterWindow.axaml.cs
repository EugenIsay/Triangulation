using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics.Metrics;
using System.Xml.Linq;
using Triangulation.Models;

namespace Triangulation;

public partial class EditRouterWindow : Window
{
    int x = 0, y = 0;
    int Id = -1;
    public EditRouterWindow()
    {
        InitializeComponent();
    }
    public EditRouterWindow(int id)
    {
        InitializeComponent();
        Router router = new Router();
        Id = id;
        router = TData.Routers[id];
        xText .Text = router.xReal.ToString();
        yText.Text = router.yReal.ToString();
        RadiusText.Text = router.Radius.ToString();
        ShowEll.Width = router.Radius * 2;
        ShowEll.Height = router.Radius * 2; ;
    }

    private void TextBox_Radius(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (int.TryParse((sender as TextBox).Text, out int result))
        {
            ShowEll.Width = result * 2;
            ShowEll.Height = result * 2;
        }
        else
        {
            (sender as TextBox).Text = "0";
        }
    }

    private void TextBox_Coordinates(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (int.TryParse((sender as TextBox).Text, out int result))
        {
            if ((sender as TextBox).Name == "xText")
            {
                x = (int)result;
            }
            else
            {
                y = (int)result;
            }
            TData.ChangeDictance();
            DistanceText.Text = ((int)Math.Sqrt(Math.Pow((x) - (TData.Receiver.xReal), 2) + Math.Pow((y) - (TData.Receiver.yReal), 2))).ToString();
        }
        else
        {
            (sender as TextBox).Text = "0";
        }
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Frequency.Text))
        {
            Frequency.Text = "0";
        }
        Router router = new Router();
        if (Id != -1)
        {
            router = TData.Routers[Id];
            router.xReal = x;
            router.yReal = y;
            router.Distance = int.Parse(DistanceText.Text);
            router.Frequency = int.Parse(Frequency.Text);
            router.Radius = int.Parse(RadiusText.Text);
            TData.Routers[Id] = router;
        }
        else
        {
            router.xReal = x;
            router.yReal = y;
            router.Distance = int.Parse(DistanceText.Text);
            router.Frequency = int.Parse(Frequency.Text);
            router.Radius = int.Parse(RadiusText.Text);
            TData.Routers.Add(router);
        }
        this.Close();
    }

    private void TextBox_Frequency(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (!float.TryParse((sender as TextBox).Text, out float result))
        {
            (sender as TextBox).Text = "0";
        }
    }
}