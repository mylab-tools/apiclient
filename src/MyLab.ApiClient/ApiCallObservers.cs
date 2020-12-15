using System;
using System.Collections.Generic;

namespace MyLab.ApiClient
{
    class ApiCallObservers
    {
        readonly List<IObserver<CallDetails>> _observers = new List<IObserver<CallDetails>>();

        public void Call(CallDetails callDetails)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(callDetails);
            }
        }

        public void Error(Exception error)
        {
            foreach (var observer in _observers)
            {
                observer.OnError(error);
            }
        }

        public void Compete()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }

        public IDisposable Subscribe(IObserver<CallDetails> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            _observers.Add(observer);

            return new Unsubscriber(observer, _observers);
        }

        class Unsubscriber : IDisposable
        {
            private readonly IObserver<CallDetails> _observer;
            private readonly List<IObserver<CallDetails>> _callObservers;

            public Unsubscriber(IObserver<CallDetails> observer, List<IObserver<CallDetails>> callObservers)
            {
                _observer = observer;
                _callObservers = callObservers;
            }
            public void Dispose()
            {
                _callObservers.Remove(_observer);
            }
        }
    }
}