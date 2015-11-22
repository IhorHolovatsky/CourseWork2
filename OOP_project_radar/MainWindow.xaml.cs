using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using OOP_project_radar.Radar;
using MessageBox = System.Windows.MessageBox;


namespace OOP_project_radar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Radar.Radar _radar;

        public MainWindow()
        {
            InitializeComponent();

            //LandscapeProvider.CreateLandscape();
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            ImageBox.Source = null;
            _radar = new Radar.Radar(ImageBox);
            _radar.ReloadRadar(null, null);

            Slider.ValueChanged += _radar.Speed_OnValueChangedHandler;
        }


        #region BUTTONS handlers

        private void btnRevert_OnClick(object sender, RoutedEventArgs e)
        {
            _radar.Revert();
        }

        private void btnStart_OnClick(object sender, RoutedEventArgs e)
        {
            _radar.Start();
        }

        private void btnRestart_OnClick(object sender, RoutedEventArgs e)
        {
            _radar.Restart();
        }

        private void btnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(tb_X.Text) || String.IsNullOrEmpty(tb_X.Text))
            {
                var rand = new Random();

                _radar.AddTarget(rand.Next(50, 550), rand.Next(50, 550), ColorPicker.SelectedColor, ColorPicker.SelectedColorText);
            
            }
            else
            {
                _radar.AddTarget(Int32.Parse(tb_X.Text), Int32.Parse(tb_Y.Text), ColorPicker.SelectedColor, ColorPicker.SelectedColorText);
            }
            targetGrid.ItemsSource = null;
            targetGrid.ItemsSource = _radar.Targets;

        }


        private void btnRemove_OnClick(object sender, RoutedEventArgs e)
        {
            var items = targetGrid.SelectedItems;

            foreach (TargetProvider item in items)
            {
                _radar.RemoveTarget(item);
            }

            targetGrid.ItemsSource = null;
            targetGrid.ItemsSource = _radar.Targets;

        }

        #endregion

        private void btnChangeLandscape_OnClick(object sender, RoutedEventArgs e)
        {
            _radar.ChangeLandscape();
        }

        private void TextBoxPasting(object sender, TextCompositionEventArgs e)
        {
           if (!IsTextAllowed(e.Text))
            {
                e.Handled = true;
            }
        }
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
    }
}
