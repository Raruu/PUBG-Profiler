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
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace PUBG_Profiler
{
    /// <summary>
    /// Interaction logic for Confirm_Dialog.xaml
    /// </summary>
    public partial class Confirm_Dialog : UserControl
    {
        public bool DialogResult { get; set; }
        public Confirm_Dialog()
        {
            InitializeComponent();            
        }
        public Confirm_Dialog(string ContentText, bool HideNo) : this()
        {
            textContent.Text = ContentText;
            if(HideNo) Btn_No.Visibility = Visibility.Hidden;
        }
        public Confirm_Dialog(string ContentText, bool HideNo, string OkText, bool HideQMark) : this(ContentText, HideNo)
        {
            Btn_Yes.Content = OkText;
            if (HideQMark) QMark.Visibility = Visibility.Hidden;
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
