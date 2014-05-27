using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RSSSUCKER.Resources;
using System.IO.IsolatedStorage;

namespace RSSSUCKER
{
        public partial class MainPage : PhoneApplicationPage
        {
            // Constructeur
            public MainPage()
            {
                InitializeComponent();

                if (IsolatedStorageSettings.ApplicationSettings.Contains("ListeUrl"))
                {
                    PhoneApplicationService.Current.State["ListeUrl"] = IsolatedStorageSettings.ApplicationSettings["ListeUrl"];
                }
            }

            private void VoirFluxTap(object sender, System.Windows.Input.GestureEventArgs e)
            {
                NavigationService.Navigate(new Uri("/VoirFlux.xaml", UriKind.Relative));
            }

            private void AjouterFluxTap(object sender, System.Windows.Input.GestureEventArgs e)
            {
                NavigationService.Navigate(new Uri("/AjouterFlux.xaml", UriKind.Relative));
            }
        }
}