using NLog;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace AciNote
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Properties.Settings mSettings = Properties.Settings.Default;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region 菜单单击事件
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuPageSetup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuPrintPreview_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuFind_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuFindNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuReplace_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuGoto_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuDateTime_Click(object sender, RoutedEventArgs e)
        {
            tbContent.Focus();
            Clipboard.SetText(DateTime.Now.ToString());
            tbContent.Paste();
        }

        private void menuViewHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuWordWrap_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            if (menu.IsChecked == true)
            {
                menu.IsChecked = false;
                tbContent.TextWrapping = TextWrapping.NoWrap;
            }
            else
            {
                menu.IsChecked = true;
                tbContent.TextWrapping = TextWrapping.Wrap;
            }
        }
        #endregion

        private void tbContent_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void tbContent_PreviewDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                string fileName = fileNames[0];
                this.Title = Path.GetFileName(fileName);
                tbContent.Text = File.ReadAllText(fileName, AppBase.GetFileEncodeType(fileName));
                tbContent.ScrollToHome();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void menuTopMost_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            if (menu.IsChecked == true)
            {
                menu.IsChecked = false;
                this.Topmost = false;
            }
            else
            {
                menu.IsChecked = true;
                this.Topmost = true;
            }
        }
    }
}
