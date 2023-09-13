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
    /// Interaction logic for FieldText_Dialog.xaml
    /// </summary>
    public partial class FieldText_Dialog : UserControl
    {
        public bool DialogResult { get; set; }
        private string __text_hint = ". . .";
        public string Text_Hint { get { return __text_hint; } }
        private string __filename = "";
        public string FileName { get { return __filename; } }
        public FieldText_Dialog()
        {
            InitializeComponent();
            DataContext = this;
            DialogResult = false;
        }
        public FieldText_Dialog(string hintText) : this()
        {
            __text_hint = hintText;
        }
        public FieldText_Dialog(string hintText, string fileName) : this(hintText)
        {
            __filename = fileName;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            TextField.Text = FileName;
            if (TextField.Text != "") TextField.Focus();
        }


        private void Cancle_Click(object sender, RoutedEventArgs e) => DialogHost.CloseDialogCommand.Execute(null, null);
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            __filename = TextField.Text;
            DialogResult = true;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        //private void Grid_Move(object sender, MouseButtonEventArgs e) => DragMove();
    }
}
