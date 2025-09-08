using QuitSmoke.Helpers;
using QuitSmoke.ViewModels;

namespace QuitSmoke.Views;

public partial class HistoryPage : ContentPage
{
    public HistoryPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetService<HistoryPageViewModel>();
    }
}
