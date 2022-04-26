using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Data;
using System.IO;
using Xamarin.CommunityToolkit.UI.Views.Options;

namespace Notes
{
    public partial class App : Application
    {
        
        static NotesDB notesDB;

        public static NotesDB NotesDB
        {
            get
             {
                if (notesDB == null)
                {
                    string connString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NotesDataBase.db3");
                    notesDB = new NotesDB(connString);
                }
                return notesDB;
            }
        }

        public App()
        {
            //Device.SetFlags(new string[] { "SwipeView_Experimental", "AppTheme_Experimental" });

            InitializeComponent();

            MainPage = new AppShell();
            //MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        public static ToastOptions GetToastOptions(string messageText)
        {
            var toastOptions = new ToastOptions()
            {
                BackgroundColor = Color.FromHex("#2196F3").MultiplyAlpha(0.5),
                CornerRadius = 25,
                Duration = TimeSpan.FromSeconds(1),
                MessageOptions = new MessageOptions()
                {
                    Message = messageText,
                    Foreground = Color.Blue,
                    Padding = 30
                }

            };
            
            return toastOptions;
        }
    }
}
