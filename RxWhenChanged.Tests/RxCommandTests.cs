using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RxWhenChanged.Tests
{
    [TestFixture]
    public class RxCommandTests
    {
        private Task<string> DoNothingAsync(object parameter)
        {
            return Task.FromResult(parameter.ToString());
        }

        private Task<string> ThrowsExceptionAsync(object parameter)
        {
            throw new Exception("expected");
        }

        [Test]
        public void CanExecute_Triggers()
        {
            var canExecute = new Subject<bool>();
            var cmd = new RxCommand<string>(canExecute, DoNothingAsync);

            Assert.IsFalse(cmd.CanExecute(null));

            canExecute.OnNext(true);
            Assert.IsTrue(cmd.CanExecute(null));

            canExecute.OnNext(false);
            Assert.IsFalse(cmd.CanExecute(null));
        }

        [Test]
        [ExpectedException(typeof (ObjectDisposedException))]
        public void CanExecuteThrowsAfterDisposing()
        {
            var cmd = new RxCommand<string>(Observable.Return(true), DoNothingAsync);
            cmd.Dispose();
            cmd.CanExecute(null);
        }

        [Test]
        public void Exception_Handles()
        {
            var exceptionMessage = string.Empty;
            var cmd = new RxCommand<string>(Observable.Return(true), ThrowsExceptionAsync);
            cmd.ThrownExceptions.Subscribe(e => exceptionMessage = e.Message);

            cmd.Execute(null);

            Assert.AreEqual("expected", exceptionMessage);
        }

        [Test]
        public void IsExecuting_Triggers()
        {
            var isExecutingsStates = new List<bool>();
            var canExecute = new Subject<bool>();
            var cmd = new RxCommand<string>(canExecute, DoNothingAsync);

            cmd.IsExecuting.Subscribe(isExecutingsStates.Add);
            cmd.Execute("a");

            CollectionAssert.AreEqual(new[] {true, false}, isExecutingsStates);
        }

        [Test]
        public void ResultsPublish()
        {
            var results = new List<string>();
            var cmd = new RxCommand<string>(Observable.Return(true), DoNothingAsync);
            cmd.Results.Subscribe(results.Add);

            cmd.Execute("a");
            cmd.Execute("b");

            CollectionAssert.AreEqual(new[] {"a", "b"}, results);
        }
    }
}