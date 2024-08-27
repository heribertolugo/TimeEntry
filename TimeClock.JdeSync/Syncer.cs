using System.Linq.Expressions;
using TimeClock.Data.Models;
using TimeClock.Data.Models.Jde;
using TimeClock.JdeSync.Helpers;

namespace TimeClock.JdeSync;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TIdType">The type for the property which will be used as an ID for the <see cref="IJdeEntityModel"/></typeparam>
internal class Syncer<TIdType>
{
    /// <summary>
    /// Occurs after initialization, but before getting the first item in jdeItems and therefore before any comparison takes place. 
    /// Making the JdeEntity null, and the TimeClockEntity the first item in tcItems.
    /// </summary>
    public event EventHandler<SyncerEventArgs>? OnStart;
    /// <summary>
    /// Occurs after getting the next item in jdeItems, but before any comparison is made.
    /// </summary>
    public event EventHandler<SyncerEventArgs>? OnBeforeCompare;
    /// <summary>
    /// Occurs when no more items in tcItems can be iterated, or whenever TimeClockEntity ID is greater than JdeEntity ID. 
    /// This is called each time the aforementioned are true while there are still items in jdeItems being enumerated.
    /// </summary>
    public event EventHandler<SyncerEventArgs>? OnNeedsNew;
    /// <summary>
    /// Occurs when TimeClockEntity ID is equal to JdeEntity ID. 
    /// This is called each time the aforementioned is true while there are still items in jdeItems being enumerated.
    /// </summary>
    public event EventHandler<SyncerEventArgs>? OnNeedsUpdate;
    /// <summary>
    /// Occurs when TimeClockEntity ID is less than JdeEntity ID, only once until TimeClockEntity ID is NOT less than JdeEntity ID. 
    /// In other words, occurs once per batch where TimeClockEntity ID is less than JdeEntity ID, before any items should be deleted. 
    /// If you need to handle each item in batch where TimeClockEntity ID is less than JdeEntity ID, subscribe to <see cref="OnDelete"/> instead. 
    /// This is called each time the aforementioned is true while there are still items in jdeItems being enumerated.
    /// </summary>
    public event EventHandler<SyncerEventArgs>? OnNeedsDelete;
    /// <summary>
    /// Occurs when TimeClockEntity ID is less than JdeEntity ID, for each item until TimeClockEntity ID is NOT less than JdeEntity ID. 
    /// In other words, occurs when an item should be deleted. 
    /// This is called each time the aforementioned is true while there are still items in jdeItems being enumerated.
    /// </summary>
    public event EventHandler<SyncerEventArgs>? OnDelete;
    /// <summary>
    /// Occurs after all items that should be deleted have been iterated.
    /// This is called each time the aforementioned is true while there are still items in jdeItems being enumerated.
    /// </summary>
    public event EventHandler<SyncerEventArgs>? OnAfterDelete;
    /// <summary>
    /// Occurs after all JdeEntity items in jdeItems have been iterated, before exiting the <see cref="SyncJdeToTc(BiDirectionalEnumerable{IJdeEntityModel}, Expression{Func{IJdeEntityModel, TIdType?}}, IEnumerable{IReferenceJde})"/> method
    /// </summary>
    public event EventHandler<SyncerEventArgs>? OnFinished;

    /// <summary>
    /// This is the engine which is used to iterate 2 collections and decide if the receiving collection (tcItems) 
    /// needs items added from the other collection (jdeItems), items removed or just update values. 
    /// Events are fired for each of the aforementioned conditions, and additional events are fired in or in-between the 
    /// steps in the aforementioned conditions. The following is the lifecycle order of the fired events to handle synchronization 
    /// between the 2 collections.
    /// 
    /// <para>
    /// <list type="number">
    /// <item><term><see cref="OnStart"/></term><description><inheritdoc cref="OnStart" path='/summary'/></description></item>
    /// <item><term><see cref="OnBeforeCompare"/></term><description><inheritdoc cref="OnBeforeCompare" path='/summary'/></description></item>
    /// <item><term><see cref="OnNeedsNew"/></term><description><inheritdoc cref="OnNeedsNew" path='/summary'/></description></item>
    /// <item><term><see cref="OnNeedsUpdate"/></term><description><inheritdoc cref="OnNeedsUpdate" path='/summary'/></description></item>
    /// <item><term><see cref="OnNeedsDelete"/></term><description><inheritdoc cref="OnNeedsDelete" path='/summary'/></description></item>
    /// <item><term><see cref="OnDelete"/></term><description><inheritdoc cref="OnDelete" path='/summary'/></description></item>
    /// <item><term><see cref="OnAfterDelete"/></term><description><inheritdoc cref="OnAfterDelete" path='/summary'/></description></item>
    /// <item><term><see cref="OnFinished"/></term><description><inheritdoc cref="OnFinished" path='/summary'/></description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="jdeItems">A <see cref="IEnumerable{T}"/> of <see cref="IJdeEntityModel"/> which is used as the control collection</param>
    /// <param name="idPropertyExpression">An expression which specifies which property for the items in jdeItems will be used as its ID for synchronization</param>
    /// <param name="tcItems">A <see cref="IEnumerable{T}"/> of <see cref="IReferenceJde"/> which represents the data which needs to be synchronized</param>
    /// <remarks>The reason the types are restricted to <see cref="IJdeEntityModel"/> and <see cref="IReferenceJde"/> is for the <see cref="SyncerEventArgs"/>. 
    /// To allow easy access to the types that will be consumed.</remarks>
    public void SyncJdeToTc(
        IEnumerable<IJdeEntityModel> jdeItems, Expression<Func<IJdeEntityModel, TIdType?>> idPropertyExpression,
        IEnumerable<IReferenceJde> tcItems)
    {
        var jdeBiDirectional = jdeItems.ToBiDirectionalEnumerable();
        var jdeIterator = (BiDirectionalEnumerator<IJdeEntityModel>)jdeBiDirectional.GetEnumerator();
        var tcIterator = tcItems.GetEnumerator();
        bool canIterateTc = tcIterator.MoveNext();

        OnStart?.Invoke(this, new SyncerEventArgs(jdeIterator.GetCurrent(), tcIterator.GetCurrent()));

        while (jdeIterator.MoveNext())
        {
            // this jdeWrapper gets the value from the JDE Entity, since different tables use a different type and column name (property) for its ID
            // the property to use as its ID is specified in idPropertyExpression. 
            // If we had at least a consistent type for ID (sometimes its string, sometimes its int), then we could've just used an interface, instead of this wrapper.
            var jdeWrapper = new JdeEntityId<TIdType>(jdeIterator.Current, idPropertyExpression);
            object? jdeId = jdeWrapper.IdValue;

            OnBeforeCompare?.Invoke(this, new SyncerEventArgs(jdeIterator.GetCurrent(), tcIterator.GetCurrent()));
            // make sure we do not attempt to process items which are meant to only exist in the TimeClock database
            this.SkipNonJdeId(tcIterator, ref canIterateTc);
            // when we cannot iterate TimeClock items, it is same as greater than because we need to create new TimeClock item
            CompareResult comparison = !canIterateTc ? CompareResult.IsGreaterThan : this.CompareTypeOrdinal(tcIterator.Current.JdeId, jdeId);

            switch (comparison)
            {
                // if we have no more items in TimeClock DB, then this JDE item must be a new record to insert into TimeClock DB
                // OR, if our current ID in this TimeClock item is greater than our JDE item ID, then we are missing a JDE item - so insert new record into TimeClock DB
                case CompareResult.IsGreaterThan:
                    OnNeedsNew?.Invoke(this, new SyncerEventArgs(jdeIterator.GetCurrent(), tcIterator.GetCurrent()));
                    // we either cannot traverse TimeClock items in this table/collection any more, or need to catch up to next TimeClock item.
                    // so don't try to iterate TimeClock db anymore and just continue
                    continue;
                // if our IDs match, update. no need to check if different, ef core will check and only create update query if needed
                case CompareResult.IsEqualTo:
                    OnNeedsUpdate?.Invoke(this, new SyncerEventArgs(jdeIterator.GetCurrent(), tcIterator.GetCurrent()));
                    canIterateTc = tcIterator.MoveNext();
                    continue;
                // we are out of sync, JDE is ahead. This means we need to sync, and mark all the TimeClocks items as delete until we resync - since they no longer exist in JDE
                case CompareResult.IsLessThan:
                    OnNeedsDelete?.Invoke(this, new SyncerEventArgs(jdeIterator.GetCurrent(), tcIterator.GetCurrent()));
                    while (canIterateTc && this.CompareTypeOrdinal(tcIterator.Current.JdeId, jdeId) == CompareResult.IsLessThan)
                    {
                        OnDelete?.Invoke(this, new SyncerEventArgs(jdeIterator.GetCurrent(), tcIterator.GetCurrent()));
                        canIterateTc = tcIterator.MoveNext();
                    }
                    OnAfterDelete?.Invoke(this, new SyncerEventArgs(jdeIterator.GetCurrent(), tcIterator.GetCurrent()));

                    //our TimeClock items are caught up, we move JDE back since it will move forward again (basically we are pausing on this item to process it with the next TimeClock item)
                    jdeIterator.MoveBack();
                    break;
                default:
                    throw new InvalidOperationException($"{nameof(comparison)} value was not a valid enum value for {nameof(CompareResult)}");
            }
        }

        OnFinished?.Invoke(this, new SyncerEventArgs(jdeIterator.GetCurrent(), tcIterator.GetCurrent()));
    }

    private void SkipNonJdeId(IEnumerator<IReferenceJde> tcIterator, ref bool canIterateTc)
    {
        int id;

        if (!canIterateTc)
            return;

        if (typeof(TIdType) == typeof(string))
        {
            if (tcIterator.GetCurrent()?.JdeId?.ToString()?.Trim() == CommonValues.NonJdeId)
                canIterateTc = tcIterator.MoveNext();
            #region Big Comment
            //** NOTE: uncomment below if the need arises to have more NonJdeId, in other words, more entities and they are using ID values less than 0 other than the specified in NonJdeId (like -2)
            //** can possibly just remove if condition above these comments and only use condition below these comments
            //** just using the string ordinal comparison to mimic the behavior of int comparison will not work, 
            //** because an ID of empty string or underscore will get sorted to a position before our NonJdeId negative values
            //** so while we have only one NonJdeId (-1) we can check for that value and skip it, instead of using expensive parse
            //** in the event of more NonJdeId values, our best option would likely be to parse, vs check if the value is contained in a list
            //** the readonly values of NonJdeId really just signify the first value for id's which do not sync with JDE
            //else if (canIterateTc && (int.TryParse(tcIterator.Current.JdeId?.ToString(), out id) && id < 0))
            //{
            //    while (canIterateTc && id <= CommonValues.NonJdeIdInt)
            //    {
            //        canIterateTc = tcIterator.MoveNext();
            //        if (canIterateTc)
            //            int.TryParse(tcIterator.Current.JdeId?.ToString(), out id);
            //    }
            //}
            #endregion Big Comment
        }
        else if (typeof(TIdType) == typeof(int))
        {
            if (!int.TryParse(tcIterator.GetCurrent()?.JdeId?.ToString(), out id))
                throw new Exception("idType was int but tcIterator.Current.JdeId could not be parsed to int");
            while (canIterateTc && id <= CommonValues.NonJdeIdInt)
            {
                canIterateTc = tcIterator.MoveNext();
                if (!canIterateTc) break;
                if (!int.TryParse(tcIterator.Current.JdeId?.ToString(), out id))
                    throw new Exception("idType was int but tcIterator.Current.JdeId could not be parsed to int");
            }
        }
    }
    /// <summary>
    /// Checks for <see langword="null"/> (giving <see langword="null"/> the lowest ordinal value), and then delegates comparison to a method for the specific type being compared.
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns>A <see cref="CompareResult"/> specifying the ordinal of <paramref name="value1"/> to <paramref name="value2"/> (e.g: if <paramref name="value1"/> is less than <paramref name="value2"/>)</returns>
    /// <exception cref="NotImplementedException">Thrown whenever <see cref="TIdType"/> is not <see cref="int"/> or <see cref="string"/></exception>
    private CompareResult CompareTypeOrdinal(object? value1, object? value2)
    {
        if (value1 is null && value2 is null)
            return CompareResult.IsEqualTo;
        if (value1 is null)
            return CompareResult.IsLessThan;
        if (value2 is null)
            return CompareResult.IsGreaterThan;

        if (typeof(TIdType) == typeof(int) || typeof(TIdType) == typeof(int?))
            return this.CompareInteger((int)value1, (int)value2);
        if (typeof(TIdType) == typeof(string)) // strings of different length should not be equal, but JDE doesn't honor whitespace
            return this.CompareString(((string)value1).Trim(), ((string)value2).Trim());

        throw new NotImplementedException($"{nameof(CompareTypeOrdinal)} has not been implemented for type {typeof(TIdType).GetType().Name}");
    }

    private CompareResult CompareInteger(int value1, int value2)
    {
        if (value1 < value2) 
            return CompareResult.IsLessThan;
        if (value1 == value2)
            return CompareResult.IsEqualTo;

        return CompareResult.IsGreaterThan;
    }

    private CompareResult CompareDecimal()
    {
        throw new NotImplementedException($"{nameof(CompareDecimal)} has not been implemented");
    }

    private CompareResult CompareString(string value1, string value2)
    {
        return value1.IsOrdinalEqual(value2) ? CompareResult.IsEqualTo 
            : (value1.IsOrdinalLess(value2) ? CompareResult.IsLessThan : CompareResult.IsGreaterThan);
    }

    private enum CompareResult
    {
        IsLessThan,
        IsEqualTo,
        IsGreaterThan
    }
}

public class SyncerEventArgs: EventArgs
{
    public SyncerEventArgs(IJdeEntityModel? jdeEntity, IReferenceJde? timeclockEntity)
    {
        this.JdeEntity = jdeEntity;
        this.TimeClockEntity = timeclockEntity;
    }
    public IReferenceJde? TimeClockEntity { get; set; }
    public IJdeEntityModel? JdeEntity { get; set; }
}