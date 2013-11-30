using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace Template
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class RehabilitationMode : Template.Common.LayoutAwarePage
    {
        //MainPage rootPage = MainPage.Current;

        private Inclinometer _inclinometer;
        private uint _desiredReportInterval;

        public RehabilitationMode()
        {
            this.InitializeComponent();
            this.InitializeSound();
            _inclinometer = Inclinometer.GetDefault();
            if (_inclinometer != null)
            {
                // Select a report interval that is both suitable for the purposes of the app and supported by the sensor.
                // This value will be used later to activate the sensor.
                uint minReportInterval = _inclinometer.MinimumReportInterval;
                _desiredReportInterval = minReportInterval > 300 ? minReportInterval : 300;
            }
            else
            {
                //rootPage.NotifyUser("No inclinometer found", NotifyType.StatusMessage);
                System.Diagnostics.Debug.WriteLine("No inclinometer found");
            }
        }
        private MediaElement _dingSound = new MediaElement();

        private void InitializeSound()
        {

            LoadSound("beep-8.wav", _dingSound);

        }

        private async void LoadSound(string SoundFilePath, MediaElement SoundElement)
        {
            // all sounds are stored in the Sounds folder
            try
            {
                Windows.Storage.StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync("Sounds");
                Windows.Storage.StorageFile file = await folder.GetFileAsync(SoundFilePath);
                var stream = await file.OpenReadAsync();
                SoundElement.AutoPlay = false;
                SoundElement.SetSource(stream, file.ContentType);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Problem opening sound file: " + SoundFilePath);
            }
        }

        private void PlaySound(MediaElement SoundElement)
        {
            try
            {
                SoundElement.Play();
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Problem playing sound: " + SoundElement.ToString());
            }
        }

        private void VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            //if (ScenarioDisableButton.IsEnabled)
            {
                if (e.Visible)
                {
                    // Re-enable sensor input (no need to restore the desired reportInterval... it is restored for us upon app resume)
                    _inclinometer.ReadingChanged += new TypedEventHandler<Inclinometer, InclinometerReadingChangedEventArgs>(ReadingChanged);
                }
                else
                {
                    // Disable sensor input (no need to restore the default reportInterval... resources will be released upon app suspension)
                    _inclinometer.ReadingChanged -= new TypedEventHandler<Inclinometer, InclinometerReadingChangedEventArgs>(ReadingChanged);
                }
            }
        }

        async private void ReadingChanged(object sender, InclinometerReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                InclinometerReading reading = e.Reading;
                
                String result = null;
                if (Helpers.SensorCalculation.learningMode)
                {
                    result = Helpers.SensorCalculation.GetPitchDirection(reading.PitchDegrees);
                    // add splash - device learning is done
                }
                else
                {
                    Helpers.SensorCalculation.checkCurrentPitchValue(reading.PitchDegrees);
                }

                //MaxValue.Text = result != null ? result : Helpers.SensorCalculation.maxPitchValue.ToString();

                if (Helpers.SensorCalculation.doBeep)
                {
                    PlaySound(_dingSound);
                    Helpers.SensorCalculation.doBeep = false;
                }
            });
        }

        private void LearningModeEnable(object sender, RoutedEventArgs e)
        {
            if (_inclinometer != null)
            {
                // Establish the report interval
                _inclinometer.ReportInterval = _desiredReportInterval;

                Window.Current.VisibilityChanged += new WindowVisibilityChangedEventHandler(VisibilityChanged);
                _inclinometer.ReadingChanged += new TypedEventHandler<Inclinometer, InclinometerReadingChangedEventArgs>(ReadingChanged);
                Helpers.SensorCalculation.learningMode = true;
            }
            else
            {
                Helpers.SensorCalculation.learningMode = false;
                //rootPage.NotifyUser("No inclinometer found", NotifyType.StatusMessage);
            }

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_inclinometer != null)
            {
                _inclinometer.ReadingChanged -= ReadingChanged;
                _inclinometer.ReportInterval = 0;
            }
            base.OnNavigatingFrom(e);
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации. Также предоставляется любое сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="navigationParameter">Значение параметра, передаваемое
        /// <see cref="Frame.Navigate(Type, Object)"/> при первоначальном запросе этой страницы.
        /// </param>
        /// <param name="pageState">Словарь состояния, сохраненного данной страницей в ходе предыдущего
        /// сеанса. Это значение будет равно NULL при первом посещении страницы.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации. Значения должны соответствовать требованиям сериализации
        /// <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">Пустой словарь, заполняемый сериализуемым состоянием.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
