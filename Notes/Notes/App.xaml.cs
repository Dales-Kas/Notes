using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Data;
using System.IO;

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
    }
}
