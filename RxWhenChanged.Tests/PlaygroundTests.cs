using System;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using RxWhenChanged.Tests.Playground;

namespace RxWhenChanged.Tests
{
    [TestFixture]
    public class PlaygroundTests
    {
        [SetUp]
        public void SetUp()
        {
            _scheduler = new TestScheduler();
            _vm = new PlaygroundViewModel(_scheduler);
        }

        private PlaygroundViewModel _vm;
        private TestScheduler _scheduler;

        [Test]
        public void InputChangesOutputWithDelay()
        {
            const string testString = "abc";
            _vm.Input = testString;
            Assert.AreEqual(null, _vm.Output);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100).Ticks);
            Assert.AreEqual(testString, _vm.Output);
        }

        [Test]
        public void EchoOutputCommand()
        {
            Assert.IsFalse(_vm.Echo.CanExecute(null));
            _vm.Input = "abc";

            Assert.IsTrue(_vm.Echo.CanExecute(null));
            _vm.Echo.Execute(null);

            Assert.AreEqual("abc", _vm.EchoOutput);
        }
    }
}