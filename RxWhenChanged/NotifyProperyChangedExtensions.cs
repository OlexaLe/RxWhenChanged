using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace RxWhenChanged
{
    public static class NotifyProperyChangedExtensions
    {
        public static IObservable<TResult> WhenChanged<TResult>(this INotifyPropertyChanged self, string property)
        {
            ValidateInputParams(self, property);

            var observable = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => handler.Invoke,
                h => self.PropertyChanged += h,
                h => self.PropertyChanged -= h)
                .Where(o => o.EventArgs.PropertyName == property)
                .Select(o => GetPropValue<TResult>(self, property));
            return observable;
        }

        private static void ValidateInputParams(INotifyPropertyChanged self, string property)
        {
            if (string.IsNullOrWhiteSpace(property))
                throw new ArgumentException("Incorrect property name", property);
            if (!self.GetType().GetTypeInfo().DeclaredProperties.Select(p => p.Name).Contains(property))
                throw new ArgumentException($"Property {property} is not found in class", property);
        }

        private static TResult GetPropValue<TResult>(object src, string propName)
        {
            var value = src.GetType().GetTypeInfo().DeclaredProperties.First(p => p.Name == propName).GetValue(src);
            return (TResult) value;
        }
    }
}