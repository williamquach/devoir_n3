using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GestionnaireBDD;
using ClassesMetier;
using Windows.UI.Popups;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPGestionSalles
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        GstBdd bdd;
        List<Place> lesPlacesR;
        double prix;
        Manifestation laManifChoisie;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnReserver_Click(object sender, RoutedEventArgs e)
        {
            if (lstManifs.SelectedItem == null)
            {
                var msgPasManif = new MessageDialog("Sélectionnez une manifestation.");
                await msgPasManif.ShowAsync();
            }
            else if ((gvPlaces.SelectedItem) == null || (gvPlaces.SelectedItem as Place).Etat == 'l')
            {
                var pasPlaceSelect = new MessageDialog("Sélectionnez au moins une place.");
                await pasPlaceSelect.ShowAsync();
            }
            else
            {
                foreach (Place p in lesPlacesR)
                {
                    bdd.ReserverPlace(p.IdPlace, Convert.ToInt16(txtNumSalle.Text), laManifChoisie.IdManif);
                }
                gvPlaces.ItemsSource = null;
                gvPlaces.ItemsSource = bdd.GetAllPlacesByIdManifestation(laManifChoisie.IdManif, Convert.ToInt16(txtNumSalle.Text));
                var placesReservees = new MessageDialog("Vos places sont réservées. Prix total : " + prix + " pour " + lesPlacesR.Count() + " places");
                await placesReservees.ShowAsync();
            }
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bdd = new GstBdd();
            lesPlacesR = new List<Place>();
            prix = 0;
            lstManifs.ItemsSource = bdd.GetAllManifestations();
            lstTarifs.ItemsSource = bdd.GetAllTarifs();
        }

        private void lstManifs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstManifs.SelectedItem != null)
                {
                laManifChoisie = lstManifs.SelectedItem as Manifestation;
                txtNbPlaces.Text = laManifChoisie.LaSalle.NbPlaces.ToString();
                txtNomSalle.Text = laManifChoisie.LaSalle.NomSalle;
                txtNumSalle.Text = laManifChoisie.LaSalle.IdSalle.ToString();

                gvPlaces.ItemsSource = bdd.GetAllPlacesByIdManifestation(laManifChoisie.IdManif, Convert.ToInt16(txtNumSalle.Text));

            }
        }
        
        private async void gvPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(gvPlaces.SelectedItem != null && (gvPlaces.SelectedItem as Place).Etat == 'l')
            {
                (gvPlaces.SelectedItem as Place).Etat = 'r';
                lesPlacesR.Add(gvPlaces.SelectedItem as Place);
                prix += (gvPlaces.SelectedItem as Place).Prix;
                txtTotal.Text = prix.ToString();
            }

            else if (gvPlaces.SelectedItem != null && (gvPlaces.SelectedItem as Place).Etat == 'r') {
                (gvPlaces.SelectedItem as Place).Etat = 'l';
                lesPlacesR.Remove(gvPlaces.SelectedItem as Place);
                prix -= (gvPlaces.SelectedItem as Place).Prix;
                txtTotal.Text = prix.ToString();
            }

            else if (gvPlaces.SelectedItem != null && (gvPlaces.SelectedItem as Place).Etat == 'o')
            {
                var messageDejaReserve = new MessageDialog("Cette place est déjà réservée.");
                await messageDejaReserve.ShowAsync();
            }
        }
    }
}
