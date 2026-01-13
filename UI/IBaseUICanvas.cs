using System;

namespace Core.UI
{
    public interface IBaseUICanvas : IDisposable
    {
        public string ID { get; }

        IObservable<IBaseUICanvas> OnShow { get; }
        IObservable<IBaseUICanvas> OnHide { get; }
        IObservable<IBaseUICanvas> OnDispose { get; }

        void Shove();

        void Hide();
    }
}