using System.Windows;

namespace Desktop.Utilities
{
    public static class ConditionalDockingHelper
    {
        public static readonly DependencyProperty PaneTypeProperty =
            DependencyProperty.RegisterAttached("PaneType", typeof (PaneType), typeof (ConditionalDockingHelper),
                new PropertyMetadata(PaneType.Normal));

        public static PaneType GetPaneType(DependencyObject obj)
        {
            return (PaneType) obj.GetValue(PaneTypeProperty);
        }

        public static void SetPaneType(DependencyObject obj, PaneType value)
        {
            obj.SetValue(PaneTypeProperty, value);
        }
    }
}