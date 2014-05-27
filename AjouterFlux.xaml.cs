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
    public partial class AjouterFlux : PhoneApplicationPage
    {
        private List<Uri> listeUrl;

        public AjouterFlux()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
    {
            Url.Text = "http://";
            if (PhoneApplicationService.Current.State.ContainsKey("ListeUrl"))
                listeUrl = (List<Uri>)PhoneApplicationService.Current.State["ListeUrl"];
            else
                listeUrl = new List<Uri>();
            base.OnNavigatedTo(e);
        }

        private void AjouterTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Uri uri;
            if (!Uri.TryCreate(Url.Text, UriKind.Absolute, out uri))
                MessageBox.Show("Le format de l'url est incorrect");
            else
            {
                listeUrl.Add(uri);
                IsolatedStorageSettings.ApplicationSettings["ListeUrl"] = listeUrl;
                PhoneApplicationService.Current.State["ListeUrl"] = listeUrl;
                MessageBox.Show("L'url a été correctement ajoutée");
            }
        }
    }
}