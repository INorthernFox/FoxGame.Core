using System;
using System.Collections.Generic;
using FluentResults;
using UniRx;

namespace Core.UI
{
    public class UIForegroundSortingService
    {
        public IObservable<Unit> OnUpdateSorting => _onUpdateSorting;
        private readonly Subject<Unit> _onUpdateSorting = new();

        private readonly int _baseOrder;

        private readonly Dictionary<string, Link> _links = new();
        private Link _lastLink;

        public UIForegroundSortingService(int baseOrder)
        {
            _baseOrder = baseOrder;
        }

        public Result<int> Get(string id, int orderSizer)
        {
            if(string.IsNullOrEmpty(id))
                return Result.Fail<int>($"Id is empty");

            if(_links.TryGetValue(id, out Link link))
            {
                return link.Order;
            }

            int order = _lastLink?.Order == null
                ? _baseOrder
                : _lastLink.Order + orderSizer;

            Link newLink = new(order, orderSizer, _lastLink);
            _links.Add(id, newLink);
            _lastLink?.SetNext(newLink);
            _lastLink = newLink;
            return order;
        }

        public void Remove(string id)
        {
            if(!_links.TryGetValue(id, out Link link))
                return;

            Link current = link.Next;

            while(current != null)
            {
                current.Order -= link.OrderSize;
                current = current.Next;
            }

            Link previous = link.Previous;
            Link next = link.Next;

            if(link == _lastLink)
                _lastLink = previous;

            previous?.SetNext(next);
            next?.SetPrevious(previous);
            _links.Remove(id);
            _onUpdateSorting.OnNext(Unit.Default);
        }

        private class Link
        {
            public Link Previous;
            public Link Next;
            public int Order;
            public readonly int OrderSize;

            public Link(int order, int orderSize, Link previous, Link next = null)
            {
                Previous = previous;
                Next = next;
                Order = order;
                OrderSize = orderSize;
            }

            public void SetNext(Link next) =>
                Next = next;

            public void SetPrevious(Link previous) =>
                Previous = previous;
        }
    }
}