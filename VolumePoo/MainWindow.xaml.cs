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
using CoreAudioApi;

namespace VolumePoo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MMDevice device;
        private const int MaxNegValue = -10;
        private const int MinNegValue = -5;
        private const int MaxPosValue = 10;
        private const int MinPosValue = 5;
        private const int MinExtreme = 0;
        private const int MaxEtreme = 100;

        public MainWindow()
        {
            InitializeComponent();
            this.Topmost = true;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = SystemParameters.PrimaryScreenWidth - 200;
            this.Top = SystemParameters.PrimaryScreenHeight - 100;            

            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            lblVolume.Content = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            device.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);            
        }

        void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (NotToNearExtreme(MinPosValue, false))
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = ContentSumToFloat(MinPosValue) / 100.0f;
            }
            else
            {
                if (NotToNearExtreme(MinNegValue, true))
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = ContentSumToFloat(MinNegValue) / 100.0f;
            }
        }

        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            if (this.Dispatcher.CheckAccess())
            {
                lblVolume.Content = (int)(data.MasterVolume * 100);                
            }
            else
            {
                object[] Params = new object[1];
                Params[0] = data;
                this.Dispatcher.Invoke(new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification), Params);
            }
        }

        private float ContentSumToFloat(int value)
        {
            var answer = Int32.Parse(lblVolume.Content.ToString()) + value;
            return float.Parse(answer.ToString());
        }

        private bool NotToNearExtreme(int settingValue, bool isMin)
        {
            if(isMin)
                return Int32.Parse(lblVolume.Content.ToString()) - settingValue > MinExtreme;

            return Int32.Parse(lblVolume.Content.ToString()) + settingValue < MaxEtreme;
        }

        private void btnMinusFive_Click(object sender, RoutedEventArgs e)
        {
            if(NotToNearExtreme(MinNegValue, true))
                device.AudioEndpointVolume.MasterVolumeLevelScalar = ContentSumToFloat(MinNegValue) / 100.0f;
        }

        private void btnMinusTen_Click(object sender, RoutedEventArgs e)
        {
            if (NotToNearExtreme(MaxNegValue, true))
                device.AudioEndpointVolume.MasterVolumeLevelScalar = ContentSumToFloat(MaxNegValue) / 100.0f;
        }

        private void btnPlusFive_Click(object sender, RoutedEventArgs e)
        {
            if (NotToNearExtreme(MinPosValue, false))
                device.AudioEndpointVolume.MasterVolumeLevelScalar = ContentSumToFloat(MinPosValue) / 100.0f;
        }

        private void btnPlusTen_Click(object sender, RoutedEventArgs e)
        {
            if (NotToNearExtreme(MaxPosValue, false))
                device.AudioEndpointVolume.MasterVolumeLevelScalar = ContentSumToFloat(MaxPosValue) / 100.0f;
        }

        private void lblVolume_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}



        