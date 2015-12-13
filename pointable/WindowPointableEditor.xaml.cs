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
    /// Interaction logic for WindowPointableEditor.xaml
    /// </summary>
    public partial class WindowPointableEditor : Window
    {
        internal static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }


        private string mFolder = string.Empty;
        private MultiIcon mMultiIcon = new MultiIcon();
        private PointableObject currentPointable;
        private Bitmap bmpIcon;
        MainWindow mainWindow;
        bool newPointable;
        PointableObject.PointableType pointableType;
        string windowTitle = "";
        string windowClass = "";
        

        private void WindowPointableEditor_Loaded(object sender, RoutedEventArgs e)
        {
          //  this.Height = this.Height - (expanderAdvance.ActualHeight - 50);

          //  buttonOK.Margin = this.Height - expanderAdvance.ActualHeight;
          //  buttonCancel.Top = this.Height - expanderAdvance.ActualHeight;
        }
        public WindowPointableEditor(PointableObject pointableToEdit, bool newPointableObject, MainWindow _mainWindow)
        {
            InitializeComponent();

            mainWindow = _mainWindow;
            currentPointable = pointableToEdit;
            newPointable = newPointableObject;

            try
            {
                populateData();

                if (currentPointable.locked)
                {
                    textBoxDescription.IsEnabled = false;
                    textBoxArguments.IsEnabled = false;
                    textBoxfilePath.IsEnabled = false;
                    buttonBrowse.IsEnabled = false;
                    radioButtonApplicationPointable.IsEnabled = false;
                    radioButtonPhysicalPointable.IsEnabled = false;

                    buttonAssignWindow.IsEnabled = false;
                }

                if (newPointable)
                {
                    this.Title = "Add New Pointable";
                }

                textBoxDescription.Focus();

                textBoxDescription.SelectAll();
            }
            catch { }
        }
        private void populateData()
        {
            try
            {
                if (currentPointable.iconPath != null)
                {
                    if (currentPointable.iconFromResource)
                        imageIcon.Source = LeapProject.Tools.ToBitmapSource(new System.Drawing.Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + currentPointable.iconPath)));
                    // imageIcon.Source = new BitmapImage(new Uri(MainWindow.resourcePath + currentPointable.iconPath, UriKind.RelativeOrAbsolute));
                    else
                    {
                        imageIcon.Source = new BitmapImage(new Uri(MainWindow.iconFolderPath + currentPointable.iconPath, UriKind.RelativeOrAbsolute));
                    }
                }
            }

            catch { }
           // textBoxfilePath.Text = currentPointable.applicationPath;
            textBoxDescription.Text = currentPointable.description;

            if (currentPointable.type == PointableObject.PointableType.Physical)
            {
                radioButtonPhysicalPointable.IsChecked = true;
                windowClass = currentPointable.windowClass;
                windowTitle = currentPointable.windowTitle;
            }
            else
            {
                radioButtonApplicationPointable.IsChecked = true;
            }
          //  textBoxArguments.Text = currentPointable.applicationArgs;
        }
        private bool saveData()
        {
          // currentPointable.applicationPath =  textBoxfilePath.Text;// = currentAction.applicationPath;
          // = currentAction.description;
         //  currentPointable.applicationArgs = textBoxArguments.Text;// = currentAction.applicationArgs;

       //    string iconFilename = @"icon_" + filename + ".png";
       //    string iconPath = MainWindow.iconFolderPath + iconFilename;
         //   if (File.Exists(iconPath)
         //       iconPath = @"icon_" + filename + ".png";


           if (mainWindow.checkPointableNameExist(textBoxDescription.Text, currentPointable))
            {
                System.Windows.MessageBox.Show("Pointable name already exists");
                textBoxDescription.SelectAll();
                return false;
            }
            else
           {
               if (iconFileName != null && iconFileName != "")
               {
                   currentPointable.iconPath = iconFileName;
                   currentPointable.iconFromResource = false;
               }

               if (currentPointable.description != "" &&
                   currentPointable.description != textBoxDescription.Text)
               {
                   if (!newPointable)
                    mainWindow.mform1.deletePointableFile(currentPointable);
               }
               currentPointable.description = textBoxDescription.Text;

               currentPointable.type = pointableType;
               currentPointable.windowTitle = windowTitle;
               currentPointable.windowClass = windowClass;

                mainWindow.savePointable(currentPointable);
             }

            return true;
        }

        private void expanderAdvance_Expanded(object sender, RoutedEventArgs e)
        {
            this.Height = this.Height + (expanderAdvance.ActualHeight - 50);
        }

        private void expanderAdvance_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Height = this.Height - (expanderAdvance.ActualHeight - 50);
        }

        OpenFileDialog openFileDialogBrowse;
        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            openFileDialogBrowse = new OpenFileDialog();
            try
            {
              //  DirectoryInfo di = new DirectoryInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.Programs));
              //  Debug.WriteLine(di);
              //  if (Directory.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs"))
                openFileDialogBrowse.InitialDirectory = MainWindow.iconFolderPath;
            }
            catch { }

            this.openFileDialogBrowse.Filter = "PNG Files (*.png)|*.png|All files (*.*)|*.*";
            this.openFileDialogBrowse.FileOk += new System.ComponentModel.CancelEventHandler(openFileDialog1_FileOk);
   
            openFileDialogBrowse.ShowDialog();
        }

        string filename;
        string iconFileName;

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                string fileFullPath = openFileDialogBrowse.FileName;
                filename = System.IO.Path.GetFileName(fileFullPath);
              //  DirectoryInfo di = new DirectoryInfo(fileFullPath);
                string selectedFolderPath = new FileInfo(fileFullPath).DirectoryName;

                if (SameDirectory(MainWindow.iconFolderPath, selectedFolderPath))
                {
                    iconFileName = filename;
                }
                else
                {//copy over
                    File.Copy(fileFullPath, MainWindow.iconFolderPath + filename);
                    iconFileName = filename;
                }
                
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

        static public bool SameDirectory(string path1, string path2)
        {
            return (
                0 == String.Compare(
                    System.IO.Path.GetFullPath(path1).TrimEnd('\\'),
                    System.IO.Path.GetFullPath(path2).TrimEnd('\\'),
                    StringComparison.InvariantCultureIgnoreCase))
                ;
        }

        private void buttonCalibrate_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mform1.startCalibration(currentPointable);
        }

        private void buttonAssignWindow_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mform1.startAttachToWindow(currentPointable);
        }

        internal void updateAttachToWindowData(string _windowTitle, string _windowClass, IntPtr _windowHandle)
        {
            if (windowClass != null)
            {
                windowClass = _windowClass;
            }
            if (windowTitle != null)
            {
                windowTitle = _windowTitle;
            }
            if (_windowHandle != IntPtr.Zero)
            {
               // windowHandle = _windowHandle;
                Bitmap bmpIcon = mainWindow.mform1.getHandleIcon(_windowHandle);
                if (bmpIcon != null)
                {
                    imageIcon.Source = ToBitmapSource(bmpIcon);

                    string executablePath = Tools.GetProcessPath(_windowHandle);
                    string appFilename = System.IO.Path.GetFileName(executablePath);

                    string iconAppFilename = @"icon_" + appFilename + ".png";
                    string iconPath = MainWindow.iconFolderPath + iconAppFilename;

                    try
                    {
                        if (!File.Exists(iconPath))
                        {
                            bmpIcon.Save(iconPath, ImageFormat.Png);
                        }
                        iconFileName = iconAppFilename;
                        //iconFromResource = false;
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Error saving icon");
                    }
                }
                //icon
            }


        }

        private void radioButtonPhysicalPointable_Checked(object sender, RoutedEventArgs e)
        {
            pointableType = PointableObject.PointableType.Physical;

            buttonCalibrate.IsEnabled = true;
            buttonAssignWindow.IsEnabled = false;
        }

        private void radioButtonApplicationPointable_Checked(object sender, RoutedEventArgs e)
        {
            pointableType = PointableObject.PointableType.Application;

            buttonCalibrate.IsEnabled = false;
            buttonAssignWindow.IsEnabled = true;
        }
    }

}
