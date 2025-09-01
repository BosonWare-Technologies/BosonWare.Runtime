using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace BosonWare;

/// <summary>
/// Provides a generic, thread-safe, time-based caching mechanism for expensive operations.
/// Each generic type T maintains its own separate cache instance for type safety and isolation.
/// </summary>
/// <typeparam name="T">The type of values to cache</typeparam>
/// <remarks>
/// <para>
/// The Cache&lt;T&gt; class uses ConcurrentDictionary for thread-safe operations and aggressive
/// method inlining for optimal performance. Values are automatically expired based on time duration,
/// and the cache supports both synchronous and asynchronous value factories.
/// </para>
/// <para>
/// Each generic type maintains a completely separate cache instance, ensuring type safety
/// and preventing interference between different cached data types.
/// </para>
/// <para>
/// The cache does not implement background cleanup; expired entries are removed lazily
/// when accessed or overwritten. For long-running applications, consider periodic
/// manual cleanup using <see cref="ClearExpired"/> if memory usage becomes a concern.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Cache expensive API calls
/// var data = await Cache&lt;ApiResponse&gt;.GetAsync(
///     "api_key",
///     async () => await httpClient.GetFromJsonAsync&lt;ApiResponse&gt;(url),
///     TimeSpan.FromMinutes(5)
/// );
/// 
/// // Cache database queries
/// var users = Cache&lt;List&lt;User&gt;&gt;.Get(
///     "active_users",
///     () => database.GetActiveUsers(),
///     TimeSpan.FromMinutes(2)
/// );
/// </code>
/// </example>
[PublicAPI]
public static class Cache<T>
{
    /// <summary>
    /// Thread-safe dictionary storing cached values with their creation timestamps.
    /// Uses ConcurrentDictionary to ensure safe multi-threaded access without explicit locking.
    /// </summary>
    private static readonly ConcurrentDictionary<object, CachedValue> CachedValues = [];

    /// <summary>
    /// Removes all cached values for the current type T.
    /// This operation is atomic and thread-safe.
    /// </summary>
    /// <remarks>
    /// This method clears only the cache for the specific generic type T.
    /// Other Cache&lt;U&gt; instances with different types remain unaffected.
    /// </remarks>
    /// <example>
    /// <code>
    /// Cache&lt;string&gt;.Clear(); // Clears only string cache
    /// Cache&lt;int&gt;.Clear();    // Clears only int cache (separate instance)
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear()
    {
        CachedValues.Clear();
    }

    /// <summary>
    /// Removes all expired cached values for the current type T.
    /// This operation is atomic and thread-safe.
    /// </summary>
    /// <example>
    /// <code>
    /// Cache&lt;string&gt;.ClearExpired(); // Clears only string cache
    /// Cache&lt;int&gt;.ClearExpired();    // Clears only int cache (separate instance)
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ClearExpired()
    {
        var now = DateTime.UtcNow.Ticks;
        
        var cachedValues = CachedValues.ToArray();
        
        foreach (var (key, cachedValue) in cachedValues) {
            if (now - cachedValue.Timestamp > cachedValue.Duration) {
                CachedValues.TryRemove(key, out _);
            }
        }
    }

    /// <summary>
    /// Removes a specific cached value identified by the given key.
    /// Safe to call even if the key doesn't exist in the cache.
    /// </summary>
    /// <param name="key">The key identifying the cached value to remove</param>
    /// <remarks>
    /// This method performs no operation if the specified key is not found in the cache.
    /// The operation is thread-safe and atomic.
    /// </remarks>
    /// <example>
    /// <code>
    /// Cache&lt;UserData&gt;.Remove("user123");
    /// Cache&lt;UserData&gt;.Remove($"user_{userId}");
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove(object key)
    {
        CachedValues.TryRemove(key, out _);
    }

    /// <summary>
    /// Retrieves a cached value or computes it using the provided factory function.
    /// Values are cached for the specified duration and automatically expire.
    /// </summary>
    /// <param name="key">Unique identifier for the cached value. Must not be null.</param>
    /// <param name="getter">Factory function to compute the value if not cached or expired. Must not be null.</param>
    /// <param name="duration">How long the value should remain valid in the cache</param>
    /// <param name="shortCircuit">If true, bypasses the cache entirely and always calls the getter function</param>
    /// <returns>The cached value if valid and not expired, otherwise the result of calling the getter function</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> or <paramref name="getter"/> is null</exception>
    /// <remarks>
    /// <para>
    /// If the key doesn't exist in the cache or the cached value has expired, the getter function
    /// is called to compute a new value, which is then stored in the cache with the current timestamp.
    /// </para>
    /// <para>
    /// The shortCircuit parameter is useful for testing scenarios where you want to bypass
    /// caching behavior or during development when you need fresh data on every call.
    /// </para>
    /// <para>
    /// Time comparison uses DateTime.UtcNow.Ticks for precise, timezone-independent timing.
    /// This eliminates issues with midnight resets and daylight saving time changes,
    /// making the cache suitable for long-running applications with any cache duration.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Cache expensive computation for 15 minutes
    /// var result = Cache&lt;ComplexData&gt;.Get(
    ///     "computation_key",
    ///     () => PerformExpensiveCalculation(),
    ///     TimeSpan.FromMinutes(15)
    /// );
    /// 
    /// // Bypass cache during development
    /// var devData = Cache&lt;TestData&gt;.Get(
    ///     "test_key",
    ///     () => GenerateTestData(),
    ///     TimeSpan.FromMinutes(5),
    ///     shortCircuit: isDevelopmentMode
    /// );
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Get(object key, Func<T> getter, TimeSpan duration, bool shortCircuit = false)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(getter, nameof(getter));

        if (shortCircuit) return getter();

        if (!CachedValues.TryGetValue(key, out var cachedValue)) {
            var instantValue = getter();

            cachedValue = new CachedValue(instantValue!, DateTime.UtcNow.Ticks, duration);

            CachedValues.TryAdd(key, cachedValue);

            return instantValue;
        }

        var utcNow = DateTime.UtcNow;

        if (TimeSpan.FromTicks(utcNow.Ticks - cachedValue.Timestamp) <= duration) 
            return cachedValue.Value;

        var value = getter();

        CachedValues[key] = new CachedValue(value!, DateTime.UtcNow.Ticks, duration);

        return value;
    }

    /// <summary>
    /// Asynchronously retrieves a cached value or computes it using the provided async factory function.
    /// Values are cached for the specified duration and automatically expire.
    /// </summary>
    /// <param name="key">Unique identifier for the cached value. Must not be null.</param>
    /// <param name="getter">Async factory function to compute the value if not cached or expired. Must not be null.</param>
    /// <param name="duration">How long the value should remain valid in the cache</param>
    /// <param name="shortCircuit">If true, bypasses the cache entirely and always calls the getter function</param>
    /// <returns>A task containing the cached value if valid and not expired, otherwise the result of awaiting the getter function</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> or <paramref name="getter"/> is null</exception>
    /// <remarks>
    /// <para>
    /// This method provides the same caching behavior as <see cref="Get"/> but supports
    /// asynchronous value factories. The cache lookup itself is synchronous, but value
    /// computation can be async when needed.
    /// </para>
    /// <para>
    /// If the async getter function throws an exception, the exception is not cached
    /// and will propagate to the caller. Subsequent calls with the same key will retry
    /// the async operation.
    /// </para>
    /// <para>
    /// The method is thread-safe, but be aware that multiple concurrent calls with the same
    /// key might result in multiple executions of the getter function if they occur before
    /// the first result is cached.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Cache API responses for 5 minutes
    /// var apiData = await Cache&lt;ApiResponse&gt;.GetAsync(
    ///     $"api_{endpoint}_{parameters.GetHashCode()}",
    ///     async () => await httpClient.GetFromJsonAsync&lt;ApiResponse&gt;(url),
    ///     TimeSpan.FromMinutes(5)
    /// );
    /// 
    /// // Cache database query results
    /// var users = await Cache&lt;List&lt;User&gt;&gt;.GetAsync(
    ///     "active_users",
    ///     async () => await dbContext.Users.Where(u => u.IsActive).ToListAsync(),
    ///     TimeSpan.FromMinutes(2)
    /// );
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<T> GetAsync(
        object key,
        Func<Task<T>> getter,
        TimeSpan duration,
        bool shortCircuit = false)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(getter, nameof(getter));

        if (shortCircuit) return await getter();

        if (!CachedValues.TryGetValue(key, out var cachedValue)) {
            var instantValue = await getter();

            cachedValue = new CachedValue(instantValue!, DateTime.UtcNow.Ticks, duration);

            CachedValues.TryAdd(key, cachedValue);

            return instantValue;
        }

        var utcNow = DateTime.UtcNow;

        if (TimeSpan.FromTicks(utcNow.Ticks - cachedValue.Timestamp) <= duration) 
            return cachedValue.Value;

        var value = await getter();

        CachedValues[key] = new CachedValue(value!, DateTime.UtcNow.Ticks, duration);

        return value;
    }

    /// <summary>
    /// Immutable structure that stores a cached value along with its creation timestamp.
    /// Used internally by the Cache&lt;T&gt; class to track value expiration.
    /// </summary>
    /// <param name="value">The cached value of type T</param>
    /// <param name="timestamp">The UTC timestamp in ticks when this value was created and cached</param>
    /// <remarks>
    /// This readonly struct ensures thread-safe access to cached values and their timestamps.
    /// The structure is immutable after construction, preventing accidental modification
    /// that could affect cache consistency.
    /// </remarks>
    private readonly struct CachedValue(T value, long timestamp, TimeSpan duration)
    {
        /// <summary>
        /// Gets the cached value of the generic type T.
        /// This value is immutable after the CachedValue instance is created.
        /// </summary>
        /// <value>The original value that was computed and cached</value>
        public T Value { get; } = value;

        /// <summary>
        /// Gets the UTC timestamp in ticks when this value was created and stored in the cache.
        /// Used for expiration calculations by comparing against the current UTC time.
        /// </summary>
        /// <value>A long representing the UTC timestamp in ticks since January 1, 0001</value>
        /// <remarks>
        /// Using UTC ticks provides precise, timezone-independent timing that doesn't suffer
        /// from midnight resets or daylight saving time changes, making it suitable for
        /// long-running applications with cache durations exceeding 24 hours.
        /// </remarks>
        public long Timestamp { get; } = timestamp;

        public long Duration { get; } = duration.Ticks;
    }
}
