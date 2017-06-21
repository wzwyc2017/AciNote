using Microsoft.Win32;
using NLog;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AciNote
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Properties.Settings mSettings = Properties.Settings.Default;
        private bool TextIsChanged;
        private string CurFileName;

        public MainWindow(string fileName = null)
        {
            InitializeComponent();

            var bounds = mSettings.MainWindowRestoreBounds;
            this.Left = bounds.Left;
            this.Top = bounds.Top;
            this.Width = bounds.Width;
            this.Height = bounds.Height;
            this.Topmost = menuTopMost.IsChecked = mSettings.Topmost;
            MainTextBox.TextWrapping = mSettings.TextWrapping;
            MainTextBox.FontFamily = new FontFamily(mSettings.FontName);
            MainTextBox.FontSize = mSettings.FontSize;
            MainTextBox.Foreground = AppBase.ConvertStringToBrush(mSettings.Foreground);
            MainTextBox.Background = AppBase.ConvertStringToBrush(mSettings.Background);
            menuWordWrap.IsChecked = (mSettings.TextWrapping == TextWrapping.Wrap);

            this.Closing += MainWindow_Closing;
            MainTextBox.TextChanged += MainTextBox_TextChanged;

            if (string.IsNullOrEmpty(fileName))
            {
                CreateNewText();
            }
            else
            {
                FormLoadFile(fileName);
            }
        }

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextIsChanged = true;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TextIsChanged)
            {
                if (string.IsNullOrEmpty(CurFileName) && string.IsNullOrEmpty(MainTextBox.Text))
                {
                    //新文件，并且内容是空，不保存
                }
                else
                {
                    if (string.IsNullOrEmpty(CurFileName) == false)
                    {
                        var rst = MessageBox.Show(this, "是否将更改保存到\r\n" + CurFileName + "？", "记事本", MessageBoxButton.YesNoCancel);
                        if (rst == MessageBoxResult.Yes)
                        {
                            if (SaveTextToFile(CurFileName) == false)
                            {
                                e.Cancel = true;
                                return;
                            }
                        }
                        else if (rst == MessageBoxResult.No)
                        {
                            //不保存退出
                        }
                        else if (rst == MessageBoxResult.Cancel)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    else
                    {
                        var rst = MessageBox.Show(this, "是否将更改保存到 无标题", "记事本", MessageBoxButton.YesNoCancel);
                        if (rst == MessageBoxResult.Yes)
                        {
                            if (ShowDialogAndSave())
                            {
                                //正常保存
                            }
                            else
                            {
                                e.Cancel = true;
                                return;
                            }
                        }
                        else if (rst == MessageBoxResult.No)
                        {
                            //不保存退出
                        }
                        else if (rst == MessageBoxResult.Cancel)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                }
            }
        }

        #region 菜单单击事件
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckChangeForSave())
                {
                    CreateNewText();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckChangeForSave())
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = AppBase.FileFilter;
                    if (openFileDialog.ShowDialog() == true)
                    {
                        FormLoadFile(openFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(CurFileName))
                {
                    ShowDialogAndSave();
                }
                else
                {
                    SaveTextToFile(CurFileName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowDialogAndSave(CurFileName);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
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
            MainTextBox.Focus();
            Clipboard.SetText(DateTime.Now.ToString());
            MainTextBox.Paste();
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
                mSettings.TextWrapping = MainTextBox.TextWrapping = TextWrapping.NoWrap;
            }
            else
            {
                menu.IsChecked = true;
                mSettings.TextWrapping = MainTextBox.TextWrapping = TextWrapping.Wrap;
            }
            mSettings.Save();
        }

        private void menuTopMost_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            if (menu.IsChecked == true)
            {
                mSettings.Topmost = this.Topmost = menu.IsChecked = false;
                this.Topmost = false;
            }
            else
            {
                mSettings.Topmost = this.Topmost = menu.IsChecked = true;
            }
            mSettings.Save();
        }

        private void menuFont_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FontDialog dlg = new System.Windows.Forms.FontDialog();
                dlg.Font = new System.Drawing.Font(MainTextBox.FontFamily.ToString(), (float)MainTextBox.FontSize);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var font = dlg.Font;
                    MainTextBox.FontFamily = new FontFamily(font.FontFamily.Name);
                    MainTextBox.FontSize = font.Size;

                    mSettings.FontName = font.FontFamily.Name;
                    mSettings.FontSize = font.Size;
                    mSettings.Save();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void menuForeColor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
                dlg.Color = AppBase.ConvertBrushToColor(MainTextBox.Foreground);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var color = dlg.Color;
                    MainTextBox.Foreground = AppBase.ConvertColorToBrush(color);

                    mSettings.Foreground = MainTextBox.Foreground.ToString();
                    mSettings.Save();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void menuBackColor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
                dlg.Color = AppBase.ConvertBrushToColor(MainTextBox.Background);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var color = dlg.Color;
                    MainTextBox.Background = AppBase.ConvertColorToBrush(color);

                    mSettings.Background = MainTextBox.Background.ToString();
                    mSettings.Save();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 其它事件函数
        private void MainTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void MainTextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                string fileName = fileNames[0];
                this.Title = Path.GetFileName(fileName);
                MainTextBox.Text = File.ReadAllText(fileName, AppBase.GetFileEncodeType(fileName));
                MainTextBox.ScrollToHome();
                CurFileName = fileName;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mSettings.MainWindowRestoreBounds = this.RestoreBounds;
            mSettings.Save();
        }
        #endregion

        #region 功能实现函数
        /// <summary>
        /// 确认文本是否已经修改
        /// </summary>
        /// <returns></returns>
        private bool CheckChangeForSave()
        {
            if (TextIsChanged)
            {
                if (string.IsNullOrEmpty(CurFileName) && string.IsNullOrEmpty(MainTextBox.Text))
                {
                    //新文件，并且内容是空，不保存
                }
                else
                {
                    if (string.IsNullOrEmpty(CurFileName) == false)
                    {
                        var rst = MessageBox.Show(this, "是否将更改保存到\r\n" + CurFileName + "？", "记事本", MessageBoxButton.YesNoCancel);
                        if (rst == MessageBoxResult.Yes)
                        {
                            if (SaveTextToFile(CurFileName) == false)
                            {
                                return false;
                            }
                        }
                        else if (rst == MessageBoxResult.No)
                        {
                            //不保存退出
                        }
                        else if (rst == MessageBoxResult.Cancel)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        var rst = MessageBox.Show(this, "是否将更改保存到 无标题", "记事本", MessageBoxButton.YesNoCancel);
                        if (rst == MessageBoxResult.Yes)
                        {
                            if (ShowDialogAndSave())
                            {
                                //正常保存
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (rst == MessageBoxResult.No)
                        {
                            //不保存退出
                        }
                        else if (rst == MessageBoxResult.Cancel)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 保存文本信息到指定文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool SaveTextToFile(string fileName)
        {
            System.IO.File.WriteAllText(fileName, MainTextBox.Text);
            TextIsChanged = false;

            return true;
        }

        /// <summary>
        /// 显示保存对话框并保存文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool ShowDialogAndSave(string fileName = "")
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = AppBase.FileFilter;
            if (string.IsNullOrEmpty(fileName) == false)
            {
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(fileName);
                string extName = Path.GetExtension(fileName).ToLower();
                if (extName.Equals(".txt"))
                {
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.FileName = Path.GetFileNameWithoutExtension(fileName);
                }
                else
                {
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.FileName = Path.GetFileName(fileName);
                }
            }
            if (saveFileDialog.ShowDialog() == true)
            {
                string fname = saveFileDialog.FileName;
                if (SaveTextToFile(fname))
                {
                    CurFileName = fname;
                    this.Title = GetFormNameByFullName(CurFileName);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据文件名显示窗体的名称
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetFormNameByFullName(string fileName)
        {
            return Path.GetFileName(fileName) + " - 记事本";
        }

        /// <summary>
        /// 创建新文本
        /// </summary>
        private void CreateNewText()
        {
            MainTextBox.Clear();
            CurFileName = "";
            this.Title = "无标题 - 记事本";
            TextIsChanged = false;
        }

        /// <summary>
        /// 加载文本文件
        /// </summary>
        /// <param name="fileName"></param>
        private void FormLoadFile(string fileName)
        {
            MainTextBox.Text = System.IO.File.ReadAllText(fileName, AppBase.GetFileEncodeType(fileName));
            MainTextBox.SelectionStart = 0;
            MainTextBox.SelectionLength = 0;
            TextIsChanged = false;
            CurFileName = fileName;
            this.Title = GetFormNameByFullName(CurFileName);
        }
        #endregion
    }
}
