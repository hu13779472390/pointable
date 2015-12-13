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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using LeapProject;

namespace PointableUI
{
    /// <summary>
    /// Interaction logic for WindowNewKeystrokes.xaml
    /// </summary>
    public partial class WindowNewKeystrokes : Window
    {
        private string mFolder = string.Empty;
        private Action currentAction;
        private Bitmap bmpIcon;
        MainWindow mainWindow;
        private bool newAction;
        public byte[] keys = new byte[4];


        public WindowNewKeystrokes(Action actionToEdit, bool newApp, MainWindow _mainWindow)
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
                    this.Title = "Add New Keystroke";
                    textBoxDescription.Text = "New Keystroke";
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
                //textBoxfilePath.Text = currentAction.applicationPath;
                textBoxDescription.Text = currentAction.description;
                // textBoxArguments.Text = currentAction.applicationArgs;
                textBoxKey1.Text = currentAction.keyDescription[0];
                textBoxKey2.Text = currentAction.keyDescription[1];
                textBoxKey3.Text = currentAction.keyDescription[2];
                textBoxKey4.Text = currentAction.keyDescription[3];

                for (int i = 0; i < 4; i++)
                    keys[i] = currentAction.keys[i];
            }

            catch { }
        }
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

        OpenFileDialog openFileDialogBrowse;

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                string fileFullPath = openFileDialogBrowse.FileName;
                filename = System.IO.Path.GetFileName(fileFullPath);
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

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (saveData())
                this.Close();

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool saveData()
        {
            try
            {
                // currentPointable.applicationPath =  textBoxfilePath.Text;// = currentAction.applicationPath;
                currentAction.description = textBoxDescription.Text;// = currentAction.description;
                //  currentPointable.applicationArgs = textBoxArguments.Text;// = currentAction.applicationArgs;

                currentAction.actionType = Action.ActionType.Keystroke;
                //    string iconFilename = @"icon_" + filename + ".png";
                //    string iconPath = MainWindow.iconFolderPath + iconFilename;
                //   if (File.Exists(iconPath)
                //       iconPath = @"icon_" + filename + ".png";

                if (mainWindow.checkActionNameExist(currentAction.description, currentAction))
                {
                    System.Windows.MessageBox.Show("Action description already exists");
                    textBoxDescription.SelectAll();
                    return false;
                }
                else
                {
                    currentAction.keyDescription[0] = textBoxKey1.Text;
                    currentAction.keyDescription[1] = textBoxKey2.Text;
                    currentAction.keyDescription[2] = textBoxKey3.Text;
                    currentAction.keyDescription[3] = textBoxKey4.Text;

                    for (int i = 0; i < 4; i++)
                        currentAction.keys[i] = keys[i];

                    if (iconFileName != null && iconFileName != "")
                    {
                        currentAction.iconPath = iconFileName;
                        currentAction.iconFromResource = false;
                    }

                    if (newAction)
                        mainWindow.saveKeystrokes(currentAction);
                    else
                        mainWindow.saveKeystrokes();//currentAction);
                }
            }
            catch { }

            return true;
        }

        private void textBoxKey1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void textBoxKey1_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key keyTriggered = e.Key;

            if (keyTriggered == Key.ImeProcessed)
                keyTriggered = e.ImeProcessedKey;

            if (keyTriggered == Key.System && e.SystemKey == Key.F10)
            {
                keyTriggered = Key.F10;
            }

            string keyString = keyTriggered.ToString();
            if (keyTriggered == Key.System)
                keyString = "Alt";

            textBoxKey1.Text = keyString;

            keys[0] = (byte)System.Windows.Input.KeyInterop.VirtualKeyFromKey(keyTriggered);
            if (keyTriggered == Key.System)
                keys[0] = (byte)0x12;
            e.Handled = true;
        }
        private void textBoxKey2_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void textBoxKey2_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key keyTriggered = e.Key;

            if (keyTriggered == Key.ImeProcessed)
                keyTriggered = e.ImeProcessedKey;

            if (keyTriggered == Key.System && e.SystemKey == Key.F10)
            {
                keyTriggered = Key.F10;
            }

            string keyString = keyTriggered.ToString();
            if (keyTriggered == Key.System)
                keyString = "Alt";

            textBoxKey2.Text = keyString;

            keys[1] = (byte)System.Windows.Input.KeyInterop.VirtualKeyFromKey(keyTriggered);
            if (keyTriggered == Key.System)
                keys[1] = (byte)0x12;
            e.Handled = true;
        }
        private void textBoxKey3_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void textBoxKey3_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key keyTriggered = e.Key;

            if (keyTriggered == Key.ImeProcessed)
                keyTriggered = e.ImeProcessedKey;

            if (keyTriggered == Key.System && e.SystemKey == Key.F10)
            {
                keyTriggered = Key.F10;
            }

            string keyString = keyTriggered.ToString();
            if (keyTriggered == Key.System)
                keyString = "Alt";

            textBoxKey3.Text = keyString;

            keys[2] = (byte)System.Windows.Input.KeyInterop.VirtualKeyFromKey(keyTriggered);
            if (keyTriggered == Key.System)
                keys[2] = (byte)0x12;
            e.Handled = true;
        }
        private void textBoxKey4_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void textBoxKey4_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key keyTriggered = e.Key;

            if (keyTriggered == Key.ImeProcessed)
                keyTriggered = e.ImeProcessedKey;

            if (keyTriggered == Key.System && e.SystemKey == Key.F10)
            {
                keyTriggered = Key.F10;
            }

            string keyString = keyTriggered.ToString();
            if (keyTriggered == Key.System)
                keyString = "Alt";

            textBoxKey4.Text = keyString;

            keys[3] = (byte)System.Windows.Input.KeyInterop.VirtualKeyFromKey(keyTriggered);
            if (keyTriggered == Key.System)
                keys[3] = (byte)0x12;
            e.Handled = true;
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxKey1.Text = "";
            textBoxKey2.Text = "";
            textBoxKey3.Text = "";
            textBoxKey4.Text = "";


            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = 0;
            }
        }

    }
}
