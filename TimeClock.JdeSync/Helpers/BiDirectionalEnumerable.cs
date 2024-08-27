using System.Collections;

namespace TimeClock.JdeSync.Helpers;

/// <summary>
/// <inheritdoc cref="IEnumerable{T}"/> 
/// The enumerator provided allows reverse traversal of a collection.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBiDirectionalEnumerable<T> : IEnumerable<T>
{
    public new IEnumerator<T> GetEnumerator()
    {
        return new BiDirectionalEnumerator<T>(this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

/// <summary>
/// <inheritdoc cref="List{T}"/> 
/// The enumerator provided allows reverse traversal of the list.
/// </summary>
/// <typeparam name="T"></typeparam>
public class BiDirectionalList<T> : List<T>, IBiDirectionalEnumerable<T>
{
    //private readonly List<T> _values;
    //public BiDirectionalList() { }
    ////{ this._values = new List<T>(); }
    //public BiDirectionalList(IEnumerable<T> values) : base(values) { }

    //{
    //    this._values.AddRange(values);
    //}
    //public T this[int index] 
    //{
    //    get { return this._values[index]; }
    //    set { this._values[index] = value; }
    //}

    //public int Count => this._values.Count;
    //public bool IsReadOnly => false;

    //public void Add(T item) => this._values.Add(item);
    //public void Clear() => this._values.Clear();
    //public bool Contains(T item) => this._values.Contains(item);
    //public void CopyTo(T[] array, int arrayIndex) => this._values.CopyTo(array, arrayIndex);
    public new IEnumerator<T> GetEnumerator() => new BiDirectionalEnumerator<T>(this);
    //public int IndexOf(T item) => this._values.IndexOf(item);
    //public void Insert(int index, T item) => this.Insert(index, item);
    //public bool Remove(T item) => this._values.Remove(item);
    //public void RemoveAt(int index) => this._values.RemoveAt(index);
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

/// <summary>
/// Basic implementation of <see cref="IBiDirectionalEnumerable{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class BiDirectionalEnumerable<T> : IBiDirectionalEnumerable<T>
{
    private IEnumerable<T> Values { get; set; }
    public BiDirectionalEnumerable(IEnumerable<T> values)
    {
        this.Values = values;
    }
    public IEnumerator<T> GetEnumerator()
    {
        return new BiDirectionalEnumerator<T>(this.Values);
    }
}

/// <summary>
/// <inheritdoc cref="IEnumerator{T}"/>
/// Providing the ability to iterate in reverse in addition to forward.
/// </summary>
/// <typeparam name="T"></typeparam>
public class BiDirectionalEnumerator<T> : IEnumerator<T>
{
    private IList<T> Values { get; init; }
    private IEnumerable<T> Source { get; set; }
    private IEnumerator<T> Enumerator { get; set; }
    private int CurrentPosition { get; set; }
    private int LastNextPosition { get; set; }
    private bool NeedPause { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public BiDirectionalEnumerator(IEnumerable<T> values)
    {
        this.Source = values;
        this.Values = new List<T>();
        this.CurrentPosition = -1;
        this.LastNextPosition = -1;
        this.Enumerator = values.GetEnumerator();
#pragma warning disable CS8601 // Possible null reference assignment.
        this.Current = default;
#pragma warning restore CS8601 // Possible null reference assignment.
        this.NeedPause = false;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public T Current { get; private set; }

    object? IEnumerator.Current => this.Current;

    public void Dispose()
    {
        this.Enumerator.Dispose();
        this.Values.Clear();
        this.Source = Enumerable.Empty<T>();
        this.CurrentPosition = 0;
        this.LastNextPosition = 0;
    }
    public bool MoveNext()
    {
        bool canMove = false;
        this.CurrentPosition++;

        if (this.CurrentPosition < this.LastNextPosition)
        {
            this.Current = this.Values[this.CurrentPosition];
            return true;
        }

        if (this.CurrentPosition == this.LastNextPosition && this.NeedPause)
        {
            this.NeedPause = false;
            this.Current = this.Values[this.CurrentPosition];
            return true;
        }

        canMove = this.Enumerator.MoveNext();

        if (canMove)
        {
            this.LastNextPosition++;
            this.Current = this.Enumerator.Current;
            this.Values.Add(this.Current);
        }

        return canMove;
    }
    public bool MoveBack()
    {
        this.CurrentPosition--;
        this.NeedPause = true;

        if (this.CurrentPosition < 0)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            this.Current = default;
#pragma warning restore CS8601 // Possible null reference assignment.
            return false;
        }

        if (this.LastNextPosition < this.CurrentPosition)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            this.Current = default;
#pragma warning restore CS8601 // Possible null reference assignment.
            return true;
        }

        this.Current = this.Values[this.CurrentPosition];

        return true;
    }
    public void Reset()
    {
        this.CurrentPosition = -1;
        this.LastNextPosition = -1;
        this.Enumerator.Reset();
    }
}