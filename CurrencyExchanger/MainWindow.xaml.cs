using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace CurrencyExchanger
{

    public partial class MainWindow : Window
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public SeriesCollection Collection { get; set; } = new SeriesCollection();
        //paste your own apiKey
        private readonly string apiKey = "";
        public MainWindow()
        {
            InitializeComponent();
            if (apiKey.Length == 0)
            {
                Title.Text = "Paste your apiKey in order for app to work";
            }
        }

        private void ComboFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbi = (ComboBoxItem)ComboFrom.SelectedValue;
            var tt = (TextBlock)cbi.Content;
            FromCurrency = tt.Text;
        }

        private void ComboTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbi = (ComboBoxItem)ComboTo.SelectedValue;
            var tt = (TextBlock)cbi.Content;
            ToCurrency = tt.Text;
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (FromCurrency != null && ToCurrency != null && apiKey.Length>0)
            {
                string endDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
                string startDate = DateTime.UtcNow.AddDays(-4).ToString("yyyy-MM-dd");
                
                string url = String.Concat($"https://free.currconv.com/api/v7/convert?q={FromCurrency}_{ToCurrency}&compact=ultra&date={startDate}&endDate={endDate}&apiKey={apiKey}");
                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpResponseMessage resp = await httpClient.GetAsync(url))
                    {
                        using (HttpContent content = resp.Content)
                        {
                            var res = await content.ReadAsStringAsync();

                            var r = JsonConvert.SerializeObject(JObject.Parse(res)[$"{FromCurrency}_{ToCurrency}"]);
                            Dictionary<string, double> values = JsonConvert.DeserializeObject<Dictionary<string, double>>(r);
                            if (values!=null)
                            {
                                SeriesCollection sc = new SeriesCollection
                            {
                               new LineSeries
                                {
                                     Values = new ChartValues<double>(values.Values)
                                }
                            };
                                Chart.Series = sc;
                            }
                        }
                    }
                }

            }
            else
            {
                MessageBox.Show("An error has occured");
            }
        }

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
