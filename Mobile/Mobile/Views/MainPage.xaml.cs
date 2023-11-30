namespace Mobile.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            receiptsColumnSeries.PaletteBrushes = new List<Brush>() {
                new SolidColorBrush(Color.FromArgb("#FF0000"))
            };
        }
    }

}
