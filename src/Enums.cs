
namespace Tenki
{
    /// <summary>
    /// Class that contains all project enums
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Numeric codes used by WMO to indicate the weather. Each code corresponds to
        /// a specific weather type
        /// </summary>
        public enum WMOCodes
        {
            ClearSky = 0,
            MainlyClear = 1,
            PartlyCloudy = 2,
            Overcast = 3,
            Fog = 45,
            DepositingRimeFog = 48,
            LightDrizzle = 51,
            MediumDrizzle = 53,
            DenseDrizzle = 55,
            LightFreezingDrizzle = 56,
            DenseFreezingDrizzle = 57,
            SlightRain = 61,
            ModerateRain = 63,
            HeavyRain = 65,
            LightFreezingRain = 66,
            HeavyFreezingRain = 67,
            SlightSnowFall = 71,
            ModerateSnowFall = 73,
            HeavySnowFall = 75,
            SnowGrains = 77,
            SlightRainShower = 80,
            ModerateRainShower = 81,
            ViolentRainShower = 82,
            SlightSnowShower = 85,
            HeavySnowShower = 86,
            Thunderstorm = 95,
            ThunderstormSlightHail = 96,
            ThunderstormHeavyHail = 99
        }
    }   
}