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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using MaterialDesignThemes.Wpf;

namespace PUBG_Profiler.Content
{
    /// <summary>
    /// Interaction logic for TuneContent.xaml
    /// </summary>
    public partial class TuneContent : UserControl, INotifyPropertyChanged
    {
        enum Scope{ X2, X3, X4, X6, X8, X15 }
        enum Sensitivity { General = 1, Vertical, Aim, Ads, Scoping }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string PropertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private MainWindow mainWindow;
        public TuneContent(object mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow as MainWindow;
            DataContext = this;
            PresetUtensils.LoadIDKeys();
            LoadProperty();
            UPerScope_Click(null, null);
        }

        private void EnterRemoveFocus(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
            }
        }

        private void UPerScope_Click(object sender, RoutedEventArgs e)
        {
            Iv_UPerScope = (bool)ScopeUsingPerScope.IsChecked;
        }

        #region Value
        private bool __Iv_UPerScope;
        public bool Iv_UPerScope 
        {
            get => __Iv_UPerScope; 
            set
            {
                __Iv_UPerScope = !(value);
                OnPropertyChanged();
            }
        }
        #endregion

        public void LoadProperty()
        {
            CurrentFileName.Text = mainWindow.FileName;            
            int LoadAt = 0;
            int ScopeIDXLoad = (int)Scope.X2;
            bool LoadSlider = true;
            Stopwatch stopwatch = new();
            stopwatch.Start();
            foreach (string presetValue in PresetUtensils.GetPresetSensitivity(mainWindow.FileName))
            {
                //Debug.WriteLine(presetValue);
                if (LoadSlider)
                {
                    if (presetValue.Contains("Scope"))
                    {
                        foreach (TextBox textBox in ScopeGrid.Children.OfType<TextBox>())
                        {
                            if (Grid.GetRow(textBox) == ScopeIDXLoad)
                            {
                                string s = presetValue.Substring((PresetUtensils.SettingKeys[LoadAt] + PresetUtensils.IDKeys[LoadAt]).Length);
                                textBox.Text = s.Remove(s.IndexOf('.'));                               
                                ScopeIDXLoad++;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (TextBox textBox in AnotherGrid.Children.OfType<TextBox>())
                        {
                            if (presetValue.Contains((string)textBox.Tag))
                            {
                                string s = presetValue.Substring(PresetUtensils.IDKeys[LoadAt].Length);
                                //Debug.WriteLine(s);
                                if (!PresetUtensils.IDKeys[LoadAt].Contains('v'))
                                    s = s.Remove(s.IndexOf('.'));
                                if (PresetUtensils.IDKeys[LoadAt].Contains('v')) LoadSlider = false;
                                textBox.Text = s;
                                break;
                            }
                        }
                    }
                }
                else if (!LoadSlider)
                {
                    if (presetValue.Contains("UsingPerScope")) 
                        ScopeUsingPerScope .IsChecked = Convert.ToBoolean(presetValue.Substring(presetValue.IndexOf('=') + 1));
                    if (presetValue.Contains("InvertMouse"))
                        ToggleInvertMouse.IsChecked = Convert.ToBoolean(presetValue.Substring(presetValue.IndexOf('=') + 1));
                }
                
                LoadAt++;
            }
            stopwatch.Stop();
            Debug.Write("Read Preset Time: ");
            Debug.WriteLine(stopwatch.ElapsedMilliseconds);
        }
        
        private async void SaveBtnClick(object sender, RoutedEventArgs e)
        {
            Confirm_Dialog confirm_Dialog = new Confirm_Dialog("Save", false);
            await mainWindow.MainDialogHost.ShowDialog(confirm_Dialog);
            if(confirm_Dialog.DialogResult == true)
            {
                List<string> ToSave = new List<string>(new string[PresetUtensils.SettingKeys.Length + 1]);
                int ISaving = 0;
                foreach (TextBox textBox in ScopeGrid.Children.OfType<TextBox>())
                {
                    ToSave[ISaving] = "Scope" + textBox.Text;
                    ISaving++;
                }
                foreach (TextBox textBox in AnotherGrid.Children.OfType<TextBox>())
                {
                    ToSave[ISaving] = textBox.Tag.ToString() + textBox.Text;
                    ISaving++;
                }
                PresetUtensils.SaveProperty(mainWindow.FileName, ToSave);

                mainWindow.snackbarMessage.Content = "Saved!";
                mainWindow.SnackBar_Time += 2;
                mainWindow.main_SnackBar.IsActive = true;
                if (mainWindow.SnackBar_Hiding == null || !mainWindow.SnackBar_Hiding.IsAlive)
                {
                    mainWindow.SnackBar_Hiding = new(new System.Threading.ThreadStart(mainWindow.HideSnackBar));
                    mainWindow.SnackBar_Hiding.Start();
                }
            }            
        }

        private void RefreshProperty(object sender, RoutedEventArgs e) => LoadProperty();
    }
}
