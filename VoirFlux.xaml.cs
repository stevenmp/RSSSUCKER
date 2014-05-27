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
using System.IO;
using System.Xml;
using System.ServiceModel.Syndication;
using System.IO.IsolatedStorage;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace RSSSUCKER
{
    public class Billet
    {
        public string Id { get; set; }
        public DateTime DatePublication { get; set; }
        public string Titre { get; set; }
        public Uri Url { get; set; }
        public bool EstDejaLu { get; set; }
    }
    public class Flux
    {
        public string Titre { get; set; }
        public Uri UrlImage { get; set; }
        public string Description { get; set; }
        public List<Billet> ListeBillets { get; set; }
    }
    public partial class VoirFlux : PhoneApplicationPage, INotifyPropertyChanged
    {
        private WebClient client;

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

        public VoirFlux()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            List<Uri> listeUrl;
            if (PhoneApplicationService.Current.State.ContainsKey("ListeUrl"))
                listeUrl = (List<Uri>)PhoneApplicationService.Current.State["ListeUrl"];
            else
                listeUrl = new List<Uri>();
            if (listeUrl.Count == 0)
            {
                MessageBox.Show("Vous devez d'abord ajouter des flux");
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
            else
            {
                Chargement.Text = "Chargement en cours ...";
                Chargement.Visibility = Visibility.Visible;
                ListeFlux = new ObservableCollection<Flux>();
                client = new WebClient();
                //queue = new Queue<Uri>();
                
                foreach (Uri uri in listeUrl)
                {
                    try
                    {
                        client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(AjouteFlux);
                        client.DownloadStringAsync(uri);
                        //string rss = await client.DownloadStringCompleted(uri);
                        //AjouteFlux(rss);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Impossible de lire le flux à l'adresse : " + uri + "\nVérifiez votre connexion internet");
                    }
                }
                Chargement.Text = string.Empty;
                Chargement.Visibility = Visibility.Collapsed;
            }
            base.OnNavigatedTo(e);
        }

        private void AjouteFlux(object sender, DownloadStringCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
            StringReader stringReader = new StringReader(e.Result);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader xmlReader = XmlReader.Create(stringReader, settings);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
            listeFlux.Add(ConstruitFlux(feed));
            PhoneApplicationService.Current.State["ListeFlux"] = ListeFlux.ToList();
            }
        }

        private Flux ConstruitFlux(SyndicationFeed feed)
        {
            Flux flux = new Flux { Titre = feed.Title.Text, UrlImage = feed.ImageUrl, Description = feed.Description == null ? string.Empty : feed.Description.Text, ListeBillets = new List<Billet>() };
            foreach (SyndicationItem item in feed.Items.OrderByDescending(e => e.LastUpdatedTime.DateTime))
            {
                Uri url = item.Links.Select(e => e.Uri).FirstOrDefault();
                if (url != null)
                {
                    Billet billet = new Billet { Id = url.AbsolutePath, DatePublication = item.LastUpdatedTime.DateTime, Titre = item.Title.Text, EstDejaLu = EstDejaLu(url.AbsolutePath), Url = url };
                    flux.ListeBillets.Add(billet);
                }
            }
            return flux;
        }

        private bool EstDejaLu(string id)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("ListeDejaLus"))
            {
                List<string> dejaLus = (List<string>)IsolatedStorageSettings.ApplicationSettings["ListeDejaLus"];
                bool any = dejaLus.Any(e => e == id);
                return any;
            }
            return false;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxFlux.SelectedItem != null)
            {
                Flux flux = (Flux)ListBoxFlux.SelectedItem;
                PhoneApplicationService.Current.State["FluxCourant"] = flux;
                NavigationService.Navigate(new Uri("/VoirFluxPivot.xaml", UriKind.Relative));
                ListBoxFlux.SelectedItem = null;
            }
        }
    }
}