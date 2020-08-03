/*!	\author		Haohan Liu
	\date		2019-04-20
	\file		MainWindow.xaml.cs
*/

using LiteBrite.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LiteBrite.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModelMain viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // center the window
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // assign a new instance of ViewModelMain
            viewModel = new ViewModelMain(this);

            // set Data Binding
            this.DataContext = viewModel;
        }

        // *********************************
        // WINDOW CLOSING EVENT HANDLERS
        // *********************************
        private void ConfirmClosing()
        {
            var result = MessageBox.Show(
                Properties.Resources.msgbox_closenotice,
                Properties.Resources.msgbox_caption_notice,
                MessageBoxButton.OKCancel,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.OK)
            {
                this.Close();
            }
        }

        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    this.ConfirmClosing();
        //}

        private void ApplicationClose(object sender, ExecutedRoutedEventArgs e)
        {
            this.ConfirmClosing();
        }

        /// <summary>
        /// EVERYTHING RELATED TO DRAG_&_DROP
        /// </summary>
        #region DRAG_&_DROP

        private Point startPoint;

        private void ListBoxSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the starting point of initiating the drag
            startPoint = e.GetPosition(null);  // absolute position
        }

        private void ListBoxSource_MouseMove(object sender, MouseEventArgs e)
        {
            // step 1: - detect a drag and drop operation
            // Get the mouse position and difference since starting the drag
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // get the dragged listbox item
                ListBox listBox = sender as ListBox;
                // Use findAncestor to take the OriginalSource, go up the visual tree
                // until a ListBoxItem is hit.  Need a ListBoxItem to get the string
                ListBoxItem listBoxItem = findAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
                if (listBoxItem != null)
                {
                    // Find the data behind the listBoxItem
                    Ellipse theItem = (Ellipse)listBox.ItemContainerGenerator.ItemFromContainer(listBoxItem);
                    // initialize drag and drop
                    // Step 2: create a DataObject containing the string to be "dragged"
                    DataObject dragData = new DataObject(typeof(Ellipse), theItem);
                    // Step 3: initialize the dragging
                    DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
                }
            }
        }

        /*
         * Helper method to search up the VisualTree to identify whether the user 
         * clicked "on" the right type of control. If the control that fired the event
         * isn't the right type then perhaps it's a descendent of another control that 
         * is the right type. The parameter 'current' is any visual component which 
         * extends DependencyObject. The generic argument T is the type of visual component
         * that we're looking for within the VisualTree of our GUI, such as ListBoxItem. 
         * If the specified type of visual component is found within the VisualTree 
         * then an object reference of that type is returned, otherwise null is returned.
         * From: http://wpftutorial.net/DragAndDrop.html
        */
        private static T findAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void Board_Drop(object sender, DragEventArgs e)
        {
            Point points = new Point(0, 0);

            int xAxis = 0;
            // Get x coordinate
            foreach (var column in board.ColumnDefinitions)
            {
                if (e.GetPosition(board).X > column.Offset && e.GetPosition(board).X < (column.Offset + column.ActualWidth))
                {
                    points.X = xAxis;
                    break;
                }
                xAxis++;
            }

            int yAxis = 0;
            // Get Y coordinate
            foreach (var row in board.RowDefinitions)
            {
                if (e.GetPosition(board).Y > row.Offset && e.GetPosition(board).Y < (row.Offset + row.ActualHeight))
                {
                    points.Y = yAxis;
                    break;
                }
                yAxis++;
            }

            if (e.Data.GetDataPresent(typeof(Ellipse)))
            {
                Rectangle ellipse = board.Children.Cast<Rectangle>().First(el => Grid.GetRow(el) == points.Y && Grid.GetColumn(el) == points.X);

                // get info of destination Ellipse
                //Rectangle chosenEllipse = (Rectangle)e.Data.GetData(typeof(Ellipse));

                ellipse.Fill = ((Ellipse)e.Data.GetData(typeof(Ellipse))).Fill;
            }
        }

        #endregion
    }
}
