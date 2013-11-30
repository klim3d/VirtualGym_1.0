using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityPlayer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Template
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        private WinRTBridge.WinRTBridge _bridge;
        private AppCallbacks appCallbacks;

        public StartPage()
        {
            
            this.InitializeComponent();
            appCallbacks = new AppCallbacks(false);
        }

        

        /// <summary>
        /// Вызывается перед отображением этой страницы во фрейме.
        /// </summary>
        /// <param name="e">Данные о событиях, описывающие, каким образом была достигнута эта страница.  Свойство Parameter
        /// обычно используется для настройки страницы.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        private void Mirror_Mode_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
            var mainPage = new MainPage();
            Window.Current.Content = mainPage;
            Window.Current.Activate();

            // Setup scripting bridge
            _bridge = new WinRTBridge.WinRTBridge();
            appCallbacks.SetBridge(_bridge);
            appCallbacks.SetSwapChainBackgroundPanel(mainPage.GetSwapChainBackgroundPanel());
            appCallbacks.SetCoreWindowEvents(Window.Current.CoreWindow);
            appCallbacks.InitializeD3DXAML();
            //Window.Current.Activate();
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Rehabilitation_Mode_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RehabilitationMode));
        }
    }
}
