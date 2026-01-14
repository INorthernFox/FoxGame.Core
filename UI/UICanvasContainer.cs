namespace Core.UI
{
    public readonly struct UICanvasContainer<TCanvas, TView>
        where TCanvas : BaseUICanvas
        where TView : BaseUICanvasViewWithModel<TCanvas>
    {
        public TCanvas Model { get; }
        public TView View { get; }

        public UICanvasContainer(TCanvas model, TView view)
        {
            Model = model;
            View = view;
        }
    }
}
