namespace ExchangeData.Helpers
{
    public static class EnumerableExtensions
    {
        private static readonly Random _random = new Random();

        public static T GetRandomElement<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var list = new List<T>(source);

            if (list.Count == 0)
            {
                throw new InvalidOperationException("The source sequence is empty.");
            }

            int randomIndex = _random.Next(list.Count);
            return list[randomIndex];
        }
    }
}
