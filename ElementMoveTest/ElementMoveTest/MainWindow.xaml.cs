using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ElementMoveTest
{
    public delegate void MoveButtonHandler(Button button, Point position);

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool m_IsPressed = true;

        public MainWindow()
        {
            InitializeComponent();
            this.MouseDown += MainWindow_MouseDown;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.MouseDevice.GetPosition((MainWindow)sender);
            string message = string.Format("x: {0}, y: {1}", point.X, point.Y);
            MessageBox.Show(message);
        }

        private void Samplebutton_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (m_IsPressed)
            {
                TranslateTransform transform = new TranslateTransform();
                transform.X = Mouse.GetPosition(myGrid).X;
                transform.Y = Mouse.GetPosition(myGrid).Y;
                this.Samplebutton.RenderTransform = transform;
            }
        }

        private void myGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.MouseDevice.GetPosition((MainWindow)sender);
            string message = string.Format("x: {0}, y: {1}", point.X, point.Y);
            MessageBox.Show(message);
            Samplebutton.Dispatcher.Invoke(new MoveButtonHandler(moveButton));
        }

        private void moveButton(Button button, Point point)
        {
            button.PointFromScreen(point);
            button.tran
        }

    }
}
