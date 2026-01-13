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
        private readonly UICanvasSortingConfig _config;

        private readonly Dictionary<string, Link> _links = new();
        private Link _head;
        private Link _tail;

        public UIForegroundSortingService(int baseOrder, UICanvasSortingConfig config)
        {
            _baseOrder = baseOrder;
            _config = config;
        }

        public Result<int> Get(string id, int orderSizer, UICanvasType canvasType = UICanvasType.None)
        {
            if (string.IsNullOrEmpty(id))
                return Result.Fail<int>("Id is empty");

            if (_links.TryGetValue(id, out Link existingLink))
                return existingLink.Order;

            int priority = _config.GetPriority(canvasType);
            Link newLink = new(0, orderSizer, priority);

            InsertByPriority(newLink);
            _links.Add(id, newLink);
            RecalculateOrders();

            return newLink.Order;
        }

        private void InsertByPriority(Link newLink)
        {
            if (_head == null)
            {
                _head = newLink;
                _tail = newLink;
                return;
            }

            Link current = _tail;
            
            while (current != null && current.Priority > newLink.Priority)
            {
                current = current.Previous;
            }

            if (current == null)
            {
                newLink.Next = _head;
                _head.Previous = newLink;
                _head = newLink;
            }
            else if (current == _tail)
            {
                current.Next = newLink;
                newLink.Previous = current;
                _tail = newLink;
            }
            else
            {
                Link nextLink = current.Next;
                current.Next = newLink;
                newLink.Previous = current;
                newLink.Next = nextLink;
                nextLink.Previous = newLink;
            }
        }

        private void RecalculateOrders()
        {
            int currentOrder = _baseOrder;
            Link current = _head;

            while (current != null)
            {
                current.Order = currentOrder;
                currentOrder += current.OrderSize;
                current = current.Next;
            }

            _onUpdateSorting.OnNext(Unit.Default);
        }

        public void Remove(string id)
        {
            if (!_links.TryGetValue(id, out Link link))
                return;

            if (link.Previous != null)
                link.Previous.Next = link.Next;
            else
                _head = link.Next;

            if (link.Next != null)
                link.Next.Previous = link.Previous;
            else
                _tail = link.Previous;

            _links.Remove(id);
            RecalculateOrders();
        }

        private class Link
        {
            public Link Previous;
            public Link Next;
            public int Order;
            public readonly int OrderSize;
            public readonly int Priority;

            public Link(int order, int orderSize, int priority)
            {
                Order = order;
                OrderSize = orderSize;
                Priority = priority;
            }
        }
    }
}
