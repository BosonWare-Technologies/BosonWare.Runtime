# BosonWare Cache<T> Class Documentation

## Overview

The `BosonWare.Cache<T>` class provides a generic, thread-safe, time-based caching mechanism for expensive operations. It uses concurrent data structures to safely cache computed values with automatic expiration, reducing redundant calculations and improving application performance.

## Code Summary

### High-Level Architecture
- **Generic type-safe caching**: Each `Cache<T>` instance maintains a separate cache for values of type `T`
- **Thread-safe operations**: Uses `ConcurrentDictionary` for safe multi-threaded access
- **Time-based expiration**: Automatically expires cached values after a specified duration
- **Lazy evaluation**: Only computes values when needed, with optional short-circuit bypass
- **Dual sync/async support**: Provides both synchronous and asynchronous value retrieval

### Key Design Patterns
- **Generic static class**: Type-specific caching with compile-time type safety
- **Method inlining**: Aggressive inlining for optimal performance in hot paths
- **Immutable value objects**: `CachedValue` struct ensures thread-safe value storage
- **Time-of-day tracking**: Uses `DateTime.Now.TimeOfDay` for efficient time comparisons
- **Defensive programming**: Null argument validation and safe dictionary operations

### Flow Summary
1. Client calls `Get()` or `GetAsync()` with a key, value factory, and expiration duration
2. Cache checks if key exists and value hasn't expired
3. If cache miss or expired: executes factory function and stores result with timestamp
4. If cache hit and not expired: returns cached value directly
5. Background cleanup happens implicitly through overwrites (no explicit cleanup threads)

## Classes

### `Cache<T>` Static Class

A generic static class that provides type-specific caching functionality for values of type `T`.

#### Methods

##### `Clear()`
```csharp
public static void Clear()
```

Removes all cached values for the current type `T`. This operation is thread-safe and atomic.

**Usage Example:**
```csharp
Cache<string>.Clear(); // Clears all cached strings
Cache<int>.Clear();    // Clears all cached integers (separate cache)
```

##### `Remove(object key)`
```csharp
public static void Remove(object key)
```

Removes a specific cached value by key. Safe to call even if the key doesn't exist.

**Parameters:**
- `key`: The key identifying the cached value to remove

**Example:**
```csharp
Cache<UserData>.Remove("user123");
```

##### `Get<T>(object key, Func<T> getter, TimeSpan duration, bool shortCircuit = false)`
```csharp
public static T Get(object key, Func<T> getter, TimeSpan duration, bool shortCircuit = false)
```

Retrieves a cached value or computes it using the provided factory function.

**Parameters:**
- `key`: Unique identifier for the cached value
- `getter`: Factory function to compute the value if not cached or expired
- `duration`: How long the value should remain cached
- `shortCircuit`: If true, bypasses cache and always calls getter

**Returns:** The cached or newly computed value of type `T`

**Exceptions:**
- `ArgumentNullException`: Thrown when `key` or `getter` is null

**Example:**
```csharp
var expensiveData = Cache<string>.Get(
    "api_data", 
    () => CallExpensiveAPI(), 
    TimeSpan.FromMinutes(5)
);
```

##### `GetAsync<T>(object key, Func<Task<T>> getter, TimeSpan duration, bool shortCircuit = false)`
```csharp
public static async Task<T> GetAsync(object key, Func<Task<T>> getter, TimeSpan duration, bool shortCircuit = false)
```

Asynchronously retrieves a cached value or computes it using the provided async factory function.

**Parameters:**
- `key`: Unique identifier for the cached value
- `getter`: Async factory function to compute the value if not cached or expired
- `duration`: How long the value should remain cached
- `shortCircuit`: If true, bypasses cache and always calls getter

**Returns:** A task containing the cached or newly computed value of type `T`

**Exceptions:**
- `ArgumentNullException`: Thrown when `key` or `getter` is null

**Example:**
```csharp
var userData = await Cache<UserProfile>.GetAsync(
    $"user_{userId}",
    async () => await database.GetUserAsync(userId),
    TimeSpan.FromMinutes(10)
);
```

### `CachedValue` Struct

An immutable readonly struct that stores cached values along with their creation timestamp.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `T` | The cached value of the generic type |
| `TimeCreated` | `TimeSpan` | The time-of-day when this value was cached |

## Usage Patterns

### Basic Caching
```csharp
// Cache expensive computations
var result = Cache<ComplexData>.Get(
    "computation_key",
    () => PerformExpensiveCalculation(),
    TimeSpan.FromMinutes(15)
);
```

### API Response Caching
```csharp
// Cache API responses with async
var apiResponse = await Cache<ApiResult>.GetAsync(
    $"api_{endpoint}_{parameters.GetHashCode()}",
    async () => await httpClient.GetFromJsonAsync<ApiResult>(url),
    TimeSpan.FromMinutes(5)
);
```

### Database Query Caching
```csharp
// Cache database queries
var users = await Cache<List<User>>.GetAsync(
    "active_users",
    async () => await dbContext.Users
        .Where(u => u.IsActive)
        .ToListAsync(),
    TimeSpan.FromMinutes(2)
);
```

### Configuration Caching
```csharp
// Cache configuration with longer expiration
var config = Cache<AppConfig>.Get(
    "app_config",
    () => LoadConfigurationFromFile(),
    TimeSpan.FromHours(1)
);
```

### Short-Circuit for Testing
```csharp
// Bypass cache during development/testing
var data = Cache<TestData>.Get(
    "test_key",
    () => GenerateTestData(),
    TimeSpan.FromMinutes(5),
    shortCircuit: isDevelopment // Skip cache in dev mode
);
```

### Cache Management
```csharp
// Remove specific entries
Cache<UserData>.Remove($"user_{userId}");

// Clear entire cache for a type
Cache<UserData>.Clear();

// Different types have separate caches
Cache<string>.Clear();  // Only affects string cache
Cache<int>.Clear();     // Only affects int cache
```

## Performance Characteristics

- **Thread Safety**: Full thread safety via `ConcurrentDictionary`
- **Memory Efficiency**: No background cleanup threads; relies on natural cache turnover
- **Time Complexity**: O(1) average case for cache lookups
- **Inlining**: Aggressive method inlining reduces call overhead
- **Type Isolation**: Each generic type maintains a separate cache instance

## Thread Safety

The `Cache<T>` class is fully thread-safe for all operations:
- Multiple threads can safely read/write concurrently
- Cache updates are atomic operations
- No locks required in client code

## Considerations

- **Memory Growth**: Cache doesn't automatically evict old entries; relies on overwrites
- **Time Precision**: Uses `TimeOfDay` which resets at midnight (consider for long-running apps)
- **Key Equality**: Uses default object equality for keys; ensure proper `GetHashCode()` implementation
- **Exception Handling**: Exceptions in getter functions are not cached and will propagate