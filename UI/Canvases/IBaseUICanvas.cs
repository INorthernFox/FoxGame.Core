using System;

namespace Core.UI.Canvases
{
    public interface IBaseUICanvas : IDisposable
    {
        string ID { get; }

        UICanvasType CanvasType { get; }

        IObservable<IBaseUICanvas> OnShow { get; }
        IObservable<IBaseUICanvas> OnHide { get; }
        IObservable<IBaseUICanvas> OnDispose { get; }

        void Shove();

        void Hide();
    }
}