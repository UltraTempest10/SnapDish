using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using SnapDishApp;

public class SearchingPageViewModel : INotifyPropertyChanged
{
    private string dishes;

    public ObservableCollection<Restaurant> Restaurants { get; set; }

    public string Dishes
    {
        get => dishes;
        set
        {
            if (dishes != value)
            {
                dishes = value;
                OnPropertyChanged();
            }
        }
    }

    public SearchingPageViewModel()
    {
        Restaurants = new ObservableCollection<Restaurant>();
        Dishes = string.Empty;
    }

    public async Task GetRecommendedRestaurants(string token, string prompt, double? latitude, double? longitude)
    {
        var restaurants = await GetRecommendedRestaurantsAsync(token, prompt, latitude, longitude);

        if (restaurants != null)
        {
            Restaurants.Clear();
            foreach (var restaurant in restaurants)
            {
                Restaurants.Add(restaurant);
            }
        }
    }

    public async Task GetRecommendedDishes(string token, string prompt)
    {
        var dishes = await GetRecommendedDishesAsync(token, prompt);
        Dishes = dishes;
    }

    private async Task<List<Restaurant>> GetRecommendedRestaurantsAsync(string token, string prompt, double? latitude, double? longitude)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, HttpClientInstance.RecommendationRestaurantUrl);
            HttpClientInstance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["prompt"] = prompt;
            if (latitude.HasValue && longitude.HasValue)
            {
                query["latitude"] = latitude.Value.ToString();
                query["longitude"] = longitude.Value.ToString();
            }

            request.RequestUri = new Uri(request.RequestUri + "?" + query.ToString());

            var response = await HttpClientInstance.HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RecommendationResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result?.Restaurants;
            }
            else
            {
                Console.WriteLine("Failed to get recommended restaurants");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to get recommended restaurants: " + ex.Message);
        }
        return null;
    }

    private async Task<string> GetRecommendedDishesAsync(string token, string prompt)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, HttpClientInstance.RecommendationDishUrl);
            HttpClientInstance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["prompt"] = prompt;

            request.RequestUri = new Uri(request.RequestUri + "?" + query.ToString());

            var response = await HttpClientInstance.HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RecommendationResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result?.Dishes;
            }
            else
            {
                Console.WriteLine("Failed to get recommended dishes");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to get recommended dishes: " + ex.Message);
        }
        return null;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RecommendationResponse
{
    public List<Restaurant> Restaurants { get; set; }
    public string Dishes { get; set; }
}

public class Restaurant
{
    public string Name { get; set; }
    public string Category { get; set; }
    public string TasteScore { get; set; }
    public string EnvironmentScore { get; set; }
    public string ServiceScore { get; set; }
    public string AvgPrice { get; set; }
    public string Address { get; set; }
    public string Image { get; set; }
}
