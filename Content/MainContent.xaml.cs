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
using MaterialDesignThemes.Wpf;

namespace PUBG_Profiler.Content
{
    /// <summary>
    /// Interaction logic for MainContent.xaml
    /// </summary>
    public partial class MainContent : UserControl
    {
        private readonly MainWindow mainWindow;
        private Thread ThApplyBadgedHide;
        public MainContent(object mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow as MainWindow;
            ComboBox_RefreshItem(null, null);            
        }

        public bool IsAvaibleKeyNameFile(string FileName) => File.Exists(GameDir + BaseNamePreset + KeyName + FileName + INI);
        private int ApplyBadgedTimeout = 0;
        private void HideApplyBadged()
        {
            while (ApplyBadgedTimeout > 0)
            {
                Thread.Sleep(1000);
                ApplyBadgedTimeout--;
            }                
            ApplyBtnBadge.Dispatcher.Invoke(() => { ApplyBtnBadge.Badge = ""; });
        }

        public static readonly string BaseNamePreset = @"\GameUserSettings";
        public static readonly string KeyName = " # ";
        public static string GameDir => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TslGame\Saved\Config\WindowsNoEditor";
        public static readonly string INI = ".ini";
        private void ComboBox_RefreshItem(object sender, RoutedEventArgs e)
        {
            ComboPresets.Items.Clear();
            ComboPresets.Items.Add(FindResource("s_Default") as string);
            foreach (string i in Directory.GetFiles(GameDir))
            {
                if (i.Contains(BaseNamePreset + KeyName))
                {
                    string j = i;
                    int RemovePath = j.IndexOf(KeyName) + 3;
                    j = j.Substring(RemovePath);
                    j = j.Remove(j.LastIndexOf('.'));
                    ComboPresets.Items.Add(j);
                }
            }
        }

        private void ApplySelection(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.PackIcon BadgedApplyIC = new();
            BadgedApplyIC.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;

            try
            {
                if (File.Exists(GameDir + BaseNamePreset + INI))
                    File.Delete(GameDir + BaseNamePreset + INI);
                File.Copy(GameDir + BaseNamePreset + KeyName + ComboPresets.Text + INI,
                    GameDir + BaseNamePreset + INI);
            }
            catch (Exception)
            {
                if (ComboPresets.Text == FindResource("s_Default") as string)
                    File.Copy(GameDir + BaseNamePreset + ".ini.origin",
                         GameDir + BaseNamePreset + INI);
                else BadgedApplyIC.Kind = MaterialDesignThemes.Wpf.PackIconKind.Close;
            }

            List<string> ToSaveLastUse = new List<string>
            {
                ComboPresets.Text // 0
            };
            SavedProperty.SaveProperty(ref ToSaveLastUse);

            ApplyBtnBadge.Badge = BadgedApplyIC;
            ApplyBadgedTimeout += 2;
            if (ThApplyBadgedHide == null || !(ThApplyBadgedHide.IsAlive))
            {
                ThApplyBadgedHide = new(new ThreadStart(HideApplyBadged));
                ThApplyBadgedHide.Start();
            }                
        }

        private async void RemoveSelection(object sender, RoutedEventArgs e)
        {
            if (File.Exists(GameDir + BaseNamePreset + KeyName + ComboPresets.Text + INI))
            {
                Confirm_Dialog confirm_Dialog = new(
                    (FindResource("s_Delete") as string) + " " + ComboPresets.Text, false);
                await mainWindow.MainDialogHost.ShowDialog(confirm_Dialog);
                if (confirm_Dialog.DialogResult == true)
                {
                    File.Delete(GameDir + BaseNamePreset + KeyName + ComboPresets.Text + INI);
                    ComboBox_RefreshItem(null, null);
                }
            }
            else
            {
                mainWindow.snackbarMessage.Content = ComboPresets.Text + " is " + FindResource("s_404") as string;
                mainWindow.main_SnackBar.IsActive = true;
                mainWindow.SnackBar_Time += 2;
                if (mainWindow.SnackBar_Hiding == null || !mainWindow.SnackBar_Hiding.IsAlive)
                {
                    mainWindow.SnackBar_Hiding = new Thread(new ThreadStart(mainWindow.HideSnackBar));
                    mainWindow.SnackBar_Hiding.Start();
                }
            }
        }

        private async void AddthisSelection(object sender, RoutedEventArgs e)
        {
            FieldText_Dialog text_Dialog = new(FindResource("s_AddThis") as string, ComboPresets.Text);
            await mainWindow.MainDialogHost.ShowDialog(text_Dialog);
            if (text_Dialog.DialogResult == true)
            {
                bool DoIt = true;
                if (IsAvaibleKeyNameFile(text_Dialog.FileName))
                {
                    Confirm_Dialog confirm_Dialog = new((FindResource("s_Replace") as string) + " " + text_Dialog.FileName, false);
                    await mainWindow.MainDialogHost.ShowDialog(confirm_Dialog);
                    if(confirm_Dialog.DialogResult == false) DoIt = false;
                }
                if (DoIt)
                {
                    File.Copy(GameDir + BaseNamePreset + INI,
                            GameDir + BaseNamePreset + KeyName + text_Dialog.FileName + INI, true);
                    ComboBox_RefreshItem(null, null);
                    ComboPresets.SelectedItem = text_Dialog.FileName;
                }
            }
        }

        private void ImportSelection(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "ini (*.ini)|*.ini";
            openFileDialog.FileOk += (s, c) =>
            {
                File.Copy(openFileDialog.FileName,
                    GameDir + BaseNamePreset + KeyName + openFileDialog.SafeFileName);
                ComboBox_RefreshItem(null, null);
                string select = openFileDialog.SafeFileName.Remove(openFileDialog.SafeFileName.LastIndexOf('.'));
                ComboPresets.SelectedItem = select;
            };
            openFileDialog.ShowDialog();
        }

        private void ExportSelection(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "ini (*.ini)|*.ini";
            saveFileDialog.FileName = ComboPresets.Text;
            saveFileDialog.FileOk += (s, c) =>
            {
                try
                {
                    File.Copy(GameDir + BaseNamePreset + KeyName + ComboPresets.Text + INI, saveFileDialog.FileName, true);
                }
                catch (Exception)
                {
                    mainWindow.snackbarMessage.Content = FindResource("s_Failed") as string;
                    mainWindow.main_SnackBar.IsActive = true;
                    mainWindow.SnackBar_Time += 2;
                    if (mainWindow.SnackBar_Hiding == null || !(mainWindow.SnackBar_Hiding.IsAlive))
                    {
                        mainWindow.SnackBar_Hiding = new(new ThreadStart(mainWindow.HideSnackBar));
                        mainWindow.SnackBar_Hiding.Start();
                    }                    
                }
            };
            saveFileDialog.ShowDialog();
        }

        private async void RenameSelection(object sender, RoutedEventArgs e)
        {
            FieldText_Dialog fieldText_Dialog = new(FindResource("s_Rename") as string, ComboPresets.Text);
            await mainWindow.MainDialogHost.ShowDialog(fieldText_Dialog);
            if (fieldText_Dialog.DialogResult == true)
            {
                try
                {
                    File.Move(GameDir + BaseNamePreset + KeyName + ComboPresets.Text + INI,
                    GameDir + BaseNamePreset + KeyName + fieldText_Dialog.FileName + INI);
                    ComboBox_RefreshItem(null, null);
                    ComboPresets.SelectedItem = fieldText_Dialog.FileName;
                }
                catch (Exception Ex)
                {
                    Confirm_Dialog confirm_Dialog = new(Ex.Message, true, @"Oho :\",true);
                    await mainWindow.MainDialogHost.ShowDialog(confirm_Dialog);
                }
            }
        }

        private async void DuplicateSelection(object sender, RoutedEventArgs e)
        {
            if (IsAvaibleKeyNameFile(ComboPresets.Text))
            {
                FieldText_Dialog fieldText_Dialog = new(FindResource("s_Duplicate") as string,
               (FindResource("s_Duplicate") as string) + " " + (FindResource("s_Of") as string)
               + " " + ComboPresets.Text);
                await mainWindow.MainDialogHost.ShowDialog(fieldText_Dialog);
                if (fieldText_Dialog.DialogResult == true)
                {
                    bool DoIt = true;
                    if(File.Exists(GameDir + BaseNamePreset + KeyName + fieldText_Dialog.FileName + INI))
                    {
                        Confirm_Dialog confirm_Dialog = new((FindResource("s_Replace") as string) + " " + fieldText_Dialog.FileName, false);
                        await mainWindow.MainDialogHost.ShowDialog(confirm_Dialog);
                        if (confirm_Dialog.DialogResult == false) DoIt = false;
                    }
                    if(DoIt)
                        File.Copy(GameDir + BaseNamePreset + KeyName + ComboPresets.Text + INI,
                            GameDir + BaseNamePreset + KeyName + fieldText_Dialog.FileName + INI, true);
                    ComboBox_RefreshItem(null, null);
                    ComboPresets.SelectedItem = fieldText_Dialog.FileName;
                }
            }
            else
            {
                mainWindow.snackbarMessage.Content = ComboPresets.Text + " is " + FindResource("s_404") as string;
                mainWindow.main_SnackBar.IsActive = true;
                mainWindow.SnackBar_Time += 2;
                if (mainWindow.SnackBar_Hiding == null || !(mainWindow.SnackBar_Hiding.IsAlive))
                {
                    mainWindow.SnackBar_Hiding = new Thread(new ThreadStart(mainWindow.HideSnackBar));
                    mainWindow.SnackBar_Hiding.Start();
                }                   
            }
        }

        private void RunPUBG(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "steam://rungameid/578080");
            App.Current.Shutdown();
        }

        public void ComboPresets_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ComboPresets.Text == FindResource("s_Default") as string )            
                GridOperation.IsEnabled = false;                         
            else            
                GridOperation.IsEnabled = true;
            
            if (IsAvaibleKeyNameFile(ComboPresets.Text))
                mainWindow.Tune_ItemList.IsEnabled = true;
            else mainWindow.Tune_ItemList.IsEnabled = false;            
        }

        private void ApplynRunPUBG(object sender, RoutedEventArgs e)
        {
            ApplySelection(null, null);
            RunPUBG(null, null);
        }
    }
}
