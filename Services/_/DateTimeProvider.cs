namespace ProvaPub.Services
{
    public static class DateTimeProvider
    {
        private static Func<DateTime> _getUtcNow = () => DateTime.UtcNow;

        public static DateTime UtcNow
        {
            get { return _getUtcNow(); }
        }

        public static void Set(DateTime dateToUse)
        {
            _getUtcNow = () => dateToUse;
        }

        public static void Reset()
        {
            _getUtcNow = () => DateTime.UtcNow;
        }
    }
}
