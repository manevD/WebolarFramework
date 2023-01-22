using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace Webolar.Framework
{
    public class ListViewColumns 
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ListViewColumns), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));
        public static bool GetIsEnabled(DependencyObject d) => (bool)d.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(ListView d, bool value) => d.SetValue(IsEnabledProperty, value);

        public static void IsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView list)
            {
                if (!KnownLists.Contains(list))
                {
                    KnownLists.Add(list);
                    list.Unloaded += OnUnloaded;
                    list.SizeChanged += OnSizeChanged;
                }
            }
            else
                throw new Exception("ListView expected");
        }

        public static readonly DependencyProperty RelativeWidthProperty = DependencyProperty.RegisterAttached("RelativeWidth", typeof(double), typeof(ListViewColumns), new FrameworkPropertyMetadata(double.NaN));
        public static double GetWidth(DependencyObject d) => (double)d.GetValue(RelativeWidthProperty);
        public static void SetWidth(GridViewColumn d, double value) => d.SetValue(RelativeWidthProperty, value);

        private static readonly List<FrameworkElement> KnownLists = new();

        private static void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            _ = KnownLists.Remove(element);
            element.Unloaded -= OnUnloaded;
            element.SizeChanged -= OnSizeChanged;
        }

        private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                bool isEnabled = listView.GetValue(IsEnabledProperty) is bool enabled && enabled;
                if (isEnabled && listView.View is GridView gridView)
                {
                    double TotalUnits = gridView.Columns.Sum(column => {
                        double width = (double)column.GetValue(RelativeWidthProperty);
                        return double.IsNaN(width) ? 1 : width;
                    });
                    double ActualWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
                    double UnitWidth = Math.Floor(ActualWidth / TotalUnits);
                    foreach (var column in gridView.Columns)
                    {
                        double unit = (double)column.GetValue(RelativeWidthProperty);
                        if (!double.IsNaN(unit) && UnitWidth != -2)
                            column.Width = unit * UnitWidth;
                    }
                }
            }
        }
    }
}
