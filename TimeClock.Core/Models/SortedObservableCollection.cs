using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace TimeClock.Core.Models;
internal class SortedObservableCollection<T, TProperty> : ObservableCollection<T>
{
    private Func<T, TProperty?>? _sortBy;
    private SortOrderDto _sortOrder;
    private IComparer<TProperty> _comparer;
    private bool IsSorting { get; set; } = false;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SortedObservableCollection()
    {
        this.SetAllDefaults();
    }
    public SortedObservableCollection(IEnumerable<T> values) : base(values)
    {
        this.SetAllDefaults();
    }
    public SortedObservableCollection(IList<T> values):base(values)
    {
        this.SetAllDefaults();
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public SortedObservableCollection(IComparer<TProperty>? comparer, SortOrderDto sortOrder = default)
    {
        this._comparer = comparer ?? Comparer<TProperty>.Default;
        this._sortBy = this._sortBy = (i =>
        {
            if (i is TProperty)
                return (TProperty)Convert.ChangeType(i, typeof(TProperty));
            return default(TProperty);
        });
        this._sortOrder = sortOrder;
    }
    public SortedObservableCollection(Func<T, TProperty> sortBy, SortOrderDto sortOrder = default, IComparer<TProperty>? comparer = null)
    {
        this._comparer = comparer ?? Comparer<TProperty>.Default;
        this._sortBy = sortBy;
        this._sortOrder = sortOrder;
    }
    public SortedObservableCollection(IEnumerable<T> values, Func<T, TProperty> sortBy, SortOrderDto sortOrder = default, IComparer<TProperty>? comparer = null) : base(values)
    {
        this._comparer = comparer ?? Comparer<TProperty>.Default;
        this._sortBy = sortBy;
        this._sortOrder = sortOrder;
    }
    public SortedObservableCollection(IList<T> values, Func<T, TProperty> sortBy, SortOrderDto sortOrder = default, IComparer<TProperty>? comparer = null) : base(values)
    {
        this._comparer = comparer ?? Comparer<TProperty>.Default;
        this._sortBy = sortBy;
        this._sortOrder = sortOrder;
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) 
    {
        if (this.IsSorting) 
            return;
        this.IsSorting = true;

        if (e.Action != NotifyCollectionChangedAction.Reset && e.Action != NotifyCollectionChangedAction.Remove)
        {
            for (int i = 0; i < (e.OldItems ?? new List<T>()).Count; i++)
                this.SortItem((T)e.OldItems![i]!, i+e.OldStartingIndex);
            for (int i = 0; i < (e.NewItems ?? new List<T>()).Count; i++)
                this.SortItem((T)e.NewItems![i]!, i+e.NewStartingIndex);
        }

        this.IsSorting = false;
        base.OnCollectionChanged(e);
    }

    public Func<T, TProperty?>? SortBy
    {
        get => this._sortBy;
        private set
        {
            this._sortBy = value;
            // unmark as private
            // SortAll()
            // call base collection changed            
        }
    }
    public SortOrderDto SortOrder
    {
        get => this._sortOrder;
        private set
        {
            this._sortOrder = value;
            // unmark as private
            // SortAll()
            // call base collection changed
        }
    }

    public IComparer<TProperty> Comparer
    {
        get => this._comparer;
        private set
        {
            this._comparer = value;
            // unmark as private
            // SortAll()
            // call base collection changed
        }
    }

    private void SetAllDefaults()
    {
        this._comparer = (IComparer<TProperty?>)Comparer<T>.Default;
        this._sortBy = (i =>
        {
            if (i is TProperty)
                return (TProperty)Convert.ChangeType(i, typeof(TProperty));
            return default(TProperty);
        });
        this._sortOrder = default;
    }

    private void SortAll()
    {
        // iterate collection calling SortItem(T item)
    }
    private void SortItem(T item, int itemIndex)
    {
        // to improve efficiency for larger collection, do as follows:
        // search from beginning, middle and end, in multiples of 2 within same loop
        // the middle should increment and decrement simultaneously
        // the middle should be the mid point between the actual middle and the beginning and the actual middle and the end
        // this will have good coverage in multiple points to find the desired sort point faster
        int index = this.Count / 2;
        int previousIndex = 0;
        int direction = -1;
        int? previousComparison = null;

        if (this.SortBy is null || this.Count < 2)
            return;

        if (item is null)
        {
            this.Insert(0, item);
            return;
        }

        while (index > -1 && index < this.Count)
        {
            int compareResult = this.Comparer.Compare(this.SortBy(item), this.SortBy(this[index]));
            if (this.SortOrder == SortOrderDto.Descending) compareResult *= -1;

            if (previousComparison is not null)
            {
                if (previousComparison != compareResult)
                {
                    this.MoveInternal(itemIndex, previousComparison <= compareResult ? previousIndex : index);
                    return;
                }
            }

            if (index > -1 && index < this.Count && !item.Equals(this[index]))
                previousComparison = compareResult;

            previousIndex = index;
            index += direction;

            if (index < 0)
                index = this.Count - 1;
        }
    }

    public void MoveInternal(int oldIndex, int newIndex)
    {
        if ((oldIndex == newIndex) || (0 > oldIndex) || (oldIndex >= this.Count) || (0 > newIndex) || (newIndex >= this.Count))
            return;

        int i = 0;
        T tmp = this[oldIndex];

        if (oldIndex < newIndex)
        {
            for (i = oldIndex; i < newIndex; i++)
            {
                this[i] = this[i + 1];
            }
        }
        else
        {
            for (i = oldIndex; i > newIndex; i--)
            {
                this[i] = this[i - 1];
            }
        }

        this[newIndex] = tmp;
    }

    public void MoveInternal(T item, int newIndex) => this.MoveInternal(this.IndexOf(item), newIndex);
}
