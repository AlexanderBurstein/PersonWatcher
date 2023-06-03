using personwatcherapi.Models;

namespace personwatcherapi.Extensions
{
    public static class UsefulExtensions
    {
        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, Task<TResult>> method,
            int concurrency = int.MaxValue)
        {
            var semaphore = new SemaphoreSlim(concurrency);
            try
            {
                return await Task.WhenAll(source.Select(async s =>
                {
                    try
                    {
                        await semaphore.WaitAsync();
                        return await method(s);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }
            finally
            {
                semaphore.Dispose();
            }
        }

        public static Task<Person> ModifyName(Person person, int[] pros, int[] contras)
        {
            return Task.Factory.StartNew(() =>
            {
                person.Name = $"{person.HowCloseToSunAndMoon(pros, contras)} {person.Name}";
                return person;
            });
        }
    }
}
