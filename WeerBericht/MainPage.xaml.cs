using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Globalization;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace WeerBericht
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        NumberFormatInfo nfi = new NumberFormatInfo();

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            nfi.NumberDecimalSeparator = ".";
        }

        double latitude;
        double longitude;

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;

            Geoposition position = await locator.GetGeopositionAsync();
            latitude = position.Coordinate.Point.Position.Latitude;
            longitude = position.Coordinate.Point.Position.Longitude;

            await MyMap.TrySetViewAsync(position.Coordinate.Point, 18D);

            mySlider.Value = MyMap.ZoomLevel;

            AddMapIcon();
        }       
        private void getPositionButton_Click(object sender, RoutedEventArgs e)
        {
            positionTextBlock.Text = String.Format("{0}, {1}",
                MyMap.Center.Position.Latitude,
                MyMap.Center.Position.Longitude);

        }
        

        private void mySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (MyMap != null)
            {
                MyMap.ZoomLevel = e.NewValue;
            }
        }

        private async void Resp_btn_Click(object sender, RoutedEventArgs e)
        {
                     
            HttpClient client = new HttpClient();
            //var data = await client.GetStringAsync("http://api.openweathermap.org/data/2.5/weather?lat=50.907799&lon=5.4221&APPID=45910e37f9b3c1547078f7a23e0fad4c");
            var data = await client.GetStringAsync("http://api.openweathermap.org/data/2.5/weather?lat="+Convert.ToString(latitude,nfi)+"&lon="+ Convert.ToString(longitude, nfi) +"&APPID=45910e37f9b3c1547078f7a23e0fad4c");
            var weatherList = JsonConvert.DeserializeObject<weather>(data);
            positionTextBlock.Text = "Temperature: " + Convert.ToString(weatherList.main.temp-272.15)+ " ° Celsius";

        }

        private void AddMapIcon()
        {
            MapIcon mapicon1 = new MapIcon();
            mapicon1.Location = new Geopoint(new BasicGeoposition() { Latitude = Convert.ToDouble(latitude, nfi) , Longitude = Convert.ToDouble(longitude) });
            mapicon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapicon1.Title = "your position";
            MyMap.MapElements.Add(mapicon1);
        }
    }
}
