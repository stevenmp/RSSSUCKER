using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace RSSSUCKER
{
    public partial class VoirBillet : PhoneApplicationPage
    {
        private Billet billet;

        public VoirBillet()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            billet = (Billet)PhoneApplicationService.Current.State["BilletCourrant"];
            billet.EstDejaLu = true;

            List<string> dejaLus;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("ListeDejaLus"))
                dejaLus = (List<string>)IsolatedStorageSettings.ApplicationSettings["ListeDejaLus"];
            else
                dejaLus = new List<string>();
            if (!dejaLus.Any(elt => elt == billet.Id))
                dejaLus.Add(billet.Id);

            IsolatedStorageSettings.ApplicationSettings["ListeDejaLus"] = dejaLus;
            PageBillet.Navigate(billet.Url);
            base.OnNavigatedTo(e);
        }

        private void PageBillet_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            MessageBox.Show("Impossible de charger la page, vérifiez votre connexion internet");
        }
    }
}