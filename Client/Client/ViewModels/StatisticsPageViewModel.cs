using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using SnapDishApp;

public class StatisticsPageViewModel : INotifyPropertyChanged
{
    private double overallAverageRating;
    private Dictionary<string, double> monthlyAverageRatings;
    private Dictionary<string, int> locationFrequencies;
    private ObservableCollection<RatingItem> ratingDistribution;

    public double OverallAverageRating
    {
        get => overallAverageRating;
        set
        {
            if (overallAverageRating != value)
            {
                overallAverageRating = value;
                OnPropertyChanged();
            }
        }
    }

    public Dictionary<string, double> MonthlyAverageRatings
    {
        get => monthlyAverageRatings;
        set
        {
            if (monthlyAverageRatings != value)
            {
                monthlyAverageRatings = value;
                OnPropertyChanged();
            }
        }
    }

    public Dictionary<string, int> LocationFrequencies
    {
        get => locationFrequencies;
        set
        {
            if (locationFrequencies != value)
            {
                locationFrequencies = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<RatingItem> RatingDistribution
    {
        get => ratingDistribution;
        set
        {
            if (ratingDistribution != value)
            {
                ratingDistribution = value;
                OnPropertyChanged();
            }
        }
    }

    public StatisticsPageViewModel()
    {
        MonthlyAverageRatings = new Dictionary<string, double>();
        LocationFrequencies = new Dictionary<string, int>();
        RatingDistribution = new ObservableCollection<RatingItem>();
    }

    public async Task GetStatisticsAsync(string token, int numMonth)
    {
        await GetOverallAverageRatingAsync(token);
        await GetMonthlyAverageRatingsAsync(token, numMonth);
        await GetLocationFrequenciesAsync(token);
        await GetRatingDistributionAsync(token);
    }

    private async Task GetOverallAverageRatingAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, HttpClientInstance.StatisticsOverallUrl);
            HttpClientInstance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await HttpClientInstance.HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                OverallAverageRating = JsonSerializer.Deserialize<double>(content);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to get overall average rating: " + ex.Message);
        }
    }

    private async Task GetMonthlyAverageRatingsAsync(string token, int numMonth)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, HttpClientInstance.StatisticsMonthlyUrl + "?numMonth=" + numMonth);
            HttpClientInstance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await HttpClientInstance.HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                MonthlyAverageRatings = JsonSerializer.Deserialize<Dictionary<string, double>>(content);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to get monthly average ratings: " + ex.Message);
        }
    }

    private async Task GetLocationFrequenciesAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, HttpClientInstance.StatisticsLocationUrl);
            HttpClientInstance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await HttpClientInstance.HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                LocationFrequencies = JsonSerializer.Deserialize<Dictionary<string, int>>(content);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to get location frequencies: " + ex.Message);
        }
    }

    private async Task GetRatingDistributionAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, HttpClientInstance.StatisticsDistributionUrl);
            HttpClientInstance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await HttpClientInstance.HttpClient.SendAsync(request);
            List<int> ratingCounts = new List<int>();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ratingCounts = JsonSerializer.Deserialize<List<int>>(content);
            }

            for (int i = 0; i < ratingCounts.Count; i++)
            {
                RatingDistribution.Add(new RatingItem
                {
                    Label = $"{i + 1}:",
                    Count = ratingCounts[i]
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to get rating distribution: " + ex.Message);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class RatingItem
    {
        public string Label { get; set; }
        public int Count { get; set; }
    }
}
