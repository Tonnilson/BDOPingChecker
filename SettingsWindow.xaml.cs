using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BDOPingChecker
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.MouseDown += delegate { try { DragMove(); } catch (Exception) { } };

            var workingArea = SystemParameters.WorkArea;
            this.Left = workingArea.Right - this.Width;
            this.Top = workingArea.Bottom - this.Height;

            this.Topmost = true;
            OpacitySlider.Value = Properties.Settings.Default.WindowOpacity;
            OpacityPercent.Content = String.Format("({0}%)", Math.Round(OpacitySlider.Value * 100));

            LockDrag.IsChecked = Properties.Settings.Default.Locked;
        }

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var targetWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
            targetWindow.BackgroundOpacity.Opacity = ((Slider)sender).Value;
            Properties.Settings.Default.WindowOpacity = ((Slider)sender).Value;
            Properties.Settings.Default.Save();

            OpacityPercent.Content = String.Format("({0}%)", Math.Round(((Slider)sender).Value * 100));
        }

        private void CloseSettingWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception)
            {

            }
        }
        private void CloseSettingWindow(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            } catch (Exception)
            {

            }
        }

        private void LockStateChanged(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox)sender;
            Properties.Settings.Default.Locked = (bool)box.IsChecked;
            Properties.Settings.Default.Save();
        }
    }
}
