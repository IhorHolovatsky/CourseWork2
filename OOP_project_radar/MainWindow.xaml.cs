using System.Windows;
using OOP_project_radar.Radar;


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

        #endregion

        private void btnChangeLandscape_OnClick(object sender, RoutedEventArgs e)
        {
           _radar.ChangeLandscape();
        }
    }
}
