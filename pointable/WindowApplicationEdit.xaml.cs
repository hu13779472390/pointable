using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.IconLib;
using System.Diagnostics;
using System.Drawing.Imaging;
using LeapProject;

namespace PointableUI
{
    /// <summary>
    /// Interaction logic for WindowNewApplication.xaml
    /// </summary>
    public partial class WindowApplicationEdit : Window
    {
        internal static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }


        private string mFolder = string.Empty;
        private MultiIcon mMultiIcon = new MultiIcon();
        private Action currentAction;
        private Bitmap bmpIcon;
        MainWindow mainWindow;
        private bool newAction;


        private void windowApplicationEdit_Loaded(object sender, RoutedEventArgs e)
        {
            this.Height = this.Height - (expanderAdvance.ActualHeight - 50);

          //  buttonOK.Margin = this.Height - expanderAdvance.ActualHeight;
          //  buttonCancel.Top = this.Height - expanderAdvance.ActualHeight;
        }
        public WindowApplicationEdit(Action actionToEdit, bool newApp, MainWindow _mainWindow)
        {
            InitializeComponent();

            
            newAction = newApp;
            mainWindow = _mainWindow;
            currentAction = actionToEdit;

            try
            {
                populateData();

                if (newApp)
                {
                    this.Title = "Add New Application";
                    textBoxDescription.Text = "New Application";
                }

                textBoxDescription.Focus();

                textBoxDescription.SelectAll();
            }
            catch { }
        }
        private void populateData()
        {
            if (currentAction.iconPath != null && currentAction.iconPath != "")
            {
                try
                {
                    if (currentAction.iconFromResource)
                    {
                        //imageIcon.Source = new BitmapImage(new Uri(MainWindow.resourcePath + currentAction.iconPath, UriKind.RelativeOrAbsolute));
                        imageIcon.Source = LeapProject.Tools.ToBitmapSource(new System.Drawing.Bitmap(
                            Form1.executable.GetManifestResourceStream("Pointable.Resources." + currentAction.iconPath)));
                    }
                    else
                    {
                        imageIcon.Source = new BitmapImage(new Uri(MainWindow.iconFolderPath + currentAction.iconPath, UriKind.RelativeOrAbsolute));
                    }
                }
                catch { }
            }

            try
            {
                textBoxfilePath.Text = currentAction.applicationPath;
                textBoxDescription.Text = currentAction.description;
                textBoxArguments.Text = currentAction.applicationArgs;
            }
            catch { }
        }
        private bool saveData()
        {
            if (mainWindow.checkActionNameExist(textBoxDescription.Text, currentAction))
            {
                System.Windows.MessageBox.Show("Action description already exists");
                textBoxDescription.SelectAll();
                return false;
            }
            else
            {
                currentAction.applicationPath = textBoxfilePath.Text;// = currentAction.applicationPath;
                currentAction.description = textBoxDescription.Text;// = currentAction.description;
                currentAction.applicationArgs = textBoxArguments.Text;// = currentAction.applicationArgs;
                currentAction.actionType = Action.ActionType.LaunchApplication;

                string iconAppFilename = @"icon_" + appFilename + ".png";
                string iconPath = MainWindow.iconFolderPath + iconAppFilename;
                //   if (File.Exists(iconPath)
                //       iconPath = @"icon_" + filename + ".png";

                if (iconSource == IconSource.App)
                {
                    try
                    {
                        bmpIcon.Save(iconPath, ImageFormat.Png);
                        currentAction.iconPath = iconAppFilename;
                        currentAction.iconFromResource = false;
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Error saving icon");
                    }
                }
                else if (iconSource == IconSource.File)
                {
                    if (iconFileName !=null)
                         currentAction.iconPath = iconFileName;
                }
                //PngBitmapEncoder encoder = new PngBitmapEncoder();
                // var image = Capture(true); // Take picture
                //encoder.Frames.Add(BitmapFrame.Create(image));

                //// Save the file to disk
                //var filename = String.Format("...");
                //using (var stream = new FileStream(filename, FileMode.Create))
                //{
                //    encoder.Save(stream);
                //}
                if (newAction)
                    mainWindow.saveApplication(currentAction);
                else
                    mainWindow.saveApplication();
            }
            return true;
        }

        private void expanderAdvance_Expanded(object sender, RoutedEventArgs e)
        {
            this.Height = this.Height + (expanderAdvance.ActualHeight - 50);
          //  buttonOK.Top = this.Height + expanderAdvance.ActualHeight;
          //  buttonCancel.Top = this.Height + expanderAdvance.ActualHeight;
        }

        private void expanderAdvance_Collapsed(object sender, RoutedEventArgs e)
        {

            this.Height = this.Height - (expanderAdvance.ActualHeight - 50);
           // buttonOK.Height = this.Height - expanderAdvance.ActualHeight;
          //  buttonCancel.Height = this.Height - expanderAdvance.ActualHeight;
        }

        OpenFileDialog openFileDialogBrowse;
        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            openFileDialogBrowse = new OpenFileDialog();
            try
            {
                DirectoryInfo di = new DirectoryInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.Programs));
              //  Debug.WriteLine(di);
              //  if (Directory.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs"))
               openFileDialogBrowse.InitialDirectory = di.ToString();
            }
            catch { }

            this.openFileDialogBrowse.Filter = "All files (*.*)|*.*";
            this.openFileDialogBrowse.FileOk += new System.ComponentModel.CancelEventHandler(openFileDialog1_FileOk);
   
            openFileDialogBrowse.ShowDialog();
        }

        string appFilename;
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                string fileFullPath = openFileDialogBrowse.FileName;
                appFilename = System.IO.Path.GetFileName(fileFullPath);

                string fileDescription = FileVersionInfo.GetVersionInfo(fileFullPath).FileDescription;
                textBoxfilePath.Text = fileFullPath;// = FileVersionInfo.GetVersionInfo(fileFullPath).FileDescription;

                if (fileDescription == null || fileDescription == "")
                    textBoxDescription.Text = appFilename;
                else
                {
                    if (fileDescription.Length > 40)
                       fileDescription =  fileDescription.Substring(0, 40);
                    textBoxDescription.Text = fileDescription;
                }
                try
                {
                    mMultiIcon.SelectedIndex = -1;
                    mMultiIcon.Load(fileFullPath);

                    int biggestIconWidth = 0;
                    IconImage biggestIcon = null;


                    for (int i = 0; i < mMultiIcon.Count; i++)
                    {
                        for (int j = 0; j < mMultiIcon[i].Count; j++)
                        {
                            if (mMultiIcon[i][j].Icon.Size.Width > biggestIconWidth)
                            {
                                biggestIconWidth = mMultiIcon[i][j].Icon.Size.Width;
                                biggestIcon = mMultiIcon[i][j];
                            }
                        }
                    }
                    if (biggestIcon != null)
                    {
                        bmpIcon = biggestIcon.Icon.ToBitmap();
                        imageIcon.Source = ToBitmapSource(bmpIcon);

                        iconSource = IconSource.App;
                    }
                }
                catch (Exception ex)
                {
                   // System.Drawing.Icon tmp = System.Drawing.Icon.ExtractAssociatedIcon(fileFullPath);
                   // Bitmap bmp = IconToBitmap(tmp);
                    bmpIcon = IconToBitmap(IconFromFilePath(fileFullPath));
                    imageIcon.Source = ToBitmapSource(bmpIcon);
                                        
                    iconSource = IconSource.App;
                }


            }
            catch { }
        }
        

        public static Icon IconFromFilePath(string filePath)
        {
            Icon result = null;
            try
            {
                result = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
                //'# swallow and return nothing. You could supply a default Icon here as well
            }
            catch
            {
            }
            return result;
        }

        private Bitmap IconToBitmap(Icon icon)
        {
            // Call ToBitmap to convert it.
            Bitmap bmp = icon.ToBitmap();
            return bmp;
       }


        private BitmapSource ToBitmapSource(System.Drawing.Bitmap source)
        {
            BitmapSource bitSrc = null;

            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }

            return bitSrc;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (saveData())
                this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void buttonBrowseIcon_Click(object sender, RoutedEventArgs e)
        {
            openFileDialogBrowseIcon = new OpenFileDialog();
            try
            {
                //  DirectoryInfo di = new DirectoryInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.Programs));
                //  Debug.WriteLine(di);
                //  if (Directory.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs"))
                openFileDialogBrowseIcon.InitialDirectory = MainWindow.iconFolderPath;
            }
            catch { }

            this.openFileDialogBrowseIcon.Filter = "PNG Files (*.png)|*.png|All files (*.*)|*.*";
            this.openFileDialogBrowseIcon.FileOk += new System.ComponentModel.CancelEventHandler(openFileDialogIcon_FileOk);

            openFileDialogBrowseIcon.ShowDialog();
        }

        //string filename;
        string iconFileName;

        OpenFileDialog openFileDialogBrowseIcon;
        IconSource iconSource = IconSource.None;
        public enum IconSource { None, App, File };

        private void openFileDialogIcon_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                string fileFullPath = openFileDialogBrowseIcon.FileName;
                string filename = System.IO.Path.GetFileName(fileFullPath);
                //  DirectoryInfo di = new DirectoryInfo(fileFullPath);
                string selectedFolderPath = new FileInfo(fileFullPath).DirectoryName;

                if (WindowPointableEditor.SameDirectory(MainWindow.iconFolderPath, selectedFolderPath))
                {
                    iconFileName = filename;
                }
                else
                {//copy over
                    File.Copy(fileFullPath, MainWindow.iconFolderPath + filename);
                    iconFileName = filename;
                }

                iconSource = IconSource.File;
                try
                {

                    imageIcon.Source = new BitmapImage(new Uri(MainWindow.iconFolderPath + filename, UriKind.RelativeOrAbsolute));

                }
                catch (Exception ex)
                {
                }

            }
            catch { }
        }


    }

}
