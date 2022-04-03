using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Views;

namespace Notes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();

            Detail = new NavigationPage(new NotePage())
            {
                BackgroundColor = Color.White
            };
            IsPresented = false;
        }

    private void BtnAbout_Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new AboutPage())
        {
            BackgroundColor = Color.White
        };
            
            IsPresented = false;
        }

        private void BtnNotes_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new NotePage())
            {
                BackgroundColor = Color.White
            };
            IsPresented = false;
        }
    }
}