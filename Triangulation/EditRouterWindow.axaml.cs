using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Xml.Linq;
using Triangulation.Models;

namespace Triangulation;

public partial class EditRouterWindow : Window
{
    int x = 0, y = 0;
    int Id = -1;
    // ������������� ��� ����������
    public EditRouterWindow()
    {
        InitializeComponent(); Frequency.SelectedIndex = 0;

    }
    // ������������� ��� ��������������
    public EditRouterWindow(int id)
    {
        InitializeComponent();
        if (TData.Routers.Count > 3)
        {
            Delete.IsVisible = true;
        }
        Router router = new Router();
        Id = id;
        router = TData.Routers[id];
        List<string> strings = Frequency.Items.Select(s => (s as ComboBoxItem).Content.ToString()).ToList();
        Frequency.SelectedIndex = strings.IndexOf($"{router.Frequency}");
        xText .Text = router.xReal.ToString();
        yText.Text = router.yReal.ToString();
        RadiusText.Text = router.Radius.ToString();
        ShowEll.Width = router.Radius * 2;
        ShowEll.Height = router.Radius * 2; ;
    }

    // ����� ����������� ��� ������������ ��������� ������ ������
    private void TextBox_Radius(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (int.TryParse((sender as TextBox).Text, out int result) && result >= 1)
        {
            ShowEll.Width = result * 2;
            ShowEll.Height = result * 2;
        }
        else
        {
            (sender as TextBox).Text = "1";
        }
    }
    // ����� �����������, ��� x � y ��������� ��������
    private void TextBox_Coordinates(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (int.TryParse((sender as TextBox).Text, out int result) && result >= 0)
        {
            if ((sender as TextBox).Name == "xText")
                x = (int)result;
            else
                y = (int)result;
        }
        else
            (sender as TextBox).Text = "0";
    }
    // ������ ������������� ���������
    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Router router = new Router();
        if (Id != -1)
        {
            router = TData.Routers[Id];
            router.Frequency = float.Parse((Frequency.SelectedItem as ComboBoxItem).Content.ToString());
            router.xReal = x;
            router.yReal = y;
            router.Radius = int.Parse(RadiusText.Text);
            TData.Routers[Id] = router;
        }
        else
        {
            router.Id = TData.Routers.Last().Id + 1;
            router.Frequency = float.Parse((Frequency.SelectedItem as ComboBoxItem).Content.ToString());
            router.xReal = x;
            router.yReal = y;
            router.Radius = int.Parse(RadiusText.Text);
            TData.Routers.Add(router);
        }
        this.Close();
    }


    // ����� ���������� �� �������� ������� �� ������
    private void DeleteButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TData.Routers.Remove(TData.Routers.FirstOrDefault(r => r.Id == Id)); 
        this.Close();
    }
}