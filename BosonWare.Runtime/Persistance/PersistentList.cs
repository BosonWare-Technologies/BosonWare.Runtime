using System.Collections;
using System.Text.Json.Serialization;

namespace BosonWare.Persistance;

public class PersistentList<T>(string location) : PersistentObject<PersistentList<T>>(location), IList<T>
{
    private readonly List<T> _items = [];

    public T this[int index] { get => ((IList<T>)_items)[index]; set => ((IList<T>)_items)[index] = value; }

    public int Count => ((ICollection<T>)_items).Count;

    public bool IsReadOnly => ((ICollection<T>)_items).IsReadOnly;

    [JsonConstructor, Obsolete("This constructor is for JsonSerializer only.", true)]
    public PersistentList() : this(string.Empty) { }

    public void Add(T item) => ((ICollection<T>)_items).Add(item);

    public void Clear() => ((ICollection<T>)_items).Clear();

    public bool Contains(T item) => ((ICollection<T>)_items).Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => ((ICollection<T>)_items).CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_items).GetEnumerator();

    public int IndexOf(T item) => ((IList<T>)_items).IndexOf(item);

    public void Insert(int index, T item) => ((IList<T>)_items).Insert(index, item);

    public bool Remove(T item) => ((ICollection<T>)_items).Remove(item);

    public void RemoveAt(int index) => ((IList<T>)_items).RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_items).GetEnumerator();

    public Task AddAsync(T item)
    {
        Add(item);

        return WriteToDiskAsync();
    }

    public Task RemoveAsync(T item)
    {
        Remove(item);

        return WriteToDiskAsync();
    }

    public Task RemoveAtAsync(int index)
    {
        RemoveAt(index);

        return WriteToDiskAsync();
    }

    public Task InsertAsync(int index, T item)
    {
        Insert(index, item);

        return WriteToDiskAsync();
    }
}