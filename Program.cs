namespace _5_assignment
{
    public class Program
    {
        public interface Subject
        {
            void subscribe(Observer observer);
            void remove(Observer observer);
            void notify();
        }
        public class WeatherData : Subject
        {
            private float temperature;
            private float humidity; 
            private float pressure;

            private List<Observer> _observers;
            public void subscribe(Observer observer) => _observers.Add(observer);
            public void remove(Observer observer) => _observers.Remove(observer);

            public float getTemperature() => temperature;
            public float getHumidity() => humidity;
            public float getPressure() => pressure;

            public WeatherData()
            {
                _observers = new List<Observer>();
            }

            public void notify()
            {
                foreach (var item in _observers)
                {
                    item.update(this.temperature,this.humidity,this.pressure);
                }
            }
            void measurementsChanged() => notify();
            public void setMeasurements(float temp, float hum, float press)
            {
                this.temperature = temp;
                this.humidity = hum;
                this.pressure = press;
                measurementsChanged();
            }
        }
        // Observer interface
        public interface Observer
        {
            public void update(float temp, float hum, float press);
        }
        // Display interface
        public interface DisplayElement
        {
            public void display();
        }
        // Current condition display shows the currently measured temperature, humidity
        public class CurrentConditionsDisplay : Observer, DisplayElement
        {
            private float temperature;
            private float humidity;
            private WeatherData weatherData;

            public CurrentConditionsDisplay(WeatherData weatherData)
            {
                this.weatherData = weatherData;
                weatherData.subscribe(this);
            }
            public CurrentConditionsDisplay() { }
            public void display() => Console.WriteLine("Current conditions: {1} degrees and {0} % humidity", this.humidity, this.temperature);
            public void update(float temp, float hum, float press)
            {
                this.temperature = temp;
                this.humidity = hum;   
                display();
            }
        }
        // Statistic display shows the average/min/max temperature
        public class StatisticsDisplay : Observer, DisplayElement
        {
            private List<float> temperature = new List<float>(); // to track temperature measurements
            private WeatherData _weatherData;
            private float minTemp;
            private float maxTemp;
            private float avgTemp;

            public StatisticsDisplay(WeatherData weatherData)
            {
                this._weatherData = weatherData;
                _weatherData.subscribe(this); // subscribe to publisher
            }
            public StatisticsDisplay() { }
            public void display() => Console.WriteLine("Avg/Max/Min temperature: {0}/{1}/{2}", this.avgTemp,this.maxTemp,this.minTemp);

            public void update(float temp, float hum, float press)
            {
                this.temperature.Add(temp); // add measurement
                this.minTemp = this.temperature.Min();
                this.maxTemp = this.temperature.Max(); 
                this.avgTemp = this.temperature.Average();
                display();
            }
        }

        public class ForecastDisplay : Observer, DisplayElement
        {
            private float temperature;
            private float humidity;
            private string temperatureForecast;
            private string humidityForecast;
            private WeatherData _weatherData;

            public ForecastDisplay() { }
            public ForecastDisplay(WeatherData weatherData)
            {
                this._weatherData = weatherData;
                this._weatherData.subscribe(this);
            }
            public void display() => Console.WriteLine("Forecast: " + temperatureForecast + humidityForecast);

            public void update(float temp, float hum, float press)
            {
                if (temp > this.temperature)
                    this.temperatureForecast = "Becomes warmer, ";
                else if (temp == this.temperature)
                    this.temperatureForecast = "Temperature the same, ";
                else this.temperatureForecast = "Becomes colder, ";

                if (hum > this.humidity)
                    this.humidityForecast = "more rainy.";
                else if (temp == this.temperature)
                    this.humidityForecast = "same chance of rain.";
                else this.humidityForecast = "less rainy.";

                this.humidity = hum;
                this.temperature = temp;
                display();
            }
        }
        static void Main(string[] args)
        {
            WeatherData weatherData = new WeatherData();
            ForecastDisplay forecastDisplay = new ForecastDisplay();
            CurrentConditionsDisplay currentConditionsDisplay = new CurrentConditionsDisplay();
            StatisticsDisplay statisticsDisplay = new StatisticsDisplay();

            weatherData.subscribe(currentConditionsDisplay);
            weatherData.subscribe(statisticsDisplay);
            //StatisticsDisplay statisticsDisplay = new StatisticsDisplay(weatherData);
            weatherData.subscribe(forecastDisplay);


            weatherData.setMeasurements(80, 65, 30.4f); // then measurementsChanged() and notify() methods are called and 
            weatherData.setMeasurements(82, 70, 29.2f);
            weatherData.setMeasurements(78, 90, 29.2f);
        }
    }
}
