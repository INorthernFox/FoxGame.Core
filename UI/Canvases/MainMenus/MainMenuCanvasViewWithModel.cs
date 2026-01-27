using System;
using Core.Loggers;
using Core.UI.Buttons;
using FluentResults;
using UniRx;
using UnityEngine;

namespace Core.UI.Canvases.MainMenus
{
    public class MainMenuCanvasView : BaseUICanvasViewWithModel<MainMenuCanvas>
    {
        [SerializeField] private BaseUIButton _fastStartGameButton;

        private readonly CompositeDisposable _disposables = new();

        public override void Initialize(MainMenuCanvas baseUICanvas, UIForegroundSortingService foregroundSorting, IGameLogger logger)
        {
            base.Initialize(baseUICanvas, foregroundSorting, logger);

            _fastStartGameButton.OnClick.Subscribe(FastStartGame).AddTo(_disposables);

            _fastStartGameButton.Initialize();
        }

        private async void FastStartGame(BaseUIButton _)
        {
            _fastStartGameButton.SetInteractable(false);
            Result startGameResult = await Model.FastStartGame();
            _fastStartGameButton.SetInteractable(true);
        }
    }

}