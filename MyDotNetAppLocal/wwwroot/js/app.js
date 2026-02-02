class WeatherApp {
    constructor() {
        this.apiKey = 'YOUR_API_KEY'; // Du behöver skaffa en API-nyckel från OpenWeatherMap
        this.baseUrl = '/api/weather';
        this.init();
    }

    init() {
        // Ladda väder för standardstad när sidan laddas
        this.getWeather('Stockholm');
        
        // Lägg till enter-lyssnare för sökfältet
        document.getElementById('locationInput').addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                this.getWeather();
            }
        });
    }

    async getWeather(location = null) {
        const locationInput = document.getElementById('locationInput');
        const searchLocation = location || locationInput.value.trim();
        
        if (!searchLocation) {
            this.showError('Vänligen ange en stad');
            return;
        }

        try {
            this.showLoading();
            
            // Hämta nuvarande väder
            const currentResponse = await fetch(`${this.baseUrl}/current?location=${encodeURIComponent(searchLocation)}`);
            const currentData = await currentResponse.json();
            
            // Hämta prognos
            const forecastResponse = await fetch(`${this.baseUrl}/forecast?location=${encodeURIComponent(searchLocation)}`);
            const forecastData = await forecastResponse.json();
            
            this.displayCurrentWeather(currentData);
            this.displayForecast(forecastData);
            
        } catch (error) {
            console.error('Error fetching weather:', error);
            this.showError('Kunde inte hämta väderdata. Försök igen senare.');
        }
    }

    displayCurrentWeather(data) {
        const currentWeatherDiv = document.getElementById('currentWeather');
        
        if (data.error) {
            this.showError(data.error);
            return;
        }

        const weatherIcons = {
            'Clear': 'fa-sun',
            'Clouds': 'fa-cloud',
            'Rain': 'fa-cloud-rain',
            'Drizzle': 'fa-cloud-rain',
            'Thunderstorm': 'fa-bolt',
            'Snow': 'fa-snowflake',
            'Mist': 'fa-smog',
            'Fog': 'fa-smog',
            'Haze': 'fa-smog'
        };

        const iconClass = weatherIcons[data.main] || 'fa-cloud';
        
        currentWeatherDiv.innerHTML = `
            <div class="weather-main">
                <div class="temperature">${Math.round(data.temperature)}°C</div>
                <div class="weather-icon">
                    <i class="fas ${iconClass}"></i>
                </div>
            </div>
            <div class="description">${data.description}</div>
            <div class="details">
                <div class="detail-item">
                    <div class="detail-label">Känns som</div>
                    <div class="detail-value">${Math.round(data.feelsLike)}°C</div>
                </div>
                <div class="detail-item">
                    <div class="detail-label">Fuktighet</div>
                    <div class="detail-value">${data.humidity}%</div>
                </div>
                <div class="detail-item">
                    <div class="detail-label">Vind</div>
                    <div class="detail-value">${data.windSpeed} m/s</div>
                </div>
                <div class="detail-item">
                    <div class="detail-label">Tryck</div>
                    <div class="detail-value">${data.pressure} hPa</div>
                </div>
            </div>
        `;
    }

    displayForecast(data) {
        const forecastContainer = document.getElementById('forecastContainer');
        
        if (data.error) {
            forecastContainer.innerHTML = `<div class="error">${data.error}</div>`;
            return;
        }

        const weatherIcons = {
            'Clear': 'fa-sun',
            'Clouds': 'fa-cloud',
            'Rain': 'fa-cloud-rain',
            'Drizzle': 'fa-cloud-rain',
            'Thunderstorm': 'fa-bolt',
            'Snow': 'fa-snowflake',
            'Mist': 'fa-smog',
            'Fog': 'fa-smog',
            'Haze': 'fa-smog'
        };

        forecastContainer.innerHTML = data.map(day => {
            const date = new Date(day.date);
            const dateStr = date.toLocaleDateString('sv-SE', { 
                weekday: 'short', 
                month: 'short', 
                day: 'numeric' 
            });
            
            const iconClass = weatherIcons[day.main] || 'fa-cloud';
            
            return `
                <div class="forecast-day">
                    <div class="date">${dateStr}</div>
                    <div class="icon">
                        <i class="fas ${iconClass}"></i>
                    </div>
                    <div class="temp">${Math.round(day.temperature)}°C</div>
                    <div class="temp">Lägsta: ${Math.round(day.minTemperature)}°C</div>
                    <div class="description">${day.description}</div>
                </div>
            `;
        }).join('');
    }

    showLoading() {
        document.getElementById('currentWeather').innerHTML = '<div class="loading">Laddar väderdata...</div>';
        document.getElementById('forecastContainer').innerHTML = '<div class="loading">Laddar prognos...</div>';
    }

    showError(message) {
        document.getElementById('currentWeather').innerHTML = `<div class="error">${message}</div>`;
        document.getElementById('forecastContainer').innerHTML = '';
    }
}

// Global funktion för knappen i HTML
function getWeather() {
    window.weatherApp.getWeather();
}

// Initiera appen när sidan laddas
document.addEventListener('DOMContentLoaded', () => {
    window.weatherApp = new WeatherApp();
});
