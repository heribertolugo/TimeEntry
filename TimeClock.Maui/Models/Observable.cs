using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeClock.Maui.Models
{
    public abstract class Observable : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event System.ComponentModel.PropertyChangingEventHandler? PropertyChanging;

        private Dictionary<string, object?> _properties;

        public Observable() 
        { 
            this._properties = new Dictionary<string, object?>();
        }

        public bool SetProperty<T>(T value, [CallerMemberName] string? target = null)
        {
            if (string.IsNullOrWhiteSpace(target))
                throw new ArgumentNullException($"property name is invalid");
            PropertyAction action = this.GetPropertyAction(target, value);

            if (action == PropertyAction.None)
                return false;

            this.OnPropertyChanging(target);
            this.AddOrUpdateProperties(target, value, action);
            this.OnPropertyChanged(target);

            return true;
        }

        public T? GetProperty<T>([CallerMemberName] string? target = null)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(target, nameof(target));
            return (T?)(!this._properties.TryGetValue(target, out object? value) ? default(T?) : value);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private PropertyAction GetPropertyAction(string propertyName, object? value)
        {
            if (!this._properties.ContainsKey(propertyName))
                return PropertyAction.Add;

            if (this._properties[propertyName].Equals(value))
                return PropertyAction.None;

            return PropertyAction.Update;
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.


        /// <summary>
        /// Adds or updates the value in _properties for the selected key based on PropertyAction provided. 
        /// Does not perform integrity check for missing key.
        /// </summary>
        /// <param name="key">key to add or lookup in _properties</param>
        /// <param name="value">value to add or update in _properties</param>
        /// <param name="action">the action to perform</param>
        /// <returns>bool specifying if value was added or updated in _properties</returns>
        private bool AddOrUpdateProperties(string key,  object? value, PropertyAction action)
        {
            switch (action)
            {
                case PropertyAction.Add:
                    this._properties.Add(key, value);
                    break;
                case PropertyAction.Update:
                    this._properties[key] = value;
                    break;
                case PropertyAction.None:
                default:
                    return false;
            }

            return true;
        }

        private enum PropertyAction
        {
            None,
            Add,
            Update
        }
    }
}
