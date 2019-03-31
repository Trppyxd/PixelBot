using System;
using System.Windows;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Cursor = System.Windows.Input.Cursor;
using MessageBox = System.Windows.MessageBox;

namespace PixelBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        //private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInf);

        public enum MouseEventFlags
        {
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004
        }

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Click()
        {
            mouse_event((int)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
        }

        private void ClickDragDrop(int x, int y, int x2, int y2)
        {
            SetCursorPos(x, y);
            mouse_event((int)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
            SetCursorPos(x2, y2);
            mouse_event((int)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
        }

        private void DoubleClick(int posX, int posY, int clickDelay = 100)
        {
            SetCursorPos(posX, posY);
            Click();
            Thread.Sleep(clickDelay);
            Click();
        }

        private void ButtonSearchPixel_OnClick(object sender, RoutedEventArgs e)
        {
            string inputHexColorCode = TextBoxHexColor.Text;

            SearchPixel(inputHexColorCode);
        }

        private bool SearchPixel(string hexcode)
        {
            //Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Bitmap bitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);

            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

            Color desiredPixelColor = ColorTranslator.FromHtml(hexcode);

            for (int x = 0; x < SystemInformation.VirtualScreen.Width; x++)
            {
                for (int y = 0; y < SystemInformation.VirtualScreen.Height; y++)
                {
                    Color currentPixelColor = bitmap.GetPixel(x, y);

                    if (desiredPixelColor == currentPixelColor)
                    {
                        MessageBox.Show(String.Format($"Found pixel at {x}, {y} - Now set mouse cursor"));
                        DoubleClick(x, y);
                        return true;
                    }
                }
            }

            MessageBox.Show("Didn't find the pixel.");
            return false;
        }
    }
}
