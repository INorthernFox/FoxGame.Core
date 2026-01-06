using System;
using UniRx;
using UnityEngine;

namespace Core.Inputs
{
    public interface IPlayerInputService: IDisposable
    {
        IObservable<Vector2> CameraMove { get; }
        IObservable<Vector2> CursorMove { get; }
        IObservable<Vector2> CursorZoom { get; }
        
        IObservable<Unit> MainClick { get; }
        
        void Activate();
        void Deactivate();
    }
}