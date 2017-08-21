using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Desktop.Utilities
{
    public enum NativeHtmlHelpCommand
    {
        Topic = 0,
        TableOfContents = 1,
        Index = 2
    }


    public static class Help
    {
        public static readonly DependencyProperty FilenameProperty =
            DependencyProperty.RegisterAttached("Filename", typeof(string), typeof(Help));

        public static readonly DependencyProperty KeywordProperty = DependencyProperty.RegisterAttached("Keyword",
            typeof(string), typeof(Help));

        static Help()
        {
            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement),
                new CommandBinding(ApplicationCommands.Help, Executed,
                    CanExecute));
        }


        public static string GetFilename(DependencyObject d)
        {
            return (string)d.GetValue(FilenameProperty);
        }

        public static void SetFilename(DependencyObject d, string value)
        {
            d.SetValue(FilenameProperty, value);
        }


        public static string GetKeyword(DependencyObject d)
        {
            return (string)d.GetValue(KeywordProperty);
        }

        public static void SetKeyword(DependencyObject d, string value)
        {
            d.SetValue(KeywordProperty, value);
        }

        private static void CanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            var el = sender as FrameworkElement;
            if (el == null) return;
            var fileName = FindFilename(el);
            if (!string.IsNullOrEmpty(fileName))
                args.CanExecute = true;
        }

        [DllImport("hhctrl.ocx", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern int HtmlHelp(
            HandleRef hwndCaller,
            [MarshalAs(UnmanagedType.LPTStr)] string pszFile,
            NativeHtmlHelpCommand uCommand,
            [MarshalAs(UnmanagedType.LPTStr)] string dwData);

        private static void Executed(object sender, ExecutedRoutedEventArgs args)
        {
            var parent = args.OriginalSource as DependencyObject;
            var keyword = GetKeyword(parent);

            var fName = $"{Environment.CurrentDirectory}\\{FindFilename(parent)}";
            // need to attach it
            var window = Window.GetWindow(parent);
            var h = new HandleRef(null, new IntPtr());
            if (window != null)
            {
                var wih = new WindowInteropHelper(window);
                var hWnd = wih.Handle;
                h = new HandleRef(null, hWnd);
            }

            if (File.Exists(fName))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    HtmlHelp(h, fName, NativeHtmlHelpCommand.Topic, keyword);

                }
                else
                {
                    HtmlHelp(h, fName, NativeHtmlHelpCommand.TableOfContents, "");
                }

            }
            else
            {
                Debug.Print($"No help file found at {fName}");
            }
        }

        private static string FindFilename(DependencyObject sender)
        {
            if (sender == null) return null;
            var fileName = GetFilename(sender);
            if (!string.IsNullOrEmpty(fileName))
                return fileName;
            return FindFilename(VisualTreeHelper.GetParent(sender));
        }


        //public static T TryFindParent<T>(this DependencyObject child)
        //    where T : DependencyObject
        //{
        //    //get parent item
        //    var parentObject = GetParentObject(child);

        //    //we've reached the end of the tree
        //    if (parentObject == null) return null;

        //    //check if the parent matches the type we're looking for
        //    var parent = parentObject as T;
        //    if (parent != null)
        //    {
        //        return parent;
        //    }
        //    //use recursion to proceed with next level
        //    return TryFindParent<T>(parentObject);
        //}

        ///// <summary>
        /////     This method is an alternative to WPF's
        /////     <see cref="VisualTreeHelper.GetParent" /> method, which also
        /////     supports content elements. Keep in mind that for content element,
        /////     this method falls back to the logical tree of the element!
        ///// </summary>
        ///// <param name="child">The item to be processed.</param>
        ///// <returns>
        /////     The submitted item's parent, if available. Otherwise
        /////     null.
        ///// </returns>
        //public static DependencyObject GetParentObject(this DependencyObject child)
        //{
        //    if (child == null) return null;

        //    //handle content elements separately
        //    var contentElement = child as ContentElement;
        //    if (contentElement != null)
        //    {
        //        var parent = ContentOperations.GetParent(contentElement);
        //        if (parent != null) return parent;

        //        var fce = contentElement as FrameworkContentElement;
        //        return fce != null ? fce.Parent : null;
        //    }

        //    //also try searching for parent in framework elements (such as DockPanel, etc)
        //    var frameworkElement = child as FrameworkElement;
        //    if (frameworkElement != null)
        //    {
        //        var parent = frameworkElement.Parent;
        //        if (parent != null) return parent;
        //    }

        //    //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
        //    return VisualTreeHelper.GetParent(child);
        //}
    }
}