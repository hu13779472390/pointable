using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PointableUI;
using System.Threading;
using System.IO;
using System.Media;
using System.Drawing.Drawing2D;
using Pointable;

namespace LeapProject
{
    public partial class FormTutorial : Form
    {

        public enum CalibrationStates { None, First, Second, Calibrated };
        public enum CalibrationEvents { Start, Next, Cancel };

        //protected override void WndProc(ref Message message)
        //{
        //    const int WM_SYSCOMMAND = 0x0112;
        //    const int SC_MOVE = 0xF010;

        //    switch(message.Msg)
        //    {
        //        case WM_SYSCOMMAND:
        //           int command = message.WParam.ToInt32() & 0xfff0;
        //           if (command == SC_MOVE)
        //              return;
        //           break;
        //    }

        //    base.WndProc(ref message);
        //}

        Form1 mainForm;

        internal static bool isAudio = false;
        internal static bool isLast = false;

        public FormTutorial(Form1 _parentForm)
        {
            mainForm = _parentForm;
            InitializeComponent();
            isAudio = false;
            isLast = false;

            try
            {
                checkTab();

                this.TransparencyKey = Color.FromArgb(227, 6, 19);

                trackBarClickSensitivity.Value = Tools.TapSensitivity;
            }
            catch { }
        }


        private void FormCalibrate_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Form1.detectionActivated = true;
                clearTutorialDragWindow();
                isAudio = false;
                stopMusic();
                mainForm.closeTutorial();
            }
            catch { }
        }



        internal void ShowSlow()
        {
            try
            {
                this.Opacity = 0;
                timerSlow.Start();
                this.Show();
            }
            catch { }
        }

        private void timerSlow_Tick(object sender, EventArgs e)
        {
            try
            {
                double currentOpacity = this.Opacity;
                currentOpacity += 0.05;

                if (currentOpacity > 1)
                {
                    currentOpacity = 1;
                    timerSlow.Stop();
                }
                this.Opacity = currentOpacity;
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                this.Visible = false;
                mainForm.startCalibrateScreen();
            }
            catch { }
        }


        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (isClickGame)
                    this.Close();

                if (tabControl1.SelectedIndex == tabControl1.TabCount - 1)
                    this.Close();
                else
                    tutorialNext();
            }
            catch { }
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            tutorialPrevious();
        }

        private void tutorialPrevious()
        {
            try
            {
                //if (tabControl1.SelectedIndex > 3)
                //    tabControl1.SelectedIndex = 3;

                if ((tabControl1.SelectedIndex > 0 && tabControl1.SelectedIndex < 3) || tabControl1.SelectedIndex > 3)
                    tabControl1.SelectedIndex = tabControl1.SelectedIndex - 1;
                
                checkTab();
            }
            catch { }
        }

        internal void tutorialNext()
        {
            try
            {
                if (tabControl1.SelectedIndex == 1)
                {
                    Form1.detectionActivated = true;
                    tabControl1.SelectedIndex = 2;
                }

                if (tabControl1.SelectedIndex < tabControl1.TabCount - 1)
                    tabControl1.SelectedIndex = tabControl1.SelectedIndex + 1;

                checkTab();
            }
            catch { }
        }

        internal static int tutorialPointableProgress = 0;

        internal void setTutorialPointableAnimation(int state)
        {
            tutorialPointableProgress = state;


            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    switch (tutorialPointableProgress)
                    {
                        case 0: //open palm
                            pictureBoxPointableOpen.Visible = true;
                            pictureBoxPointableSelect.Visible = false;
                            pictureBoxPointableCircular.Visible = false;

                            pictureBoxTextMute.Visible = false;
                            pictureBoxTextUp.Visible = false;
                            pictureBoxTextDown.Visible = false;
                            pictureBoxTextCircle.Visible = false;
                            pictureBoxTextNext.Visible = false;
                            break;
                        case 1: //mute unmute
                            pictureBoxPointableOpen.Visible = false;
                            pictureBoxPointableSelect.Visible = true;
                            pictureBoxPointableCircular.Visible = false;

                            pictureBoxTextMute.Visible = true;
                            pictureBoxTextUp.Visible = false;
                            pictureBoxTextDown.Visible = false;
                            pictureBoxTextCircle.Visible = false;
                            pictureBoxTextNext.Visible = false;
                            break;
                        case 2: //up 
                            pictureBoxPointableOpen.Visible = false;
                            pictureBoxPointableSelect.Visible = true;
                            pictureBoxPointableCircular.Visible = false;

                            pictureBoxTextMute.Visible = false;
                            pictureBoxTextUp.Visible = true;
                            pictureBoxTextDown.Visible = false;
                            pictureBoxTextCircle.Visible = false;
                            pictureBoxTextNext.Visible = false;
                            break;

                        case 3: //down
                            pictureBoxPointableOpen.Visible = false;
                            pictureBoxPointableSelect.Visible = true;
                            pictureBoxPointableCircular.Visible = false;

                            pictureBoxTextMute.Visible = false;
                            pictureBoxTextUp.Visible = false;
                            pictureBoxTextDown.Visible = true;
                            pictureBoxTextCircle.Visible = false;
                            pictureBoxTextNext.Visible = false;
                            break;

                        case 4: //circle
                            pictureBoxPointableOpen.Visible = false;
                            pictureBoxPointableSelect.Visible = false;
                            pictureBoxPointableCircular.Visible = true;

                            pictureBoxTextMute.Visible = false;
                            pictureBoxTextUp.Visible = false;
                            pictureBoxTextDown.Visible = false;
                            pictureBoxTextCircle.Visible = true;
                            pictureBoxTextNext.Visible = false;
                            break;

                        case 5: //circle
                            pictureBoxPointableOpen.Visible = false;
                            pictureBoxPointableSelect.Visible = false;
                            pictureBoxPointableCircular.Visible = true;

                            pictureBoxTextMute.Visible = false;
                            pictureBoxTextUp.Visible = false;
                            pictureBoxTextDown.Visible = false;
                            pictureBoxTextCircle.Visible = true;
                            pictureBoxTextNext.Visible = false;
                            break;

                        case 6: //next
                            pictureBoxPointableOpen.Visible = false;
                            pictureBoxPointableSelect.Visible = false;
                            pictureBoxPointableCircular.Visible = true;

                            pictureBoxTextMute.Visible = false;
                            pictureBoxTextUp.Visible = false;
                            pictureBoxTextDown.Visible = false;
                            pictureBoxTextCircle.Visible = false;
                            pictureBoxTextNext.Visible = true;
                            break;
                    }
                }
                catch { }
            });
        }
        private void checkTab()
        {
            try {

                timerColorAnimate.Enabled = false;
                clearTutorialDragWindow();

                switch (tabControl1.SelectedIndex)
                {
                    case 1:

                        if (mainForm.firstRun)
                        {
                            mainForm.startCalibrateScreen();
                            this.Visible = false;

                            tabControl1.SelectedIndex = 2;
                        }

                        //buttonNext.Text = "Next";
                        buttonPrevious.Enabled = true;
                        buttonDone.Visible = false;
                        buttonNext.Visible = true;

                        break;
                    case 3: //speakers example

                        isLast = false;
                        if (isAudio == false)
                        {
                            isAudio = true;
                            setTutorialPointableAnimation(0);

                            if (mainForm.state.State == Form1.FiniteStateMachine.States.WindowPointable)
                                mainForm.doStateWindowPointable();
                            //startMusic();
                        }
                        //buttonNext.Text = "Next";
                        buttonPrevious.Enabled = false;
                        buttonDone.Visible = false;
                        buttonNext.Visible = true;
                        break;
                    case 4: //scroll

                        if (isAudio)
                        {
                            isAudio = false;
                            if (mainForm.state.State == Form1.FiniteStateMachine.States.WindowPointable)
                                mainForm.doStateWindowPointable();
                            stopMusic();
                        }
                        else if (isLast)
                        {
                            isLast = false;
                            if (mainForm.state.State == Form1.FiniteStateMachine.States.WindowPointable)
                                mainForm.doStateWindowPointable();
                            stopMusic();
                        }

                        //buttonNext.Text = "Next";
                        buttonPrevious.Enabled = true;
                        buttonDone.Visible = false;
                        buttonNext.Visible = true;
                        //textBox1.Select(0, 1);
                        flowLayoutPanel1.Focus();
                        break;
                    case 5: //click
                        if (isAudio)
                        {
                            isAudio = false;
                            if (mainForm.state.State == Form1.FiniteStateMachine.States.WindowPointable)
                                mainForm.doStateWindowPointable();
                            stopMusic();
                        }
                        else if (isLast)
                        {
                            isLast = false;
                            if (mainForm.state.State == Form1.FiniteStateMachine.States.WindowPointable)
                                mainForm.doStateWindowPointable();
                            stopMusic();
                        }

                        timerColorAnimate.Enabled = true;
                        //buttonNext.Text = "Next";
                        buttonPrevious.Enabled = true;
                        buttonDone.Visible = false;
                        buttonNext.Visible = true;
                        break;
                    case 7:
                        if (isAudio)
                        {
                            isAudio = false;
                            if (mainForm.state.State == Form1.FiniteStateMachine.States.WindowPointable)
                                mainForm.doStateWindowPointable();
                            stopMusic();
                        }
                        if (!isLast)
                        {
                            isLast = true;
                            if (mainForm.state.State == Form1.FiniteStateMachine.States.WindowPointable)
                                mainForm.doStateWindowPointable();
                            stopMusic();
                        }

                        buttonPrevious.Enabled = true;
                        buttonDone.Visible = true;
                        buttonNext.Visible = false;
                        //buttonNext.Text = "OK";
                        break;
                    case 0:
                        if (isAudio)
                        {
                            isAudio = false;
                            if (mainForm.state.State == Form1.FiniteStateMachine.States.WindowPointable)
                                mainForm.doStateWindowPointable();
                            stopMusic();
                        }
                        if (isLast)
                        {
                            isLast = false;
                            if (mainForm.state.State == Form1.FiniteStateMachine.States.WindowPointable)
                                mainForm.doStateWindowPointable();
                            stopMusic();
                        }

                        //buttonNext.Text = "Next";
                        buttonPrevious.Enabled = false;
                        buttonDone.Visible = false;
                        buttonNext.Visible = true;
                        break;                        
                    default:
                        if (isAudio)
                        {
                            isAudio = false;
                            if (mainForm.state.State == Form1.FiniteStateMachine.States.WindowPointable)
                                mainForm.doStateWindowPointable();
                            stopMusic();
                        }
                        else if (isLast)
                        {
                            isLast = false;
                            if (mainForm.state.State == Form1.FiniteStateMachine.States.WindowPointable)
                                mainForm.doStateWindowPointable();
                            stopMusic();
                        }

                        //buttonNext.Text = "Next";
                        buttonPrevious.Enabled = true;
                        buttonDone.Visible = false;
                        buttonNext.Visible = true;
                        break;
                }
            }
            catch{}
        }

        private void tutorialMusic()
        {
            try
            {
                tabControl1.SelectedIndex = 3;                
                checkTab();
            }
            catch { }
        }
        private void FormTutorial_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.D6)
                {
                    tutorialNext();
                }
                else if (e.KeyChar == (char)Keys.D4)
                {
                    tutorialPrevious();
                }
                else if (e.KeyChar == (char)Keys.D8)
                {
                    tutorialMusic();
                }
            }
            catch { }
        }

        SoundPlayer snd;
        private void startMusic()
        {            
            try
            { 
                if (snd != null)
                    snd.Stop();

                if (snd == null)
                {
                    Stream str = Pointable.Properties.Resources.TutorialMusic;
                    snd = new SoundPlayer(str);
                }

                snd.PlayLooping();
            }
            catch { }
        }
        internal void startMusicIfNotPlaying()
        {
            if (snd == null)
            {
                startMusic();
            }
        }
        private void stopMusic()
        {          
            try
            {
                if (snd != null)
                {
                    snd.Stop();
                    snd.Dispose();
                    snd = null;
                }
            }
            catch{}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            startMusic();
        }

        private void panelColourBig_Click(object sender, EventArgs e)
        {
            
        }

        private void panelColor1_Click(object sender, EventArgs e)
        {
            colorProcess(1);
        }

        private void panelColor2_Click(object sender, EventArgs e)
        {

            colorProcess(2);
        }

        private void panelColor3_Click(object sender, EventArgs e)
        {

            colorProcess(3);
        }

        private void panelColor4_Click(object sender, EventArgs e)
        {

            colorProcess(4);
        }

        private void panelColor5_Click(object sender, EventArgs e)
        {

            colorProcess(5);
        }

        private void panelColor6_Click(object sender, EventArgs e)
        {

            colorProcess(6);
        }

        private void buttonClick2_Click(object sender, EventArgs e)
        {

        }

        private void buttonClick1_Click(object sender, EventArgs e)
        {

        }

        int currentColorIndex = 1;
        Color currentColor = Color.FromArgb(255, 255, 192);

        private void resetColorAll()
        {
            panelColor1.BorderStyle = BorderStyle.None;
            panelColor2.BorderStyle = BorderStyle.None;
            panelColor3.BorderStyle = BorderStyle.None;
            panelColor4.BorderStyle = BorderStyle.None;
            panelColor5.BorderStyle = BorderStyle.None;
            panelColor6.BorderStyle = BorderStyle.None;
        }
        private void colorProcess(int i)
        {
            try
            {
                resetColorAll();

                switch (i)
                {
                    case 0:

                        break;
                    case 1:
                        //currentColor = panelColor1.BackColor;
                        panelColor1.BorderStyle = BorderStyle.Fixed3D;
                        break;
                    case 2:
                        currentColor = panelColor2.BackColor;
                        panelColor2.BorderStyle = BorderStyle.Fixed3D;
                        break;
                    case 3:
                        currentColor = panelColor3.BackColor;
                        panelColor3.BorderStyle = BorderStyle.Fixed3D;
                        break;
                    case 4:
                        currentColor = panelColor4.BackColor;
                        panelColor4.BorderStyle = BorderStyle.Fixed3D;
                        break;
                    case 5:
                        currentColor = panelColor5.BackColor;
                        panelColor5.BorderStyle = BorderStyle.Fixed3D;
                        break;
                    case 6:
                        currentColor = panelColor6.BackColor;
                        panelColor6.BorderStyle = BorderStyle.Fixed3D;
                        break;
                }

                currentColorIndex = i;
                //currentColor = panelColorBig.BackColor;
            }
            catch { }
        }        



        private void FormTutorial_Load(object sender, EventArgs e)
        {

        }

        int counter = 0;

        private void pictureBoxColorBig_MouseDown(object sender, MouseEventArgs e)
        {
            clickTrigger(new Point(e.X, e.Y));
        }

        private void labelClickMe_MouseDown(object sender, MouseEventArgs e)
        {
            //timerColorAnimate.Enabled = true;
            try
            {
                labelClickMe.Visible = false;
                Point point = new Point();
                point.X = labelClickMe.Left - pictureBoxColorBig.Left + e.X;
                point.Y = labelClickMe.Top - pictureBoxColorBig.Top + e.Y;
                clickTrigger(point);
            }
            catch { }
        }



        List<FormTutorialDrag> tutorialDrag = new List<FormTutorialDrag>();

        private void buttonCreateWindow_Click(object sender, EventArgs e)
        {
            generateTestWindow();
        }

        private void generateTestWindow()
        {
            try
            {
                pictureBoxDialogGenerateWindow.Visible = false;

                if (tutorialDrag.Count > 0)
                {
                    for (int i = tutorialDrag.Count - 1; i > -1; i--)
                    {
                        if (tutorialDrag[i].IsDisposed)
                        {
                            tutorialDrag.RemoveAt(i);
                        }
                    }
                }

                if (tutorialDrag.Count < 4)
                {
                    FormTutorialDrag tutorialDragLocal = new FormTutorialDrag();

                    if (tutorialDrag.Count == 0)
                    {
                        tutorialDragLocal.Left = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].WorkingArea.Left;
                        tutorialDragLocal.Top = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].WorkingArea.Top;
                    }
                    else if (tutorialDrag.Count == 1)
                    {
                        tutorialDragLocal.Left = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].WorkingArea.Right - tutorialDragLocal.Width;
                        tutorialDragLocal.Top = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].WorkingArea.Top;
                    }
                    else if (tutorialDrag.Count == 2)
                    {
                        tutorialDragLocal.Left = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].WorkingArea.Left;
                        tutorialDragLocal.Top = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].WorkingArea.Bottom - tutorialDragLocal.Height;
                    }
                    else if (tutorialDrag.Count == 3)
                    {
                        tutorialDragLocal.Left = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].WorkingArea.Right - tutorialDragLocal.Width; ;
                        tutorialDragLocal.Top = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].WorkingArea.Bottom - tutorialDragLocal.Height; ;
                    }

                    //tutorialDragLocal.MdiParent = this.MdiParent;
                    tutorialDragLocal.Show();
                    tutorialDrag.Add(tutorialDragLocal);
                }
            }
            catch { }
        }

        internal void checkTestWindowOnTop()
        {
            try
            {
                foreach (FormTutorialDrag form in tutorialDrag)
                {
                    Tools.SetWindowPos(form.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);

                }
            }
            catch { }
        }

        private void pictureBoxDialogGenerateWindow_Click(object sender, EventArgs e)
        {
            generateTestWindow();
        }

        private void clearTutorialDragWindow()
        {
            try
            {
                if (tutorialDrag.Count > 0)
                {
                    for (int i = tutorialDrag.Count - 1; i > -1; i--)
                    {
                        try
                        {
                            if (!tutorialDrag[i].IsDisposed)
                            {
                                tutorialDrag[i].Close();
                                tutorialDrag.RemoveAt(i);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        private void buttonWindowClose_Click(object sender, EventArgs e)
        {
            try
            {
                clearTutorialDragWindow();
            }
            catch { }
        }

        private void flowLayoutPanel1_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                flowLayoutPanel1.Focus();
            }
            catch { }
        }

        private void tabPage5_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                flowLayoutPanel1.Focus();
            }
            catch { }
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            try
            {
                mainForm.openGuide();
                this.Close();
            }
            catch { }
        }

        bool isClickGame = false;

        internal void runClickGame()
        {
            try
            {
                isClickGame = true;
                tabControl1.SelectedIndex = 5;
                buttonPrevious.Visible = false;
                buttonNext.Image = buttonDone.Image;
                buttonNext.Visible = true;
            }
            catch { }
            //button1.Visible = true;
        }

        private void trackBarClickSensitivity_Scroll(object sender, EventArgs e)
        {            
           // Console.WriteLine(trackBarClickSensitivity.Value);
          //  Tools.TapSensitivity = trackBarClickSensitivity.Value;
          //  mainForm.setControllerConfiguration();
        }

        private void trackBarClickSensitivity_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int value = (int)Math.Round(e.X * (trackBarClickSensitivity.Maximum - trackBarClickSensitivity.Minimum) / (double)trackBarClickSensitivity.Width + trackBarClickSensitivity.Minimum);
                if (value < trackBarClickSensitivity.Minimum)
                    value = trackBarClickSensitivity.Minimum;
                if (value > trackBarClickSensitivity.Maximum)
                    value = trackBarClickSensitivity.Maximum;

                trackBarClickSensitivity.Value = value;
            }
            catch { }
        }

        private void trackBarClickSensitivity_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                int value = (int)Math.Round(e.X * (trackBarClickSensitivity.Maximum - trackBarClickSensitivity.Minimum) / (double)trackBarClickSensitivity.Width + trackBarClickSensitivity.Minimum);
                if (value < trackBarClickSensitivity.Minimum)
                    value = trackBarClickSensitivity.Minimum;
                if (value > trackBarClickSensitivity.Maximum)
                    value = trackBarClickSensitivity.Maximum;

                trackBarClickSensitivity.Value = value;
                Tools.TapSensitivity = trackBarClickSensitivity.Value;
                mainForm.setControllerConfiguration();

                pictureBoxColorBig.Focus();
            }
            catch { }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch { }
        }

        
        private void buttonGameStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (gameState == GameState.Tutorial || gameState == GameState.End)
                {
                    gameState = GameState.Game;

                    buttonGameStart.Text = "End";
                    counter = 0;
                    labelClickScore.Text = counter.ToString();

                    gameCounter = 30;
                    labelGameCountdown.Text = "30s";
                    labelGameCountdown.Visible = true;

                    timerColorAnimate.Enabled = true;
                    timerGameCounter.Enabled = true;

                    labelClickScore.Visible = true;
                }
                else if (gameState == GameState.Game)
                {
                    gameState = GameState.Tutorial;
                    buttonGameStart.Text = "Start";

                    counter = 0;
                    labelClickScore.Text = counter.ToString();

                    labelGameCountdown.Visible = false;
                    timerGameCounter.Enabled = false;


                    labelClickScore.Visible = true;
                    //start timer
                }
            }
            catch { }
        }

        List<Point> ballPoints = new List<Point>();
        List<int> ballSize = new List<int>();
        List<Color> ballColor = new List<Color>();

        Random rand = new Random();

        private void addBall(Point position)
        {
            try
            {
                Color colorBall = currentColor;
                if (currentColorIndex == 1)
                {
                    int randomNumber = rand.Next(2, 7);
                    switch (randomNumber)
                    {
                        case 2:
                            colorBall = panelColor2.BackColor;
                            break;
                        case 3:
                            colorBall = panelColor3.BackColor;
                            break;
                        case 4:
                            colorBall = panelColor4.BackColor;
                            break;
                        case 5:
                            colorBall = panelColor5.BackColor;
                            break;
                        default:
                            colorBall = panelColor6.BackColor;
                            break;
                    }
                }

                ballPoints.Add(position);
                ballSize.Add(25);
                ballColor.Add(colorBall);
            }
            catch { }
        }
        private void addWinBall(Point position, string strValue)
        {
            try
            {
                gameBallWinPoints.Add(position);
                gameBallWinText.Add(strValue);
                gameBallWinSize.Add(10);
            }
            catch { }
        }

        enum GameState { Tutorial, Game, End };
        GameState gameState = GameState.Tutorial;

        //List<Point> gameBallPoints = new List<Point>();
        //List<int> gameBallSize = new List<int>();
        //List<Color> gameBallColor = new List<Color>();

        Point[] gameBallPoints = new Point[] { new Point(140, 160), new Point(140, 40), new Point(400, 180), new Point(600, 100) };
        int[] gameBallSize = new int[] { 20, 40, 70, 100 };
        Color[] gameBallColor = new Color[] { Color.Black, Color.DarkSalmon, Color.PaleTurquoise, Color.Khaki };
        string[] gameBallText = new string[] { "x10", "x5", "x2", "x1" };
        int[] gameBallValue = new int[] { 10, 5, 2, 1 };
        int[] gameBallHiddenState = new int[] { 0, 0, 0, 0 };

        List<Point> gameBallWinPoints = new List<Point>();
        List<string> gameBallWinText = new List<string>();
        List<int> gameBallWinSize = new List<int>();


        private void clickTrigger(Point e)
        {
            try
            {
                if (!timerColorAnimate.Enabled)
                    timerColorAnimate.Enabled = true;

                if (labelClickMe.Visible)
                    labelClickMe.Visible = false;

                if (gameState == GameState.Game)
                {
                    gameCheckClicked(e);
                    //addBall(e);
                    //counter++;
                    labelClickScore.Text = counter.ToString();
                } 
                 else if (gameState == GameState.Tutorial)
                {
                    addBall(e);
                    counter++;
                    labelClickScore.Text = counter.ToString();
                } 
            }
            catch { }
        }

        private void gameCheckClicked(Point e)
        {
            try
            {
                for (int i = 0; i < gameBallPoints.Length; i++)
                {

                    if (gameBallHiddenState[i] > 30)
                        continue;

                    if (gameCheckObjectWithinCircle(e, gameBallPoints[i], gameBallSize[i]))
                    {
                        counter += gameBallValue[i];
                        addWinBall(gameBallPoints[i], gameBallText[i]);

                        gameBallHiddenState[i] = 31;

                        Point newPoint = gameGenerateNewPoint(gameBallSize[i]);

                        gameBallPoints[i].X = newPoint.X;
                        gameBallPoints[i].Y = newPoint.Y;

                        return;
                        //Console.WriteLine(i);
                    }
                }
            }
            catch { }
        }
        private Point gameGenerateNewPoint(int size)
        {

            Point newPoint = new Point();

            try
            {
                bool pointExisting = true;
                do
                {
                    newPoint.X = rand.Next(50, 700);
                    newPoint.Y = rand.Next(30, 170);

                    pointExisting = false;

                    for (int i = 0; i < gameBallPoints.Length; i++)
                    {
                        if (gameBallSize[i] == size) continue;

                        if (gameCheckObjectWithinCircle(newPoint, gameBallPoints[i], gameBallSize[i] + size))
                        {
                            pointExisting = true;
                            break;
                        }
                    }
                }
                while (pointExisting);
            }
            catch { }
            return newPoint;
        }

        bool gameCheckObjectWithinCircle(Point e, Point circle, int size)
        {
            double distance = (e.X - circle.X) * (e.X - circle.X) + (e.Y - circle.Y) * (e.Y - circle.Y);
            distance = Math.Sqrt(distance);

            if (distance < size)
            {
                return true;
            }

            return false;
        }

        string gameEndMessage = "";
        private void gameEnd()
        {
            try
            {
                timerGameCounter.Enabled = false;
                timerColorAnimate.Enabled = false;

                gameState = GameState.End;
                buttonGameStart.Text = "Start";

                if (counter > Tools.modeClickS)
                {
                    Tools.modeClickS = counter;
                    gameEndMessage = "New High Score!" + System.Environment.NewLine + counter.ToString();
                }
                else
                {
                    gameEndMessage = "Your score is " + System.Environment.NewLine + counter.ToString();
                    gameEndMessage = gameEndMessage + System.Environment.NewLine + "Current High Score: " + Tools.modeClickS;
                }
                counter = 0;


                labelClickScore.Visible = false;
                labelClickScore.Text = counter.ToString();

                labelGameCountdown.Visible = false;
                timerGameCounter.Enabled = false;


                gameBallWinPoints.Clear();
                gameBallWinText.Clear();
                gameBallWinSize.Clear();

                pictureBoxColorBig.Invalidate();
            }
            catch { }
        }

        private void timerColorAnimate_Tick(object sender, EventArgs e)
        {
            try
            {
                if (gameState == GameState.Game)
                {
                    for (int i = 0; i < gameBallHiddenState.Length; i++)
                    {
                        if (gameBallHiddenState[i] != 30)
                        {
                            gameBallHiddenState[i]++;

                            if (gameBallHiddenState[i] > 50)
                                gameBallHiddenState[i] = 0;
                        }
                    }

                    for (int i = 0; i < gameBallWinPoints.Count; i++)
                    {
                        gameBallWinSize[i] = gameBallWinSize[i] + 1;

                        if (gameBallWinSize[i] > 40)
                            gameBallWinSize[i] = 0;
                        //ballColor[i] = Color.FromArgb((100 - ballSize[i]) * 255 / 100, ballColor[i]);
                    }


                    if (gameBallWinPoints.Count > 0)
                    {
                        for (int i = gameBallWinPoints.Count - 1; i > -1; i--)
                        {
                            if (gameBallWinSize[i] == 0)
                            {
                                gameBallWinPoints.RemoveAt(i);
                                gameBallWinSize.RemoveAt(i);
                                gameBallWinText.RemoveAt(i);
                            }
                        }
                    }
                }
                else if (gameState == GameState.Tutorial)
                {
                    if (ballPoints.Count > 0)
                    {

                        for (int i = 0; i < ballPoints.Count; i++)
                        {
                            ballSize[i] = ballSize[i] + 1;

                            if (ballSize[i] > 100)
                                ballSize[i] = 0;

                            ballColor[i] = Color.FromArgb((100 - ballSize[i]) * 255 / 100, ballColor[i]);
                        }

                        for (int i = ballPoints.Count - 1; i > -1; i--)
                        {
                            if (ballSize[i] == 0)
                            {
                                ballPoints.RemoveAt(i);
                                ballSize.RemoveAt(i);
                                ballColor.RemoveAt(i);
                            }
                        }
                    }
                }
                pictureBoxColorBig.Invalidate();
            }
            catch { }
        }

        private void pictureBoxColorBig_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(pictureBoxColorBig.BackColor);

                if (gameState == GameState.Game)
                {
                    //if (ballPoints.Count == 0) return;
                    for (int i = 0; i < gameBallPoints.Length; i++)
                    {
                        if (gameBallHiddenState[i] > 30) 
                            continue;

                        Rectangle rectBall = new Rectangle(gameBallPoints[i].X - gameBallSize[i] / 2, 
                            gameBallPoints[i].Y - gameBallSize[i] / 2, gameBallSize[i], gameBallSize[i]);
                        Pen pen = new Pen(Color.FromArgb(gameBallColor[i].A, Color.Black), 1);
                        RectangleF rectBallF = rectBall;

                        e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb((gameBallHiddenState[i]) * 255 / 30, gameBallColor[i])), rectBall);
                        e.Graphics.DrawEllipse(pen, rectBall);

                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        e.Graphics.DrawString(gameBallText[i], buttonGameItem1.Font,
                            new SolidBrush(Color.Black), rectBallF, sf);
                   
                    }


                    for (int i = 0; i < gameBallWinPoints.Count; i++)
                    {
                        Rectangle rectBall = new Rectangle(gameBallWinPoints[i].X - gameBallWinSize[i]*2,
                            gameBallWinPoints[i].Y - gameBallWinSize[i] * 2, gameBallWinSize[i] * 4, gameBallWinSize[i] * 4);
                        Pen pen = new Pen( Color.Black);

                        RectangleF rectBallF = rectBall;

                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        Font newFont = new Font(buttonGameItem1.Font.FontFamily, gameBallWinSize[i]);
                        e.Graphics.DrawString(gameBallWinText[i], newFont, 
                            new SolidBrush(Color.FromArgb((30 - (gameBallWinSize[i] -10)) * 255 / 30, Color.Black)), rectBallF, sf);
                   
                        //e.Graphics.FillEllipse(new SolidBrush(ballColor[i]), rectBall);
                        //e.Graphics.DrawEllipse(pen, rectBall);
                    }
                }
                else if (gameState == GameState.End)
                {
                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        RectangleF rectBallF = new RectangleF(0, 0, pictureBoxColorBig.Width, pictureBoxColorBig.Height);
                    e.Graphics.DrawString(gameEndMessage, labelClickMe.Font,
               new SolidBrush(Color.Black), rectBallF, sf);

                }
                else if (gameState == GameState.Tutorial)
                {
                    if (ballPoints.Count == 0) return;

                    for (int i = 0; i < ballPoints.Count; i++)
                    {
                        Rectangle rectBall = new Rectangle(ballPoints[i].X - ballSize[i] / 2, ballPoints[i].Y - ballSize[i] / 2, ballSize[i], ballSize[i]);
                        Pen pen = new Pen(Color.FromArgb(ballColor[i].A, Color.Black), 1);
                        e.Graphics.FillEllipse(new SolidBrush(ballColor[i]), rectBall);
                        e.Graphics.DrawEllipse(pen, rectBall);
                    }
                }
            }
            catch { }
        }

        int gameCounter = 30;
        private void timerGameCounter_Tick(object sender, EventArgs e)
        {
            try
            {
                gameCounter--;

                labelGameCountdown.Text = gameCounter.ToString() + "s";
                if (gameCounter == 0)
                {
                    timerGameCounter.Enabled = false;

                    gameEnd();
                    //end game
                }
            }
            catch { }
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            try
            {
                mainForm.startCalibrateScreen();
                this.Visible = false;

                tabControl1.SelectedIndex = 2;
            }
            catch { }
        }


    }
}
