/*!	\author		Haohan Liu
	\date		2019-04-20
	\file		ViewModelMain.cs
*/

using LiteBrite.Helpers;
using LiteBrite.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LiteBrite.ViewModel
{
    public class ViewModelMain : DependencyObject
    {
        MainWindow main;
        // list of colors
        List<string> myColors;

        /// <summary>
        /// RELAY COMMAND SECTION
        /// </summary>
        #region RelayCommand

        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand NewGameCommand { get; set; }
        public RelayCommand RandomCommand { get; set; }
        public RelayCommand LightUpCommand { get; set; }
        public RelayCommand NormalCommand { get; set; }
        public RelayCommand AboutCommand { get; set; }

        #endregion

        // ctor 
        public ViewModelMain(MainWindow m)
        {
            main = m;
            myColors = new List<string>();

            GenerateColorPanel();
            GenerateBoard();

            // Initializations for RelayCommands
            // FILE
            OpenFileCommand = new RelayCommand(OpenFile);
            SaveCommand = new RelayCommand(SaveFile);
            //ExitCommand = new RelayCommand(ExitProgram);
            // GAME
            NewGameCommand = new RelayCommand(StartNewGame);
            RandomCommand = new RelayCommand(RandomGame);
            // LIGHT
            LightUpCommand = new RelayCommand(LightUpBoard);
            NormalCommand = new RelayCommand(PaintBoardNormal);
            // HELP
            AboutCommand = new RelayCommand(DisplayAbout);
        }

        // *********************************
        // RELAY COMMAND  METHODS
        // *********************************

        private void OpenFile(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            openFileDialog.Filter = "Text File Format |*.txt";

            if(openFileDialog.ShowDialog() == true)
            {
                foreach (var rectangle in File.ReadAllLines(openFileDialog.FileName).ToList())
                {
                    string[] rectangleProps = rectangle.Split(',');

                    // Rectangle prop set up
                    main.board.Children
                        .Cast<Rectangle>()
                        .First(r => Grid.GetColumn(r) == Convert.ToInt32(rectangleProps[0]) && Grid.GetRow(r) == Convert.ToInt32(rectangleProps[1]))
                        .Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(rectangleProps[2]));
                }
            }
        }

        private void SaveFile(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            saveFileDialog.Filter = "Text File Format |*.txt";

            if (saveFileDialog.ShowDialog() == true)
            {
                // Column,Row,Color
                List<string> rectangles = new List<string>();

                foreach (Rectangle rectangle in main.board.Children)
                {
                    rectangles.Add($"{Grid.GetColumn(rectangle)},{Grid.GetRow(rectangle)},{rectangle.Fill.ToString()}");
                }

                File.WriteAllLines(saveFileDialog.FileName, rectangles);
            }

        }

        //private void ExitProgram(object obj)
        //{
        //    var result = MessageBox.Show(
        //        "Would you like to save your work?",
        //        "Notice",
        //        MessageBoxButton.YesNo,
        //        MessageBoxImage.Information);

        //    if (result == MessageBoxResult.Yes)
        //    {
        //        SaveFile(obj);
        //    }

        //    // exit program
        //    main.Close();
        //}

        private void StartNewGame(object obj)
        {
            var result = MessageBox.Show(
                Properties.Resources.msgbox_newgamenotice, 
                Properties.Resources.msgbox_caption_notice,
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                SaveFile(obj);
            }

            // reset the draw board
            foreach (Rectangle rectangle in main.board.Children)
            {
                rectangle.Fill = new SolidColorBrush(Colors.MintCream);
            }
        }
        
        private void RandomGame(object obj)
        {
            Random rnd = new Random();

            // fill the board with random colors 
            foreach (Rectangle rectangle in main.board.Children)
            {
                rectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(myColors[rnd.Next(myColors.Count())]));
            }
        }

        private void LightUpBoard(object obj)
        {
            foreach (Rectangle rectangle in main.board.Children)
            {
                rectangle.Stroke = new SolidColorBrush(Colors.Black);

                // this will only light up those rectangles which have not been painted
                if (rectangle.Fill.ToString() == (Colors.MintCream).ToString())
                {
                    rectangle.Fill = new SolidColorBrush(Colors.BlanchedAlmond);
                }
            }
        }

        private void PaintBoardNormal(object obj)
        {
            foreach (Rectangle rectangle in main.board.Children)
            {
                rectangle.Stroke = new SolidColorBrush(Colors.DimGray);

                // this will only light up those rectangles which have not been painted
                if (rectangle.Fill.ToString() == (Colors.MintCream).ToString() || rectangle.Fill.ToString() == (Colors.BlanchedAlmond).ToString())
                {
                    rectangle.Fill = new SolidColorBrush(Colors.MintCream);
                }
            }
        }

        private void DisplayAbout(object obj)
        {
            MessageBox.Show(
                "INFO5102 GUI Development Project#3\n\nName:\tLite Brite\n\nAuthor:\tHaohan Liu\n\nDate:\tApril 20, 2019", 
                Properties.Resources.msgbox_caption_about, 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }

        // *********************************
        // HELPER METHODS
        // *********************************

        private void GenerateColorPanel()
        {
            ObservableCollection<Ellipse> allColorList = new ObservableCollection<Ellipse>();
            List<string> allColors = new List<string>();

            // color type
            Type colorType = typeof(Colors);
            // colors info
            PropertyInfo[] colors = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo c in colors)
            {
                allColors.Add(c.ToString());
            }

            // Create several Ellipse objs
            // store them in colorList
            foreach (string color in allColors)
            {
                Ellipse ellipse = new Ellipse();

                // ellipse prop set-up
                ellipse.Stroke = new SolidColorBrush(Colors.DarkSlateGray);
                ellipse.StrokeThickness = 3;
                ellipse.Height = 50;
                ellipse.Width = 50;
                ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color.Replace("System.Windows.Media.Color", "")));

                // fill up myColor
                myColors.Add(color.Replace("System.Windows.Media.Color", ""));

                allColorList.Add(ellipse);
            }

            colorList = allColorList;
        }

        private void GenerateBoard()
        {
            for (int i = 0; i < 50; i++)
            {
                main.board.RowDefinitions.Add(new RowDefinition());
                main.board.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    Rectangle rectangle = new Rectangle();

                    // set loc
                    Grid.SetColumn(rectangle, i);
                    Grid.SetRow(rectangle, j);

                    rectangle.Stroke = new SolidColorBrush(Colors.DimGray);
                    rectangle.StrokeThickness = 1;

                    rectangle.Fill = new SolidColorBrush(Colors.MintCream);

                    // add to board
                    main.board.Children.Add(rectangle);
                }
            }
        }
        
        // *********************************
        // PROP DEPENDENCIES
        // *********************************

        // color list
        public ObservableCollection<Ellipse> colorList
        {
            get { return (ObservableCollection<Ellipse>)GetValue(colorListProperty); }
            set { SetValue(colorListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for colorList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty colorListProperty =
            DependencyProperty.Register("colorList", typeof(ObservableCollection<Ellipse>), typeof(ViewModelMain), new PropertyMetadata(null));
        
    }
}
