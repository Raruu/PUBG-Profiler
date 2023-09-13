using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.ComponentModel;
using MaterialDesignThemes.Wpf;

namespace PUBG_Profiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Content.MainContent mainContent;
        private Content.TuneContent tuneContent;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadProperty();
            mainContent.ApplyBtnBadge.BadgeChanged += (s, e) => { LoadProperty(); };
            snackbarMessage = main_SnackBar.Message;
            SnackBar_Time = 0;
            
        }


        #region StatusBar
        private void OpenGameSettingDirectory(object sender, RoutedEventArgs e) => System.Diagnostics.Process.Start("explorer.exe", PUBG_Profiler.Content.MainContent.GameDir);
        private void ShutDownApp(object sender, RoutedEventArgs e) => this.Close();
        private void Grid_DragMove(object sender, MouseButtonEventArgs e) => DragMove();
        private void ColorBtn_MouseLeave(object sender, MouseEventArgs e) => (sender as Button).Background = Brushes.Transparent;
        private void ColorBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            ColorAnimation animation = new(Colors.Gray, new Duration(TimeSpan.FromMilliseconds(256)));
            button.Background = new SolidColorBrush();
            button.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }
        private void ColorBtn_MouseEnter_Red(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            ColorAnimation animation = new(Colors.Red, new Duration(TimeSpan.FromMilliseconds(256)));
            button.Background = new SolidColorBrush();
            button.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                BorderThickness = new Thickness(0);
                ic_MaxNorWindow.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowMaximize;
            }
            else
            {
                BorderThickness = new Thickness(7);
                ic_MaxNorWindow.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowRestore;
            }
        }
        private void MaxNor_Window(object sender, RoutedEventArgs e)
        {
            //MaxHeight -= SystemParameters.WorkArea.Height;
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            { 
                WindowState = WindowState.Normal;
            }
        }
        private void Minimizing_Window(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        #endregion

        #region SnackBar
        public int SnackBar_Time { get; set; }
        public Thread SnackBar_Hiding { get; set; }
        public MaterialDesignThemes.Wpf.SnackbarMessage snackbarMessage { get; set; }
        public void HideSnackBar()
        {
            while (SnackBar_Time > 0)
            {
                Thread.Sleep(1000);
                SnackBar_Time--;
            }            
            main_SnackBar.Dispatcher.Invoke(() => { main_SnackBar.IsActive = false; });            
        }
        #endregion

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            if((sender as Button) == BtnMenu_Close)
            {
                BtnMenu_Close.Visibility = Visibility.Collapsed;
                BtnMenu_Open.Visibility = Visibility.Visible;
            }
            else
            {
                BtnMenu_Close.Visibility = Visibility.Visible;
                BtnMenu_Open.Visibility = Visibility.Collapsed;
            }
        }

        public string FileName => mainContent.ComboPresets.Text;
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Stopwatch stopwatch = new();
            stopwatch.Start();
            theContent.Children.Clear();            
            switch (((sender as ListView).SelectedItem as ListBoxItem).Name)
            {
                case "Home_ItemList":
                    if (mainContent == null) mainContent = new(this);
                    theContent.Children.Add(mainContent);
                    //theContent.Content = mainContent;
                    break;
                case "Tune_ItemList":
                    if (tuneContent == null) tuneContent = new(this);
                    if (tuneContent.CurrentFileName.Text != mainContent.ComboPresets.Text)
                        tuneContent.LoadProperty();
                    theContent.Children.Add(tuneContent);
                    //theContent.Content = tuneContent;
                    break;
                default:
                    snackbarMessage.Content = ":[";
                    main_SnackBar.IsActive = true;
                    if (!SnackBar_Hiding.IsAlive)
                        SnackBar_Hiding = new(new ThreadStart(HideSnackBar));
                    SnackBar_Hiding.Start();
                    break;
            }
            UpdateLayout();
            stopwatch.Stop();
            System.Diagnostics.Debug.Write("Change Content Time: ");
            System.Diagnostics.Debug.WriteLine(stopwatch.ElapsedMilliseconds);
            
        }

        private void LoadProperty()
        {
            int LoadAt = 0;
            foreach (string i in SavedProperty.LoadProperty())
            {
                if (LoadAt == 0) 
                { 
                    LastApplied_TextBox.Text = i;
                    if (mainContent.IsAvaibleKeyNameFile(i))
                        mainContent.ComboPresets.Text = i;
                    mainContent.ComboPresets_MouseLeave(null, null);
                }
                LoadAt++;
            }
        }

        
        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            PaletteHelper _PaletteHelper = new PaletteHelper();
            ITheme theme = _PaletteHelper.GetTheme();
            IBaseTheme baseTheme = null;
            if (theme.GetBaseTheme() == BaseTheme.Dark)
                baseTheme = new MaterialDesignLightTheme();
            else baseTheme = new MaterialDesignDarkTheme();
            theme.SetBaseTheme(baseTheme);
            _PaletteHelper.SetTheme(theme);
        }
    }
}
