using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Buttons
{
    public class BaseUIButton : MonoBehaviour, IDisposable
    {
        [SerializeField] private Button _button;

        public IObservable<BaseUIButton> OnClick => _onClickSubject;
        private readonly Subject<BaseUIButton> _onClickSubject = new();

        private readonly CompositeDisposable _disposables = new();

        public void Initialize()
        {
            _button.onClick
                .AsObservable()
                .Subscribe(x => _onClickSubject?.OnNext(this))
                .AddTo(_disposables);
        }

        public void Shove() =>
            gameObject.SetActive(true);

        public void Hide() =>
            gameObject.SetActive(false);

        public void SetInteractable(bool value) =>
            _button.interactable = value;

        public void Dispose() =>
            _disposables?.Dispose();
    }
}