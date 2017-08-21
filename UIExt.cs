using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Callidex
{
    public static class UIExt
    {
        private static Action EmptyDelegate = delegate() { };

        public static void Refresh(this UIElement ui)
        {
            ui.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);

        }
    }
}
