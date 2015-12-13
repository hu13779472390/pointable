using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PointableUI
{
    public class PointableObject
    {
        public string description = "";
        public PointableType type = PointableType.Physical;
        public string iconPath = "";
        public bool iconFromResource = false;
        public Vector3 position = new Vector3(1,1,1);
        public string windowTitle = "";
        public string windowClass = "";
        public bool calibrated = false;
        public bool locked = false;
        public bool isEnabled = true;

        public enum ControlLocations { Top, Right, Bottom, Left, None };
        public enum PointableType { Physical, Application};

        public Action [] actions= new Action[4];

        //public PointableObject(string center, string top, string right, string bottom, string left )
        //{
        //    iconCenter = center;
        //    iconTop = top;
        //    iconRight = right;
        //    iconBottom = bottom;
        //    iconLeft = left;
        //}
        public PointableObject()
        {

        }        

        //public PointableObject(PointableType _type, string _description, string _iconPath, Action top, Action right, Action bottom, Action left)
        //{
        //    type = _type;
        //    PointableObject(_description, _iconPath, top, right, bottom, left);
        //}

        public PointableObject(string _description, string _iconPath, Action top, Action right, Action bottom, Action left)
        {
            description = _description;
            iconPath = _iconPath;
            iconFromResource = false;

            if (top == null) top = new Action();
            if (right == null) right = new Action();
            if (bottom == null) bottom = new Action();
            if (left == null) left = new Action();

            actions[(int)ControlLocations.Top] = top;
            actions[(int)ControlLocations.Right] = right;
            actions[(int)ControlLocations.Bottom] = bottom;
            actions[(int)ControlLocations.Left] = left;
        }

        public void setLock()
        {
            locked = true;
        }

        //public string getIconName(ControlLocations controlLocation)
        //{
        //    switch (controlLocation)
        //    {
        //        case ControlLocations.Top:
        //            return iconTop;
        //            break;
        //        case ControlLocations.Right:
        //            return iconRight;
        //            break;
        //        case ControlLocations.Bottom:
        //            return iconBottom;
        //            break;
        //        case ControlLocations.Left:
        //            return iconLeft;
        //            break;
        //    }
        //    return null;
        //}

        public void keysToKeyCode()
        {
            try
            {
                foreach (Action action in this.actions)
                {
                    if (action.actionType == Action.ActionType.Keystroke)
                    {
                        for (int i = 0; i < action.keys.Length; i++)
                        {
                            action.keysKeyCode[i] = action.keys[i].ToString();
                        }
                    }
                }
            }
            catch { }
        }
        public void keycodeToKeys()
        {
            try
            {
                foreach (Action action in this.actions)
                {
                    if (action.actionType == Action.ActionType.Keystroke)
                    {
                        for (int i = 0; i < action.keys.Length; i++)
                        {
                            action.keys[i] = (byte)Convert.ToByte(Convert.ToInt32(action.keysKeyCode[i]));
                        }
                    }
                }
            }
            catch { }
        }
    }

    public class Action
    {
        public enum ActionType {LaunchApplication, Keystroke, None};        
        public string description ="";     
        public string iconPath = "";
        public bool iconFromResource = false;
        public ActionType actionType = ActionType.None;
        public string[] keyDescription = new string[] { "", "", "", "" };
        public byte[] keys = new byte[4];
        public string[] keysKeyCode = new string[] { "", "", "", "" };
        public string applicationPath = "";
        public string applicationArgs = "";
        

        public Action()
        {

        }

        public static Action createAction(Action actionOriginal)
        {
            Action newAction = new Action();
            newAction.actionType = actionOriginal.actionType;
            newAction.applicationArgs = actionOriginal.applicationArgs;
            newAction.applicationPath = actionOriginal.applicationPath;
            newAction.description = actionOriginal.description;
            newAction.iconFromResource = actionOriginal.iconFromResource;
            newAction.iconPath = actionOriginal.iconPath;
            for (int i = 0; i < actionOriginal.keys.Length; i++)
            {
                newAction.keyDescription[i] = actionOriginal.keyDescription[i];
                newAction.keys[i] = actionOriginal.keys[i];
                newAction.keysKeyCode[i] = actionOriginal.keysKeyCode[i];
            }

            return newAction;
        }

        public Action(string _description, string _iconPath, ActionType type)
        {
            actionType = type;
            description = _description;
            iconPath = _iconPath;
            iconFromResource = false;
        }

        public Action(string _description, string _iconPath, string _keyDescription1, string _keyDescription2, string _keyDescription3, string _keyDescription4,
            byte key1, byte key2, byte key3, byte key4 )
        {
            //actionType = ActionType.Keystroke;
            description = _description;
            iconPath = _iconPath;
            iconFromResource = false;
            this.setKeystrokes(_keyDescription1, _keyDescription2, _keyDescription3, _keyDescription4, key1, key2, key3, key4);

        }

        public Action(string _description, string _iconPath, string _applicationPath, string _applicationArgs)
        {
            //actionType = ActionType.LaunchApplication;
            description = _description;
            iconPath = _iconPath;
            iconFromResource = false;
            this.setLaunch(_applicationPath, _applicationArgs);
        }

        public void setKeystrokes(string _keyDescription1, string _keyDescription2, string _keyDescription3, string _keyDescription4, 
            byte key1, byte key2, byte key3, byte key4)
        {
            actionType = ActionType.Keystroke;
            keyDescription[0] = _keyDescription1;
            keyDescription[1] = _keyDescription2;
            keyDescription[2] = _keyDescription3;
            keyDescription[3] = _keyDescription4;

            keys[0] = key1;
            keys[1] = key2;
            keys[2] = key3;
            keys[3] = key4;

            keysKeyCode = new string[] { "", "", "", "" };
        }

        public void setLaunch(string _applicationPath, string _applicationArgs)
        {
            actionType = ActionType.LaunchApplication;
            applicationPath = _applicationPath;
            applicationArgs = _applicationArgs;
        }

        //public void keysToKeyCode()
        //{
        //    for (int i = 0; i < keys.Length; keys++)
        //    {
        //        keysKeyCode[i] = keys[i].ToString();
        //    }

        //}
        //public void keycodeToKeys()
        //{
        //    for (int i = 0; i < keys.Length; keys++)
        //    {
        //        keys[i] = (byte)Convert.ToByte(Convert.ToInt32(keysKeyCode[i]));
        //    }
        //}
    }

}
