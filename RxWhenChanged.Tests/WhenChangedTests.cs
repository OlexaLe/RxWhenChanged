using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace RxWhenChanged.Tests
{
    [TestFixture]
    public class WhenChangedTests
    {
        [SetUp]
        public void SetUp()
        {
            _vm = new ViewModel();
        }

        private ViewModel _vm;

        private class ViewModel : INotifyPropertyChanged
        {
            private string _test;

            public string Test
            {
                get { return _test; }
                set
                {
                    _test = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void PropertyNameShouldNotBeEmpty()
        {
            _vm.WhenChanged<string>("");
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void PropertyNameShouldNotBeNull()
        {
            _vm.WhenChanged<string>(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void PropertyNameShouldNotBeWhitespace()
        {
            _vm.WhenChanged<string>(" ");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void PropertyShouldExistInClass()
        {
            _vm.WhenChanged<string>("Missing");
        }
    }
}