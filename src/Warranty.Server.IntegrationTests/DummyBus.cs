using System;
using System.Collections.Generic;
using Common.Extensions;
using NServiceBus;

namespace Warranty.Server.IntegrationTests
{
    public abstract class UseDummyBus
    {
        protected UseDummyBus()
        {
            Bus = new DummyBus();
        }

        public DummyBus Bus { get; }
    }


    public class DummyBus : IBus
    {
        public DummyBus()
        {
            SentLocalMessages = new List<object>();
            SentMessages = new List<object>();
        }

        public List<object> SentLocalMessages { get; }
        public List<object> SentMessages { get; }

        public T CreateInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }

        public T CreateInstance<T>(Action<T> action)
        {
            var obj = CreateInstance<T>();
            action(obj);
            return obj;
        }

        public object CreateInstance(Type messageType)
        {
            return Activator.CreateInstance(messageType);
        }

        public void Publish<T>(params T[] messages)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(T message)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>()
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Type messageType)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Type messageType, Predicate<object> condition)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>(Predicate<T> condition)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(Type messageType)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T>()
        {
            throw new NotImplementedException();
        }

        public ICallback SendLocal(params object[] messages)
        {
            SentLocalMessages.AddRange(messages.AsEnumerable());
            return null;
        }

        public ICallback SendLocal(object message)
        {
            SentLocalMessages.Add(message);
            return null;
        }

        public ICallback SendLocal<T>(Action<T> messageConstructor)
        {
            SentLocalMessages.Add(CreateInstance(messageConstructor));
            return null;
        }

        public ICallback Send(params object[] messages)
        {
            SentMessages.AddRange(messages.AsEnumerable());
            return null;
        }

        public ICallback Send(object message)
        {
            SentMessages.Add(message);
            return null;
        }

        public ICallback Send<T>(Action<T> messageConstructor)
        {
            SentMessages.Add(CreateInstance(messageConstructor));
            return null;
        }

        public ICallback Send(string destination, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(string destination, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(Address address, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(Address address, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(string destination, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(Address address, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(string destination, string correlationId, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(string destination, string correlationId, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(Address address, string correlationId, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(Address address, string correlationId, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(string destination, string correlationId, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(Address address, string correlationId, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback SendToSites(IEnumerable<string> siteKeys, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback SendToSites(IEnumerable<string> siteKeys, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Defer(TimeSpan delay, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Defer(TimeSpan delay, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Defer(DateTime processAt, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Defer(DateTime processAt, object message)
        {
            throw new NotImplementedException();
        }

        public void Reply(params object[] messages)
        {
            throw new NotImplementedException();
        }

        public void Reply(object message)
        {
            throw new NotImplementedException();
        }

        public void Reply<T>(Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public void Return<T>(T errorEnum)
        {
            throw new NotImplementedException();
        }

        public void HandleCurrentMessageLater()
        {
            throw new NotImplementedException();
        }

        public void ForwardCurrentMessageTo(string destination)
        {
            throw new NotImplementedException();
        }

        public void DoNotContinueDispatchingCurrentMessageToHandlers()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> OutgoingHeaders { get; }
        public IMessageContext CurrentMessageContext { get; }
        public IInMemoryOperations InMemory { get; }
    }
}