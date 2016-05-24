using Newtonsoft.Json;  //nodig om json te Deserialiseren 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;  //nodig om HTTP client te grbruiken
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation; //nodig om geolocation te gebruiken
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
using System.Globalization;  //nodig om te globaliseren
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps; //nodig om de mapcontrols te gebruiken
using WeerBericht.Classes;  //nodig om de klassen in de map klassen te gebruiken


namespace WeerBericht
{
    public sealed partial class MainPage : Page
    {
        NumberFormatInfo nfi = new NumberFormatInfo();  //initialiseren van nfi
        double latitude;    //een double latitude en longitude aanmaken
        double longitude;   //zodat ze overal gekent zijn in het programma

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            nfi.NumberDecimalSeparator = ".";   //wordt gebruikt om de "," te vervangen door een "." 
                                                //want api neemt alleen een punt aan
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) //OnNavigatedTo: zodra het programma opstart
        {
            var locator = new Geolocator();     //maakt een nieuwe geolocator aan
            locator.DesiredAccuracyInMeters = 50;   //nauwkeurigheid van de locator

            Geoposition position = await locator.GetGeopositionAsync(); //haalt asynchroon de positie op.

            latitude = position.Coordinate.Point.Position.Latitude;     //haalt de latitude en longitude van uw
            longitude = position.Coordinate.Point.Position.Longitude;   //positie op en steekt ze in de variabelen.

            await MyMap.TrySetViewAsync(position.Coordinate.Point, 18D);    //zet de view van de map op uw positie

            mySlider.Value = MyMap.ZoomLevel;   //zet de value van de slider op het zoomlevel van de map (18(D)).

            AddMapIcon();   //voert de functie AddMapIcon() uit
        }
               
        private void getPositionButton_Click(object sender, RoutedEventArgs e) //click event
        {
            positionTextBlock.Text = String.Format("{0}, {1}",    //zet de variabelen latitude en longitude
                MyMap.Center.Position.Latitude,                   //om naar een string en zet ze in het textblock
                MyMap.Center.Position.Longitude);
        }

        private void mySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) //ValueChanged event
        {
            if (MyMap != null)     
            {
                MyMap.ZoomLevel = e.NewValue;   //zet het zoomlevel van de map gelijk met de waarde van de slider
            }
        }

        private async void Resp_btn_Click(object sender, RoutedEventArgs e) //Sent request button event (async functie)
        {
            //APPID=45910e37f9b3c1547078f7a23e0fad4c  =  beveiligde sleutel voor de api
            //een voorbeeld api met vaste locatie: 
            // http://api.openweathermap.org/data/2.5/weather?lat=50.907799&lon=5.4221&APPID=45910e37f9b3c1547078f7a23e0fad4c
                   
            HttpClient client = new HttpClient();       //maakt een nieuwe HttpClient aan

            //gaat async verbinding maken met webservice api.aponweather.org en gaat alles in var data steken.
            //lat en lon moeten eerste nog geconverteerd worden naar een string en de "," vervangen worden door een "."
            var data = await client.GetStringAsync("http://api.openweathermap.org/data/2.5/weather?lat="+Convert.ToString(latitude,nfi)+"&lon="+ Convert.ToString(longitude, nfi) +"&APPID=45910e37f9b3c1547078f7a23e0fad4c");

            //gaat de json data van <RootObject>(data) deserialiseren en in weatherlist steken
            var weatherList = JsonConvert.DeserializeObject<RootObject>(data);      

            //de temperatuur(°C), vochtigheid(%), bewolking(%) en windsnelheid/richting in het textblock steken
            positionTextBlock.Text = "Temperature: " + Convert.ToString(weatherList.main.temp - 272.15) + " °Celsius"
                + "\n" + "Humidity: " + Convert.ToString(weatherList.main.humidity) + " %"
                + "\n" + "Clouds: " + Convert.ToString(weatherList.clouds.all) + " %"
                + "\n" + "Wind: speed: " + Convert.ToString(weatherList.wind.speed) + " deg: " + Convert.ToString(weatherList.wind.deg);
        }

        private void AddMapIcon() //functie die een mapicon aanmaakt en tekent op de positie
        {                         
            MapIcon mapIcon1 = new MapIcon(); //maakt een MapIcon aan

            //zet mapIcon1.Location gelijk aan de huidige positie
            //de "," moet hier weer vervangen worden door een "."
            mapIcon1.Location = new Geopoint(new BasicGeoposition() { Latitude = Convert.ToDouble(latitude, nfi) , Longitude = Convert.ToDouble(longitude) });
                        
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);   //stelt de grootte van het punt in
            mapIcon1.Title = "your position";   //stelt de titel van het punt in
            MyMap.MapElements.Add(mapIcon1);    //tekent het punt op de map
        }
    }
}
