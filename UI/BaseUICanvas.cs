using System;
using UniRx;

namespace Core.UI
{
    public abstract class BaseUICanvas : IBaseUICanvas
    {
        public string ID { get; private set; }

        public IObservable<IBaseUICanvas> OnShow => _onShove;
        private readonly Subject<IBaseUICanvas> _onShove = new();

        public IObservable<IBaseUICanvas> OnHide => _onHide;
        private readonly Subject<IBaseUICanvas> _onHide = new();

        public IObservable<IBaseUICanvas> OnDispose => _onDispose;
        private readonly Subject<IBaseUICanvas> _onDispose = new();
            
        protected void SetID(string id) => 
            ID = id;
        
        public void Shove() =>
            _onShove.OnNext(this);

        public void Hide() =>
            _onHide.OnNext(this);

        public void Dispose() =>
            _onDispose?.OnNext(this);
    }


}