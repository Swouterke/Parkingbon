using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace ParkingBon
{
    /// <summary>
    /// Interaction logic for ParkingBonWindow.xaml
    /// </summary>
    public partial class ParkingBonWindow : Window
    {
        public ParkingBonWindow()
        {
            InitializeComponent();
            Nieuw();
        }


        private void Nieuw()
        { 
            DatumBon.SelectedDate = DateTime.Now;
            AankomstLabelTijd.Content = DateTime.Now.ToLongTimeString();
            TeBetalenLabel.Content = "0 €";
            VertrekLabelTijd.Content = AankomstLabelTijd.Content;        
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Programma afsluiten ?", "Afsluiten", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void minder_Click(object sender, RoutedEventArgs e)
        {
            int bedrag = Convert.ToInt32(TeBetalenLabel.Content.ToString().Replace(" €", ""));
            if (bedrag > 0)
                bedrag -= 1;
            TeBetalenLabel.Content = bedrag.ToString() + " €";
            VertrekLabelTijd.Content = Convert.ToDateTime(AankomstLabelTijd.Content).AddHours(0.5 * bedrag).ToLongTimeString();
            SaveAndPrintEnableDisable(bedrag);
        }

        private void meer_Click(object sender, RoutedEventArgs e)
        {
            int bedrag = Convert.ToInt32(TeBetalenLabel.Content.ToString().Replace(" €", ""));
            DateTime vertrekuur = Convert.ToDateTime(AankomstLabelTijd.Content).AddHours(0.5 * bedrag);
            if (vertrekuur.Hour < 22)
                bedrag += 1;
            TeBetalenLabel.Content = bedrag.ToString() + " €";
            VertrekLabelTijd.Content = Convert.ToDateTime(AankomstLabelTijd.Content).AddHours(0.5 * bedrag).ToLongTimeString();
            SaveAndPrintEnableDisable(bedrag);
        }

        private void SaveAndPrintEnableDisable(int bedrag)
        {
            if (bedrag > 0)
            {
                Save.IsEnabled = true;
                SaveButton.IsEnabled = true;
                Print.IsEnabled = true;
                PrintButton.IsEnabled = true;
            }
            else
            {
                Save.IsEnabled = false;
                SaveButton.IsEnabled = false;
                Print.IsEnabled = false;
                PrintButton.IsEnabled = false;
            }
        }

        //statusbar
        private void NieuwOfNiet(object sender, RoutedEventArgs e)
        {
            BonInfo.Text = "nieuwe bon";
        }

        //Commands invullen
        //--NEW--
        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Nieuw();
        }
        //--SAVE--
        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                DateTime datum = (DateTime)DatumBon.SelectedDate;
                string aankomstTijd = AankomstLabelTijd.Content.ToString();
                string aankomstTijdGoed = aankomstTijd.Replace(":", "-");
                dlg.FileName = datum.Day + "-" + datum.Month + "-" + datum.Year + "om" + aankomstTijdGoed + ".bon";
                dlg.DefaultExt = ".bon";
                dlg.Filter = "Bonnen |*";
                if (dlg.ShowDialog() == true)
                {
                    using (StreamWriter bestand = new StreamWriter(dlg.FileName))
                    {
                        bestand.WriteLine(datum.ToShortDateString());
                        bestand.WriteLine(AankomstLabelTijd.Content);
                        bestand.WriteLine(TeBetalenLabel.Content);
                        bestand.WriteLine(VertrekLabelTijd.Content);
                    }
                    BonInfo.Text = System.IO.Path.GetFullPath(dlg.FileName).ToString() ; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij het opslaan: " + ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
        //--OPEN--
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = "Bon";
                dlg.DefaultExt = ".bon";
                dlg.Filter = "Bonnen |*.bon";
                if (dlg.ShowDialog() == true)
                {
                    using (StreamReader bestand = new StreamReader(dlg.FileName))
                    {
                        DateTime date = DateTime.ParseExact(bestand.ReadLine(), "dd/MM/yyyy", null);
                        DatumBon.SelectedDate = date;
                        AankomstLabelTijd.Content = bestand.ReadLine();
                        TeBetalenLabel.Content = bestand.ReadLine();
                        VertrekLabelTijd.Content = bestand.ReadLine();
                    }
                    BonInfo.Text = System.IO.Path.GetFullPath(dlg.FileName).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij het openen: " + ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
        //--PRINT--
        private double vertPositie;

        private void PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FixedDocument document = new FixedDocument();
            document.DocumentPaginator.PageSize = new System.Windows.Size(640, 320);
            PageContent inhoud = new PageContent();
            document.Pages.Add(inhoud);

            FixedPage page = new FixedPage();
            inhoud.Child = page;

            page.Width = 640;
            page.Height = 320;
            Image logo = new Image();
            logo.Source = logoImage.Source;
            logo.Margin = new Thickness(96);
            page.Children.Add(logo);

            vertPositie = 96;
            page.Children.Add(Regel("datum: " + DatumBon.Text));
            page.Children.Add(Regel("starttijd: " + AankomstLabelTijd.Content));
            page.Children.Add(Regel("eindtijd: " + VertrekLabelTijd.Content));
            page.Children.Add(Regel("bedrag betaald: " + TeBetalenLabel.Content));

            Afdrukvoorbeeld preview = new Afdrukvoorbeeld();
            preview.Owner = this;
            preview.AfdrukDocument = document;
            preview.ShowDialog();
        }
        private TextBlock Regel(string tekst)
        {
            TextBlock deRegel = new TextBlock();
            deRegel.Margin = new Thickness(300, vertPositie, 96, 96);
            deRegel.FontSize = 18;
            vertPositie += 36;
            deRegel.Text = tekst;
            return deRegel;
        }

        //--CLOSE--
        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

    }
}
