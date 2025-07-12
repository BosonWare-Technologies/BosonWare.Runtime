using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace BosonWare.Persistence;

public class PersistentDictionary<TKey, TValue>(string location)
    : PersistentObject<PersistentDictionary<TKey, TValue>>(location), IDictionary<TKey, TValue> where TKey: notnull
{
    private readonly Dictionary<TKey, TValue> _items = [];

    public TValue this[TKey key] { get => ((IDictionary<TKey, TValue>)_items)[key]; set => ((IDictionary<TKey, TValue>)_items)[key] = value; }

    public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_items).Keys;

    public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_items).Values;

    public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)_items).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_items).IsReadOnly;

    [JsonConstructor, Obsolete("This constructor is for JsonSerializer only.", true)]
    public PersistentDictionary() : this(string.Empty) { }

    public void Add(TKey key, TValue value) => ((IDictionary<TKey, TValue>)_items).Add(key, value);

    public void Add(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_items).Add(item);
    
    public void Clear() => ((ICollection<KeyValuePair<TKey, TValue>>)_items).Clear();
    
    public bool Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_items).Contains(item);
    
    public bool ContainsKey(TKey key) => ((IDictionary<TKey, TValue>)_items).ContainsKey(key);
    
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)_items).CopyTo(array, arrayIndex);
    
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)_items).GetEnumerator();
    
    public bool Remove(TKey key) => ((IDictionary<TKey, TValue>)_items).Remove(key);
    
    public bool Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_items).Remove(item);
    
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => ((IDictionary<TKey, TValue>)_items).TryGetValue(key, out value);
    
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_items).GetEnumerator();
}