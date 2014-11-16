using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Sulakore.Habbo.Components
{
    public class HClient : WebBrowser
    {
        private IntPtr _ieHandle;
        private IntPtr IEHandle
        {
            get
            {
                if (_ieHandle != IntPtr.Zero) return _ieHandle;

                var bcName = new StringBuilder(100);
                IntPtr handle = Handle;
                do NativeMethods.GetClassName((handle = NativeMethods.GetWindow(handle, 5)), bcName, bcName.MaxCapacity);
                while (bcName.ToString() != "Internet Explorer_Server");

                return (_ieHandle = handle);
            }
        }

        public HClient()
        {
            ScriptErrorsSuppressed = true;
            ScrollBarsEnabled = false;
        }

        new public void Enter()
        {
            NativeMethods.PostMessage(IEHandle, 256u, 13, IntPtr.Zero);
        }
        new public void Click(int x, int y)
        {
            NativeMethods.SendMessage(IEHandle, 0x201, IntPtr.Zero, (IntPtr)((y << 16) | x));
            NativeMethods.SendMessage(IEHandle, 0x202, IntPtr.Zero, (IntPtr)((y << 16) | x));
        }
        new public void Click(Point coordinate)
        {
            Click(coordinate.X, coordinate.Y);
        }

        public void Say(string message)
        {
            Speak(message, false);
        }
        public void Shout(string message)
        {
            Speak(message, true);
        }
        public void Speak(string message, bool shout)
        {
            if (string.IsNullOrEmpty(message)) return;

            Enter();
            if (shout) message = ":shout " + message;
            foreach (char c in message) NativeMethods.PostMessage(IEHandle, 0x102, c, IntPtr.Zero);
            Enter();
        }

        public void Sign(HSigns sign)
        {
            Say(":sign " + sign.Juice());
        }
        public void Stance(HStances stance)
        {
            Say(":" + stance.ToString());
        }
        public void Gesture(HGestures gesture)
        {
            switch (gesture)
            {
                case HGestures.Wave: Say("o/"); break;
                case HGestures.Idle: Say(":idle"); break;
                case HGestures.ThumbsUp: Say("_b"); break;
                case HGestures.BlowKiss: Say(":kiss"); break;
                case HGestures.Laugh: Say(":whisper  :D"); break;
            }
        }
    }
}