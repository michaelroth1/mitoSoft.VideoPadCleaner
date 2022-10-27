namespace mitoSoft.VideopadCleaner
{
    public static class ListExtensions
    {
        public static void RemoveByValue<TKey, TValue>(this SortedDictionary<TKey, TValue> keyValuePairs, TValue value)
        {
            for (int j = keyValuePairs.Count - 1; j >= 0; j--)
            {
                var k = keyValuePairs.ElementAt(j).Key;
                var v = keyValuePairs.ElementAt(j).Value;
                if (v!.Equals(value))
                {
                    keyValuePairs.Remove(k);
                }
            }
        }
    }
}