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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using LeapProject;

namespace PointableUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal Form1 mform1;

        public MainWindow( Form1 form1)
        {
            mform1 = form1;
            InitializeComponent();
            imageControls = new Image[] { imageTop, imageRight, imageBottom, imageLeft, imageCenter };

            string executableName = System.Windows.Forms.Application.ExecutablePath;
            FileInfo executableFileInfo = new FileInfo(executableName);
            exeDirectory = executableFileInfo.DirectoryName;

            iconFolderPath = Form1.iconFolderPath;//exeDirectory + @"\Icons\";

            setUIActionVisible(false);

            this.pointables = mform1.pointables;
            this.actionsApplication = mform1.actionsApplication;
            this.actionsKeystrokes = mform1.actionsKeystrokes;

            generateObjects();

        }


        Action actionLaunchSoundIncrease = new Action("Increase Volume", "sound_increase.png", "", "");
        Action actionLaunchSoundNext = new Action("Next Track", "sound_next.png", "", "");
        Action actionLaunchSoundDecrease = new Action("Decrease Volume", "sound_decrease.png", "", "");
        Action actionLaunchSoundMute = new Action("Mute", "sound_mute.png", "", "");

        Action actionLaunchPowerPoint = new Action("Launch PowerPoint", "screen_ppt.png","powerpnt","" );
        Action actionLaunchWord = new Action("Launch Word", "screen_word.png", "winword", "");
        Action actionLaunchExcel = new Action("Launch Excel", "screen_excel.png", "excel", "");
        Action actionLaunchChrome = new Action("Launch Chrome", "screen_chrome.png", "chrome", "");
        
        Action actionLaunchFacebook = new Action("Facebook", "monitor_facebook.png", "http://www.facebook.com", "");
        Action actionLaunchYahoo = new Action("Yahoo", "monitor_yahoo.png", "http://www.yahoo.com", "");
        Action actionLaunchCNN = new Action("CNN", "monitor_cnn.png", "http://www.cnn.com", "");
        Action actionLaunchEngadget = new Action("Engadget", "monitor_engadget.png", "http://www.engadget.com", "");

        Action actionLaunchPhotoshop = new Action("Photoshop", "picture_photoshop.png", @"C:\Program Files (x86)\Adobe\Adobe Photoshop CS4\Photoshop.exe", "");

        Action actionKeyShowDesktop = new Action("Show Desktop", "key_keyboard.png","Win", "D","","", (byte)0x5b, (byte)0x44, (byte)0x00, (byte)0x00);
        Action actionKeyCopy = new Action("Copy", "key_keyboard.png", "Control", "C", "", "", (byte)0x11, (byte)0x43, (byte)0x00, (byte)0x00);
        Action actionKeyPaste = new Action("Paste", "key_keyboard.png", "Control", "P", "", "", (byte)0x11, (byte)0x50, (byte)0x00, (byte)0x00);

        Action actionKeyRefresh = new Action("Browser Refresh", "key_keyboard.png", "F5", "", "", "", (byte)0x74, (byte)0x00, (byte)0x00, (byte)0x00);
        Action actionKeyBack = new Action("Browser Back", "key_keyboard.png", "Alt", "Left", "", "", (byte)0x12, (byte)0x25, (byte)0x00, (byte)0x00);
        Action actionKeyForward = new Action("Browser Forward", "key_keyboard.png", "Alt", "Right", "", "", (byte)0x12, (byte)0x27, (byte)0x00, (byte)0x00);
        Action actionKeyNewTab = new Action("Browser New Tab", "key_keyboard.png", "Control", "T", "", "", (byte)0x11, (byte)0x54, (byte)0x00, (byte)0x00);


     //   Action actionEmpty = new Action("", "", "", "");
        
        Image[] imageControls;// = new Image [4];

        //store all actions
        List<Action> actionsApplication = new List<Action>();
        List<Action> actionsKeystrokes = new List<Action>();

        //store all keystrokes
       // Action[] keystrokes;

        List<PointableObject> pointables = new List<PointableObject>();

        //string resourcePath = @"pack://application:,,/PointableUI;component/Resources/";
        //string resourcePath = @"Resource/";
        internal static string resourcePath = @"pack://application:,,/Pointable;component/Resources/";
        internal static string exeDirectory;
        internal static string iconFolderPath;//@"\Icons\"; = @"\Icons\";

        int selectedObjectIndex = 0;

        private void generateObjects()
        {
            //Action[] actionArray = new Action [] { actionLaunchPowerPoint, actionLaunchWord, actionLaunchExcel, actionLaunchChrome, actionLaunchFacebook, actionLaunchYahoo, actionLaunchCNN, actionLaunchEngadget, actionLaunchPhotoshop};

            //foreach (Action action in actionArray)
            //    actionsApplication.Add(action);

            //actionArray = new Action[] { actionKeyShowDesktop, actionKeyCopy, actionKeyPaste, actionKeyRefresh, actionKeyBack, actionKeyForward, actionKeyNewTab };

            //foreach (Action action in actionArray)
            //    actionsKeystrokes.Add(action);



            //pointables.Add(new PointableObject("Screen", "screen.png",
            //    actionLaunchPowerPoint, actionLaunchWord, actionLaunchExcel, actionLaunchChrome));

            //pointables.Add( new PointableObject("Speakers", "sound.png",
            //    actionLaunchSoundIncrease, actionLaunchSoundNext, actionLaunchSoundDecrease, actionLaunchSoundMute));
            
            //pointables.Add(new PointableObject("Monitor", "monitor.png",
            //    actionLaunchFacebook, actionLaunchYahoo, actionLaunchCNN, actionLaunchEngadget));

            //pointables.Add(new PointableObject("Picture", "screen.png",
            //    actionLaunchPhotoshop, actionEmpty(), actionLaunchPowerPoint, actionEmpty()));

            //pointables.Add(new PointableObject("Browser Control", "screen.png",
            //    actionKeyRefresh, actionKeyBack, actionKeyForward, actionKeyNewTab));

            try
            {
                populatePointableListBox();
                listBoxPointables.SelectedIndex = 0;
                updateComboboxApplication();
                updateComboboxKeystrokes();
                populateUIObject(selectedObjectIndex);
            }
            catch { }

        }

        private void resetObjectIconsOnly(int selectedObject)
        {
            String iconPath;
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    iconPath = pointables[selectedObject].actions[i].iconPath;
                    if (iconPath != null && iconPath != "")
                    {
                        string test = MainWindow.resourcePath;
                        if (pointables[selectedObject].actions[i].iconFromResource)
                        {
                            //imageControls[i].Source = new BitmapImage(new Uri(MainWindow.resourcePath + iconPath, UriKind.RelativeOrAbsolute));
                            imageControls[i].Source = Tools.ToBitmapSource(new System.Drawing.Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + iconPath)));
                        }
                        else
                            imageControls[i].Source = new BitmapImage(new Uri(MainWindow.iconFolderPath + iconPath, UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        //imageControls[i].Source = null; 
                        imageControls[i].Source = Tools.ToBitmapSource(new System.Drawing.Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.action_question.png")));
                    }
                }
                catch { }
            }

            //resetAll(false);
           // setUIActionVisible(false);
        }

        private void populateUIObject(int selectedObject)
        {
            String iconPath;
            resetObjectIconsOnly(selectedObject);

            try
            {
                iconPath = pointables[selectedObject].iconPath;
                if (iconPath != null && iconPath != "")
                {
                    if (pointables[selectedObject].iconFromResource)
                        imageCenter.Source = Tools.ToBitmapSource(new System.Drawing.Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + iconPath)));
                    //imageCenter.Source = new BitmapImage(new Uri(MainWindow.resourcePath + iconPath, UriKind.RelativeOrAbsolute));
                    else
                        imageCenter.Source = new BitmapImage(new Uri(MainWindow.iconFolderPath + iconPath, UriKind.RelativeOrAbsolute));
                }
                else
                    imageCenter.Source = null;
            }
            catch { }
                                                
            resetAll(false);
            setUIActionVisible(false);

            if (pointables[selectedObject].locked)
            {
                buttonDeletePointable.IsEnabled = false;
            }
            else
            {
                buttonDeletePointable.IsEnabled = true;
            }

            if (pointables[selectedObject].isEnabled)
            {
                buttonDisable.Content = "Disable";
            }
            else
            {
                buttonDisable.Content = "Enable";
            }

            if (pointables[selectedObject].type == PointableObject.PointableType.Application)
            {
                buttonCalibrate.IsEnabled = false;
            }
            else
            {
                buttonCalibrate.IsEnabled = true;
            }

        }
        
        private void populatePointableListBox()
        {
            listBoxRefreshing = true;

            listBoxPointables.Items.Clear();
            foreach (PointableObject pointable in pointables)
            {
                if (pointable.isEnabled)
                {
                    string strItem = pointable.description;
                    if (pointable.type == PointableObject.PointableType.Physical && !pointable.calibrated)
                        strItem = strItem + "   [Not Calibrated]";

                    if (pointable.type == PointableObject.PointableType.Application)
                        strItem = "App-" + strItem;

                    if (pointable.type == PointableObject.PointableType.Application && (pointable.windowClass == null || pointable.windowClass == ""))
                        strItem = strItem + "   [Not Attached]";

                    listBoxPointables.Items.Add(strItem);
                }
                else
                {
                    string strItem = pointable.description;
                    strItem = strItem + "   [Disabled]";

                    if (pointable.type == PointableObject.PointableType.Application)
                        strItem = "App-" + strItem;

                    listBoxPointables.Items.Add(strItem);
                }
            }

            listBoxRefreshing = false;

            if (selectedObjectIndex < listBoxPointables.Items.Count)
                listBoxPointables.SelectedIndex = selectedObjectIndex;
            else
                listBoxPointables.SelectedIndex = 0;

            labelPointableCount.Content = listBoxPointables.Items.Count.ToString() + "/50";
        }


        internal WindowPointableEditor windowPointableEditor;
        private void pointableEdit(PointableObject pointableToEdit, bool newPointable)
        {
            if (this.windowPointableEditor == null)
            {
                this.windowPointableEditor = new WindowPointableEditor(pointableToEdit, newPointable, this);
                this.windowPointableEditor.Closed += (sender, args) => this.windowPointableEditor = null;
                this.windowPointableEditor.Left = this.Left + this.Width - this.windowPointableEditor.Width;
                this.windowPointableEditor.Top = this.Top + this.Height / 2 - this.windowPointableEditor.Height / 2;
                this.windowPointableEditor.ShowDialog();
            }
        }

        private void imageTop_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectTopAction(true);
        }
        private void selectTopAction(bool newSelection)
        {
            resetAll(true);
            imageTop.Opacity = 1.0;


            if (newSelection)
                selectAction(0);
        }
        private void imageRight_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectRightAction(true);
        }
        private void selectRightAction(bool newSelection)
        {
            resetAll(true);
            imageRight.Opacity = 1.0;

            if (newSelection)
                selectAction(1);
        }

        private void imageBottom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectBottomAction(true);
        }
        private void selectBottomAction(bool newSelection)
        {
            resetAll(true);
            imageBottom.Opacity = 1.0;

            if (newSelection)
                selectAction(2);
        }

        private void imageLeft2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectLeftAction(true);
        }
        private void selectLeftAction(bool newSelection)
        {
            resetAll(true);
            imageLeft.Opacity = 1.0;

            if (newSelection)
                 selectAction(3);
        }
        private void imageLeft_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }
        private void imageCenter_MouseUp(object sender, MouseButtonEventArgs e)
        {
            resetAll(true);
            setUIActionVisible(false);
            imageCenter.Opacity = 1.0;

            pointableEdit(pointables[selectedObjectIndex], false);
         }

        private void resetAll(bool grey)
        {
            try
            {
                double opacity = 1.0;
                if (grey) opacity = 0.3;

                imageTop.Opacity = opacity;
                imageRight.Opacity = opacity;
                imageBottom.Opacity = opacity;
                imageLeft.Opacity = opacity;
                imageCenter.Opacity = opacity;
            }
            catch { }
        }


        private void updateComboboxApplication()
        {
            comboBoxApplicationRefreshing = true;
            try
            {
                comboBoxApplication.Items.Clear();
            }
            catch { }
            //if (pointables[selectedObjectIndex].actions[selectedAction].actionType == Action.ActionType.LaunchApplication)
            //{
            //    comboBoxApplication.Items.Add(pointables[selectedObjectIndex].actions[selectedAction].description);
            //}else
            //{
            //    comboBoxApplication.Items.Add("");
            //}


            foreach (Action action in actionsApplication)
            {
                try
                {
                    PointableObject pointableTemp = mform1.getPointableofAction(action);
                    if (pointableTemp != null)
                        comboBoxApplication.Items.Add(action.description + "   - " + pointableTemp.description);
                    else
                        comboBoxApplication.Items.Add(action.description);
                }
                catch { }
            }
            //comboBoxApplication.SelectedIndex = 0;
            comboBoxApplicationRefreshing = false;
        }

        bool comboBoxKeystrokesRefreshing = false;
        bool noActionRefreshing = false;

        private void updateComboboxKeystrokes()
        { 
            comboBoxKeystrokesRefreshing = true;
            try
            {
                comboBoxKeystrokes.Items.Clear();
            }
            catch { }

            foreach (Action action in actionsKeystrokes)
            {
                try
                {
                    PointableObject pointableTemp = mform1.getPointableofAction(action);
                    if (pointableTemp != null)
                        comboBoxKeystrokes.Items.Add(action.description + "   - " + pointableTemp.description);
                    else
                        comboBoxKeystrokes.Items.Add(action.description);
                }
                catch { }
            }
            //comboBoxApplication.SelectedIndex = 0;

            comboBoxKeystrokesRefreshing = false;
        }

        private void setUIActionVisible(bool visible)
        {
            //comboBoxApplication.IsEnabled = visible;
            //buttonApplicationAdd.IsEnabled = visible;
            //comboBoxKeystrokes.IsEnabled = visible;
            //buttonKeystrokesAdd.IsEnabled = visible;

            tabControlAction.IsEnabled = visible;
        }

        int selectedAction= 0;
       // int selectedActionKeystroke = 0;
       // int selectedActionApplication = 0;

        private void selectAction(int i)
        {
            try
            {
                if (pointables[selectedObjectIndex].locked)
                {
                    setUIActionVisible(false);
                    return;
                }
                else
                {
                    setUIActionVisible(true);
                }

                selectedAction = i;

                //      comboBoxApplication.Text = pointables[selectedObjectIndex].action[i].description;

                //int index = actions.FindIndex(pointables[selectedObjectIndex].action[i]);
                if (pointables[selectedObjectIndex].actions[i].actionType == Action.ActionType.LaunchApplication)
                {
                    labelActionDescription.Content = pointables[selectedObjectIndex].actions[i].description;
                    // int index = actionsApplication.IndexOf(pointables[selectedObjectIndex].actions[i]);
                    // comboBoxApplication.SelectedIndex = index;
                    tabControlAction.SelectedIndex = 0;
                    //selectedActionKeystroke = index;
                }
                else if (pointables[selectedObjectIndex].actions[i].actionType == Action.ActionType.Keystroke)
                {
                    labelActionKeystrokeDescription.Content = pointables[selectedObjectIndex].actions[i].description;
                    //comboBoxKeystrokes.Items[0] = pointables[selectedObjectIndex].actions[i].description;
                    // comboBoxKeystrokes.SelectedIndex = 0;
                    // int index = actionsKeystrokes.IndexOf(pointables[selectedObjectIndex].actions[i]);
                    // comboBoxKeystrokes.SelectedIndex = index;
                    tabControlAction.SelectedIndex = 1;
                    //selectedActionKeystroke = index;
                }
                else if (pointables[selectedObjectIndex].actions[i].actionType == Action.ActionType.None)
                {
                    labelActionDescription.Content = pointables[selectedObjectIndex].actions[i].description;
                    tabControlAction.SelectedIndex = 0;
                }
            }
            catch { }
        }
        

        private void buttonAddPointable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pointables.Count < 50) //8
                {
                    PointableObject newPointable = new PointableObject("New Pointable", "pointable_generic.png",
                         null, null, null, null);
                    pointableEdit(newPointable, true);
                }
                else
                {
                    MessageBox.Show("Maximum Pointables reached");
                }
            }
            catch { }
        }


        private void buttonEditPointable_Click(object sender, RoutedEventArgs e)
        {
            pointableEdit(pointables[selectedObjectIndex], false);
        }

        private void buttonDeletePointable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pointables[selectedObjectIndex].locked) return;

                //MessageBox.Show("Do you really want to delete " + pointables[selectedObjectIndex].description + "?");

                if (MessageBox.Show("Do you really want to delete " + pointables[selectedObjectIndex].description + "?", "Delete Pointable", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    //do yes stuff
                }

                listBoxRefreshing = true;
                mform1.deletePointableFile(pointables[selectedObjectIndex]);
                //pointable delete file
                
                pointables.Remove(pointables[selectedObjectIndex]);

                selectedObjectIndex = 0;
                populatePointableListBox();
                populateUIObject(selectedObjectIndex);

                listBoxRefreshing = false;
                listBoxPointables.SelectedIndex = 0;
            }
            catch { }
        }
        
        private void buttonDisable_Click(object sender, RoutedEventArgs e)
        {
            if (pointables[selectedObjectIndex].isEnabled)
            {
                pointables[selectedObjectIndex].isEnabled = false;
                populatePointableListBox();
            }
            else
            {
                pointables[selectedObjectIndex].isEnabled = true;
                populatePointableListBox();

            }
            try
            {
                mform1.savePointableData(pointables[selectedObjectIndex]);
            }
            catch { }
        }


        private void buttonAddApplication_Click(object sender, RoutedEventArgs e)
        {

        }

        bool listBoxRefreshing = false;
        private void listBoxPointables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (listBoxRefreshing) return;

                selectedObjectIndex = listBoxPointables.SelectedIndex;

                populateUIObject(selectedObjectIndex);
            }
            catch { }
        }

        WindowApplicationEdit windowApplicationEdit;
        private void buttonApplicationEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Action actionToEdit = pointables[selectedObjectIndex].actions[selectedAction];
                openApplicationEditWindow(actionToEdit, false);
            }
            catch { }
        }
        private void openApplicationEditWindow(Action actionToEdit, bool newApp)
        {
            try
            {
                if (this.windowApplicationEdit == null)
                {
                    this.windowApplicationEdit = new WindowApplicationEdit(actionToEdit, newApp, this);
                    this.windowApplicationEdit.Closed += (sender, args) => this.windowApplicationEdit = null;
                    this.windowApplicationEdit.Left = this.Left + this.Width - this.windowApplicationEdit.Width;
                    this.windowApplicationEdit.Top = this.Top + this.Height / 2 - this.windowApplicationEdit.Height / 2;
                    this.windowApplicationEdit.ShowDialog();
                }
            }
            catch { }
        }

        internal void saveApplication(Action newAction)
        {
            try
            {
                //actionsApplication.Add(newAction);
                pointables[selectedObjectIndex].actions[selectedAction] = newAction;

                addActionsApplication(newAction);
                saveApplication();
            }
            catch { }
           // comboBoxApplicationRefreshing = true;
            //comboBoxApplication.SelectedIndex = comboBoxApplication.Items.Count - 1;
           // comboBoxApplicationRefreshing = false;
        }
        internal void saveApplication()
        {
            try
            {
                mform1.savePointableData(pointables[selectedObjectIndex]);

                labelActionDescription.Content = pointables[selectedObjectIndex].actions[selectedAction].description;
                updateComboboxApplication();
                resetObjectIconsOnly(selectedObjectIndex);
            }
            catch { }
            //populateUIObject(selectedObjectIndex);
            //switch (selectedAction)
            //{
            //    case 0:
            //        selectTopAction(false);
            //        break;
            //    case 1:
            //        selectRightAction(false);
            //        break;
            //    case 2:
            //        selectBottomAction(false);
            //        break;
            //    case 3:
            //        selectLeftAction(false);
            //        break;
            //}
        }

        internal void saveKeystrokes(Action newAction)
        {
           // actionsKeystrokes.Add(newAction);
            try
            {
                pointables[selectedObjectIndex].actions[selectedAction] = newAction;
                addActionsKeystroke(newAction);

                saveKeystrokes();
            }

            catch { }
            //comboBoxKeystrokesRefreshing = true;
            //comboBoxKeystrokes.SelectedIndex = comboBoxKeystrokes.Items.Count - 1;
            //comboBoxKeystrokesRefreshing = false;
        }
        internal void saveKeystrokes()
        {
            try
            {
                mform1.savePointableData(pointables[selectedObjectIndex]);

                labelActionKeystrokeDescription.Content = pointables[selectedObjectIndex].actions[selectedAction].description;
                updateComboboxKeystrokes();
                resetObjectIconsOnly(selectedObjectIndex);
            }

            catch { }
            //updateComboboxKeystrokes(); 
            //populateUIObject(selectedObjectIndex);
            //switch (selectedAction)
            //{
            //    case 0:
            //        selectTopAction(false);
            //        break;
            //    case 1:
            //        selectRightAction(false);
            //        break;
            //    case 2:
            //        selectBottomAction(false);
            //        break;
            //    case 3:
            //        selectLeftAction(false);
            //        break;
            //}
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }



        internal void savePointable(PointableObject newPointable)
        {
            try
            {
                // pointables.Add(new PointableObject("Screen", "screen.png",
                //      actionLaunchPowerPoint, actionLaunchWord, actionLaunchExcel, actionLaunchChrome));
                if (!pointables.Contains(newPointable))
                {
                    pointables.Add(newPointable);

                    populatePointableListBox();

                    selectedObjectIndex = pointables.Count - 1;
                    listBoxPointables.SelectedIndex = selectedObjectIndex;
                    populateUIObject(selectedObjectIndex);
                }
                else
                {
                    populatePointableListBox();
                    populateUIObject(selectedObjectIndex);

                }


                mform1.savePointableData(newPointable);
            }
            catch { }
        }

        internal bool checkPointableNameExist(string pointableName, PointableObject currentPointable)
        {
            foreach (PointableObject pointable in pointables)
            {
                if (currentPointable != pointable)
                {
                    if (pointable.description == pointableName)
                        return true;
                }
            }
            return false;
        }


        private void buttonApplicationDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pointables[selectedObjectIndex].actions[selectedAction] = actionEmpty();
                actionsApplication.RemoveAt(comboBoxApplication.SelectedIndex);

                updateComboboxApplication();
                populateUIObject(selectedObjectIndex);
            }
            catch { }
        }



        private static Action actionEmpty()
        {
            Action newAction = new Action("", "", "", "");
            newAction.actionType = Action.ActionType.None;
            return newAction;
        }

        internal static Action actionEmptyApplication()
        {
            return new Action("", "", Action.ActionType.LaunchApplication);
        }
        internal static Action actionEmptyKeystroke()
        {
            return new Action("", "", Action.ActionType.Keystroke);
        }

        private void buttonApplicationAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Action actionToEdit = actionEmptyApplication();// pointables[selectedObjectIndex].action[selectedAction];

                openApplicationEditWindow(actionToEdit, true);
            }
            catch { }
        }

        private void addActionsApplication(Action actionToAdd)
        {
            try
            {
                PointableObject pointableOfAction = mform1.getPointableofAction(actionToAdd);

                foreach (Action actionInPointable in pointableOfAction.actions)
                {
                    if (actionInPointable != actionToAdd && actionInPointable.description == actionToAdd.description)
                    {//if already exist in the same pointable then ignore.
                        return;
                    }
                }

                if (actionToAdd.description != "")
                    actionsApplication.Add(actionToAdd);
            }
            catch { }
        }

        private void addActionsKeystroke(Action actionToAdd)
        {
            try
            {
                PointableObject pointableOfAction = mform1.getPointableofAction(actionToAdd);

                foreach (Action actionInPointable in pointableOfAction.actions)
                {
                    if (actionInPointable != actionToAdd && actionInPointable.description == actionToAdd.description)
                    {//if already exist in the same pointable then ignore.
                        return;
                    }
                }

                if (actionToAdd.description != "")
                    actionsKeystrokes.Add(actionToAdd);
            }
            catch { }
        }

        internal bool comboBoxApplicationRefreshing = false;


        private void comboBoxApplication_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
           // return;//
            try
            {
                if (comboBoxApplication.SelectedIndex == -1) return;
                // if (comboBoxApplicationRefreshing || comboBoxKeystrokesRefreshing || noActionRefreshing) return;

                comboBoxApplicationRefreshing = true;
                int selection = comboBoxApplication.SelectedIndex;
                comboBoxApplication.SelectedIndex = -1;

                //if (selection == 0)
                //{
                //    comboBoxApplicationRefreshing = false;
                //    return;
                //}
                //if (comboBoxApplication.SelectedIndex < 0) comboBoxApplication.SelectedIndex = 0;
                if (pointables[selectedObjectIndex].actions[selectedAction] != actionsApplication[selection])
                {
                    pointables[selectedObjectIndex].actions[selectedAction] = Action.createAction(actionsApplication[selection]);
                    labelActionDescription.Content = pointables[selectedObjectIndex].actions[selectedAction].description;

                    addActionsApplication(pointables[selectedObjectIndex].actions[selectedAction]);
                }
                //populateUIObject(selectedObjectIndex);
                resetObjectIconsOnly(selectedObjectIndex);

                updateComboboxApplication();


                try
                {
                    mform1.savePointableData(pointables[selectedObjectIndex]);
                }
                catch { }
                //switch (selectedAction)
                //{
                //    case 0:
                //        selectTopAction(false);
                //        break;
                //    case 1:
                //        selectRightAction(false);
                //        break;
                //    case 2:
                //        selectBottomAction(false);
                //        break;
                //    case 3:
                //        selectLeftAction(false);
                //        break;
                //}

            }
            catch { }
            comboBoxApplicationRefreshing = false;
        }


        internal bool checkActionNameExist(string actionDescription, Action currentAction)
        {
            if (currentAction.actionType == Action.ActionType.LaunchApplication)
            {
                foreach (Action action in actionsApplication)
                {
                    if (currentAction != action)
                    {
                        if (action.description == actionDescription)
                            return true;
                    }
                }
            }
            else
            {
                foreach (Action action in actionsKeystrokes)
                {
                    if (currentAction != action)
                    {
                        if (action.description == actionDescription)
                            return true;
                    }
                }
            }
            return false;
        }

        private void buttonKeystrokesEdit_Click(object sender, RoutedEventArgs e)
        {            
            //Action actionToEdit = actionEmpty();// pointables[selectedObjectIndex].action[selectedAction];
            try
            {
                Action actionToEdit = pointables[selectedObjectIndex].actions[selectedAction];
                openWindowNewKeystrokes(actionToEdit, false);
            }
            catch { }
        }
        private void buttonKeystrokesDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pointables[selectedObjectIndex].actions[selectedAction] = actionEmpty();

                actionsKeystrokes.RemoveAt(comboBoxKeystrokes.SelectedIndex);

                updateComboboxKeystrokes();
                populateUIObject(selectedObjectIndex);
            }
            catch { }

        }
        private void buttonAddKeystrokes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Action actionToEdit = actionEmptyKeystroke();// pointables[selectedObjectIndex].action[selectedAction];

                openWindowNewKeystrokes(actionToEdit, true);
            }
            catch { }
        }
        private void comboBoxKeystrokes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (comboBoxKeystrokes.SelectedIndex == -1) return;
                // if (comboBoxApplicationRefreshing || comboBoxKeystrokesRefreshing || noActionRefreshing) return;

                comboBoxKeystrokesRefreshing = true;
                int selection = comboBoxKeystrokes.SelectedIndex;
                comboBoxKeystrokes.SelectedIndex = -1;

                //if (selection == 0)
                //{
                //    comboBoxApplicationRefreshing = false;
                //    return;
                //}
                //if (comboBoxApplication.SelectedIndex < 0) comboBoxApplication.SelectedIndex = 0;
                if (pointables[selectedObjectIndex].actions[selectedAction] != actionsKeystrokes[selection])
                {
                    pointables[selectedObjectIndex].actions[selectedAction] = Action.createAction(actionsKeystrokes[selection]);
                    labelActionKeystrokeDescription.Content = pointables[selectedObjectIndex].actions[selectedAction].description;

                    addActionsKeystroke(pointables[selectedObjectIndex].actions[selectedAction]);
                }
                //populateUIObject(selectedObjectIndex);
                resetObjectIconsOnly(selectedObjectIndex);

                updateComboboxKeystrokes();
                //if (comboBoxKeystrokesRefreshing || comboBoxApplicationRefreshing || noActionRefreshing) return;

                //comboBoxKeystrokesRefreshing = true;

                //if (comboBoxKeystrokes.SelectedIndex < 0) comboBoxKeystrokes.SelectedIndex = 0;
                //pointables[selectedObjectIndex].actions[selectedAction] = actionsKeystrokes[comboBoxKeystrokes.SelectedIndex];

                //populateUIObject(selectedObjectIndex);

                //switch (selectedAction)
                //{
                //    case 0:
                //        selectTopAction(false);
                //        break;
                //    case 1:
                //        selectRightAction(false);
                //        break;
                //    case 2:
                //        selectBottomAction(false);
                //        break;
                //    case 3:
                //        selectLeftAction(false);
                //        break;
                //}

                try
                {
                    mform1.savePointableData(pointables[selectedObjectIndex]);
                }
                catch { }
            }
            catch { }
            comboBoxKeystrokesRefreshing = false;
        }
        WindowNewKeystrokes windowNewKeystrokes;
        private void openWindowNewKeystrokes(Action actionToEdit, bool newKeystrokes)
        {
            try
            {
                if (this.windowNewKeystrokes == null)
                {
                    this.windowNewKeystrokes = new WindowNewKeystrokes(actionToEdit, newKeystrokes, this);
                    this.windowNewKeystrokes.Closed += (sender, args) => this.windowNewKeystrokes = null;
                    this.windowNewKeystrokes.Left = this.Left + this.Width - this.windowNewKeystrokes.Width;
                    this.windowNewKeystrokes.Top = this.Top + this.Height / 2 - this.windowNewKeystrokes.Height / 2;
                    this.windowNewKeystrokes.ShowDialog();
                }
            }
            catch { }
        }

        private void tabControlAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             return;//
            //switch between
            //
            if (comboBoxApplicationRefreshing || comboBoxKeystrokesRefreshing || noActionRefreshing ) return;

            if (tabControlAction.SelectedIndex == 0)//launch
            {
                comboBoxApplicationRefreshing = true;

                //if (comboBoxApplication.SelectedIndex < 0) comboBoxApplication.SelectedIndex = 0;
                pointables[selectedObjectIndex].actions[selectedAction] = Action.createAction(actionsApplication[comboBoxApplication.SelectedIndex]); 
                //pointables[selectedObjectIndex].actions[selectedAction].description = 

                labelActionDescription.Content = pointables[selectedObjectIndex].actions[selectedAction].description;

                resetObjectIconsOnly(selectedObjectIndex);

                //switch (selectedAction)
                //{
                //    case 0:
                //        selectTopAction(false);
                //        break;
                //    case 1:
                //        selectRightAction(false);
                //        break;
                //    case 2:
                //        selectBottomAction(false);
                //        break;
                //    case 3:
                //        selectLeftAction(false);
                //        break;
                //}

                try
                {
                    mform1.savePointableData(pointables[selectedObjectIndex]);
                }
                catch { }

                comboBoxApplicationRefreshing = false;
            }
            else if (tabControlAction.SelectedIndex == 1)//key       
            {
                comboBoxKeystrokesRefreshing = true;

                //if (comboBoxKeystrokes.SelectedIndex < 0) comboBoxKeystrokes.SelectedIndex = 0;
                pointables[selectedObjectIndex].actions[selectedAction] = Action.createAction(actionsKeystrokes[comboBoxKeystrokes.SelectedIndex]);

                labelActionKeystrokeDescription.Content = pointables[selectedObjectIndex].actions[selectedAction].description;

                resetObjectIconsOnly(selectedObjectIndex);

                //switch (selectedAction)
                //{
                //    case 0:
                //        selectTopAction(false);
                //        break;
                //    case 1:
                //        selectRightAction(false);
                //        break;
                //    case 2:
                //        selectBottomAction(false);
                //        break;
                //    case 3:
                //        selectLeftAction(false);
                //        break;
                //}

                try
                {
                    mform1.savePointableData(pointables[selectedObjectIndex]);
                }
                catch { }

                comboBoxKeystrokesRefreshing = false;
            }
        }

        private void buttonCalibrate_Click(object sender, RoutedEventArgs e)
        {
            mform1.startCalibration(pointables[selectedObjectIndex]);
        }

        internal void calibrationUpdate(PointableObject currentObject)
        {
           // this.IsEnabled = true;
            populatePointableListBox();
           //int index = pointables.IndexOf(currentObject);


           //string strItem = currentObject.description;
           //if (!currentObject.calibrated)
           //    strItem = strItem + "   [Not Calibrated]";

           //listBoxPointables.Items.GetItemAt(index) = strItem;
        }

        //private void selectAction(int selectedAction)
        //{
        //    //check contents of pointable-action.
        //    //update tabcontrol selectedindex
        //    //if application - update combobox
        //        //first value = pointable-action-description

        //}

        #region new Action ui
        private void labelDescription_Click(object sender, RoutedEventArgs e)
        {
            comboBoxApplication.IsDropDownOpen = true;
        }

        #endregion

        private void labelKeystrokeDescription_Click(object sender, MouseButtonEventArgs e)
        {
            comboBoxKeystrokes.IsDropDownOpen = true;
        }



    }
}
