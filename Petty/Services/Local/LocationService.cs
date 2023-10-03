namespace Petty.Services.Local
{
    public class LocationService : Service
    {
        public LocationService(LoggerService loggerService)
            : base(loggerService)
        {
        }

        public Task UpdateUserLocation(Models.Location location, string token)
        {
            throw new NotImplementedException();
        }
    }
}
