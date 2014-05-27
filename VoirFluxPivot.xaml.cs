using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Globalization;

namespace RSSSUCKER
{
    public partial class VoirFluxPivot : PhoneApplicationPage, INotifyPropertyChanged
    {
        private ObservableCollection<Flux> listeFlux;
        public ObservableCollection<Flux> ListeFlux
        {
            get { return listeFlux; }
            set { NotifyPropertyChanged(ref listeFlux, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }

        private bool NotifyPropertyChanged<T>(ref T variable, T valeur, [CallerMemberName] string nomPropriete = null)
        {
            if (object.Equals(variable, valeur)) return false;

            variable = valeur;
            NotifyPropertyChanged(nomPropriete);
            return true;
        }

        public VoirFluxPivot()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ListeFlux = new ObservableCollection<Flux>((List<Flux>)PhoneApplicationService.Current.State["ListeFlux"]);

            base.OnNavigatedTo(e);
        }

        private void PivotFlux_Loaded(object sender, RoutedEventArgs e)
        {
            PivotFlux.SelectedItem = (Flux)PhoneApplicationService.Current.State["FluxCourant"];
            PivotFlux.SelectionChanged += PivotFlux_SelectionChanged;
        }

        private void ListeBoxBillets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if (listBox.SelectedItem != null)
            {
                Billet billet = (Billet)listBox.SelectedItem;
                PhoneApplicationService.Current.State["BilletCourrant"] = billet;
                NavigationService.Navigate(new Uri("/VoirBillet.xaml", UriKind.Relative));
                listBox.SelectedItem = null;
            }
        }

        private void PivotFlux_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PhoneApplicationService.Current.State["FluxCourant"] = PivotFlux.SelectedItem;
        }
    }
}