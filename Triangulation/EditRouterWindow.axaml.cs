using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Diagnostics.Metrics;
using System.Xml.Linq;
using Triangulation.Models;

namespace Triangulation;

public partial class EditRouterWindow : Window
{
    Router router = new Router();
    public EditRouterWindow()
    {
        InitializeComponent();
    }
    public EditRouterWindow(int id)
    {
        InitializeComponent();
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
                router.xReal = (int)result;
            }
            else
            {
                router.yReal = (int)result;
            }
            TData.ChangeDictance();
            DistanceText.Text = router.Distance.ToString();
        }
        else
        {
            (sender as TextBox).Text = "0";
        }
    }
}