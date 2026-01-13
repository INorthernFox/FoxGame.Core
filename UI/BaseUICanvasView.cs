using System.Linq;
using Core.Loggers;
using FluentResults;
using UniRx;
using UnityEngine;

namespace Core.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class BaseUICanvasView<T> : MonoBehaviour
        where T : IBaseUICanvas
    {
        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private int _orderSizer = 100;

        private UIForegroundSortingService _foregroundSorting;
        private IGameLogger _logger;
        private readonly CompositeDisposable _disposables = new();
        
        protected T Model {get; private set;}

        public string ID =>
            Model.ID;

        public virtual void Initialize(T baseUICanvas, UIForegroundSortingService foregroundSorting, IGameLogger logger)
        {
            Model = baseUICanvas;
            _foregroundSorting = foregroundSorting;
            _logger = logger;

            _foregroundSorting.OnUpdateSorting.Where(x => gameObject.activeSelf).Subscribe(x => UpdateOrder()).AddTo(_disposables);
            Model.OnShow.Subscribe(x => Shove()).AddTo(_disposables);
            Model.OnHide.Subscribe(x => Hide()).AddTo(_disposables);
            Model.OnDispose.Subscribe(x => Dispose()).AddTo(_disposables);
        }

        private void UpdateOrder()
        {
            Result<int> orderResult = _foregroundSorting.Get(ID, _orderSizer);

            if(orderResult.IsFailed)
            {
                string errors = string.Join("; ", orderResult.Errors.Select(e => e.Message));
                _logger.LogError(IGameLogger.LogSystems.UIWindow, $"Can't update order {errors}", "BaseUIWindowView.UpdateOrder");
                return;
            }

            _canvas.sortingOrder = orderResult.Value;
        }

        private void Shove()
        {
            gameObject.SetActive(true);
            UpdateOrder();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            _foregroundSorting.Remove(ID);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void Dispose()
        {
            _disposables.Clear();
        }
    }
}