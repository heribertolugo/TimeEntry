using System.Linq.Expressions;
using TimeClock.Data.Models.Jde;

namespace TimeClock.JdeSync.Helpers;
internal interface IJdeEntityId<TIdType>
{
    public Expression<Func<IJdeEntityModel, TIdType?>> IdProperty { get; set; }
    public IJdeEntityModel JdeEntity { get; }
    public TIdType? IdValue { get; }
}
/// <summary>
/// A wrapper for JDE Entities who implement <see cref="IJdeEntityModel"/> used for defining a specific property and retrieving that property's value in a predictable and consistent manner across multiple Entity. 
/// The retrieved property value is typically the property/value which will be used as its ID, accessible through the <see cref="IdValue"/> property. 
/// An instance of the JDE Entity instance is stored in <see cref="JdeEntity"/>. The Expression used to select the property to be used as its ID is stored in <see cref="IdProperty"/>
/// </summary>
/// <typeparam name="TIdType"></typeparam>
internal class JdeEntityId<TIdType> : IJdeEntityId<TIdType>
{
    private Expression<Func<IJdeEntityModel, TIdType?>> _idProperty;
    private Func<IJdeEntityModel, TIdType?> GetIdProperty { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public JdeEntityId(IJdeEntityModel jdeEntity, Expression<Func<IJdeEntityModel, TIdType?>> propertyExpression)
    {
        this.JdeEntity = jdeEntity;
        this.IdProperty = propertyExpression;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public IJdeEntityModel JdeEntity { get; }
    public TIdType? IdValue { get; private set; }

    public Expression<Func<IJdeEntityModel, TIdType?>> IdProperty
    {
        get => this._idProperty;
        set
        {
            this._idProperty = value;
            this.GetIdProperty = this._idProperty.Compile();
            this.IdValue = this.GetIdProperty.Invoke(this.JdeEntity);
        }
    }
}
