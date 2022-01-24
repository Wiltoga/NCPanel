using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NCPanel
{
    //https://weblogs.asp.net/akjoshi/Attached-behavior-for-auto-scrolling-containers-while-doing-drag-amp-drop
    public static class DragDropExtension
    {
        #region ScrollOnDragDropProperty

        public static readonly DependencyProperty ScrollOnDragDropProperty =
            DependencyProperty.RegisterAttached("ScrollOnDragDrop",
                typeof(bool),
                typeof(DragDropExtension),
                new PropertyMetadata(false, HandleScrollOnDragDropChanged));

        public static T? GetFirstVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is not null)
                    {
                        if (child is T)
                        {
                            return (T)child;
                        }

                        var childItem = GetFirstVisualChild<T>(child);
                        if (childItem != null)
                        {
                            return childItem;
                        }
                    }
                }
            }

            return null;
        }

        public static bool GetScrollOnDragDrop(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(ScrollOnDragDropProperty);
        }

        public static void SetScrollOnDragDrop(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(ScrollOnDragDropProperty, value);
        }

        private static void HandleScrollOnDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = d as FrameworkElement;

            if (container is null)
            {
                return;
            }

            Unsubscribe(container);

            if (e.NewValue is true)
            {
                Subscribe(container);
            }
        }

        private static void OnContainerPreviewDragOver(object sender, DragEventArgs e)
        {
            var container = sender as FrameworkElement;

            if (container is null)
            {
                return;
            }

            var scrollViewer = GetFirstVisualChild<ScrollViewer>(container);

            if (scrollViewer is null)
            {
                return;
            }

            double tolerance = 50;
            double verticalPos = e.GetPosition(container).Y;
            double offset = 10;

            scrollViewer.UpdateLayout();
            if (verticalPos < tolerance) // Top of visible list?
            {
                var perc = verticalPos / tolerance;
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - perc * offset); //Scroll up.
            }
            else if (verticalPos > container.ActualHeight - tolerance) //Bottom of visible list?
            {
                var perc = (container.ActualHeight - verticalPos) / tolerance;
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + perc * offset); //Scroll down.
            }
        }

        private static void Subscribe(FrameworkElement container)
        {
            container.PreviewDragOver += OnContainerPreviewDragOver;
        }

        private static void Unsubscribe(FrameworkElement container)
        {
            container.PreviewDragOver -= OnContainerPreviewDragOver;
        }

        #endregion ScrollOnDragDropProperty
    }
}