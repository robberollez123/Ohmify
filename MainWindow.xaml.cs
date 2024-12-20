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

namespace Ohmify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<double> lijstMetSerieWeerstanden = new List<double>();
        List<double> lijstMetParallelWeerstanden = new List<double>();

        double serie,
               parallel,
               parTussenstap,
               resultaatSerie,
               resultaatPar,
               resultaat,
               nieuweWaarde;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnVoegSerieToe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double.TryParse(txbSerie.Text, out serie);
                lijstMetSerieWeerstanden.Add(serie);

                txbSerie.Clear();
                UpdateSerieListBox();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void btnVoegParallelToe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double.TryParse(txbParallel.Text, out parallel);
                lijstMetParallelWeerstanden.Add(parallel);

                txbParallel.Clear();
                UpdateParallelListBox();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void btnBerekenOhm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verkrijg de waarden van de tekstvakken en converteer ze naar double
                double? spanning = string.IsNullOrWhiteSpace(txbSpanning.Text) ? (double?)null : double.Parse(txbSpanning.Text);
                double? stroom = string.IsNullOrWhiteSpace(txbStroom.Text) ? (double?)null : double.Parse(txbStroom.Text);
                double? weerstand = string.IsNullOrWhiteSpace(txbWeerstand.Text) ? (double?)null : double.Parse(txbWeerstand.Text);

                // Als geen enkele waarde ingevuld is, geef een foutmelding
                if (spanning == null && stroom == null && weerstand == null)
                {
                    MessageBox.Show("Vul minimaal twee velden in!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Bereken de ontbrekende waarde
                if (spanning == null)
                {
                    if (stroom.HasValue && weerstand.HasValue)
                    {
                        // Spanning (U) berekenen: U = I * R
                        spanning = stroom.Value * weerstand.Value;
                        txbSpanning.Text = FormatValueWithUnits(spanning.Value, "V");
                    }
                    else
                    {
                        MessageBox.Show("Vul zowel stroom als weerstand in om de spanning te berekenen.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else if (stroom == null)
                {
                    if (spanning.HasValue && weerstand.HasValue)
                    {
                        // Stroom (I) berekenen: I = U / R
                        stroom = spanning.Value / weerstand.Value;
                        txbStroom.Text = FormatValueWithUnits(stroom.Value, "A");
                    }
                    else
                    {
                        MessageBox.Show("Vul zowel spanning als weerstand in om de stroom te berekenen.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else if (weerstand == null)
                {
                    if (spanning.HasValue && stroom.HasValue)
                    {
                        // Weerstand (R) berekenen: R = U / I
                        weerstand = spanning.Value / stroom.Value;
                        txbWeerstand.Text = FormatValueWithUnits(weerstand.Value, "Ω");
                    }
                    else
                    {
                        MessageBox.Show("Vul zowel spanning als stroom in om de weerstand te berekenen.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij berekening: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void btnBereken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                resultaat = parTussenstap = resultaatSerie = resultaatPar = 0;

                if (rbtSerie.IsChecked == true)
                {
                    foreach (double i in lijstMetSerieWeerstanden)
                    {
                        resultaatSerie += i;
                    }

                    txbResultaat.Text = FormatOhmValue(resultaatSerie);
                }
                else if (rbtParallel.IsChecked == true)
                {
                    foreach (double i in lijstMetParallelWeerstanden)
                    {
                        parTussenstap += 1 / i;
                    }

                    resultaatPar = Math.Round(1 / parTussenstap, 2);

                    txbResultaat.Text = FormatOhmValue(resultaatPar);
                }
                else if (rbtTotaal.IsChecked == true)
                {
                    foreach (double i in lijstMetSerieWeerstanden)
                    {
                        resultaatSerie += i;
                    }

                    foreach (double j in lijstMetParallelWeerstanden)
                    {
                        parTussenstap += 1 / j;
                    }

                    resultaatPar = Math.Round(1 / parTussenstap, 2);

                    resultaat = resultaatPar + resultaatSerie;
                    txbResultaat.Text = FormatOhmValue(resultaat);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void btnResetSpanning_Click(object sender, RoutedEventArgs e)
        {
            txbSpanning.Clear();
        }

        private void btnResetStroom_Click(object sender, RoutedEventArgs e)
        {
            txbStroom.Clear();
        }

        private void btnResetWeerstand_Click(object sender, RoutedEventArgs e)
        {
            txbWeerstand.Clear();
        }

        private void btnAanpassen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (double.TryParse(txbNieuweWaarde.Text, out nieuweWaarde))
                {
                    if (lsbSerie.SelectedIndex != -1)
                    {
                        int selectedIndex = lsbSerie.SelectedIndex;
                        lsbSerie.Items[selectedIndex] = nieuweWaarde;
                        lijstMetSerieWeerstanden[selectedIndex] = nieuweWaarde;

                        txbNieuweWaarde.Clear();
                    }
                    else if (lsbParallel.SelectedIndex != -1)
                    {
                        int selectedIndex = lsbParallel.SelectedIndex;
                        lsbParallel.Items[selectedIndex] = nieuweWaarde;
                        lijstMetParallelWeerstanden[selectedIndex] = nieuweWaarde;

                        txbNieuweWaarde.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Selecteer een weerstand om aan te passen.", "Geen weerstand geselecteerd", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Voer een geldige numerieke waarde in.", "Ongeldige waarde", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }


        private void btnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lsbSerie.SelectedIndex != -1)
                {
                    int selectedIndex = lsbSerie.SelectedIndex;

                    lijstMetSerieWeerstanden.RemoveAt(selectedIndex);
                    lsbSerie.Items.RemoveAt(selectedIndex);
                }
                else if (lsbParallel.SelectedIndex != -1)
                {
                    int selectedIndex = lsbParallel.SelectedIndex;

                    lijstMetParallelWeerstanden.RemoveAt(selectedIndex);
                    lsbParallel.Items.RemoveAt(selectedIndex);
                }
                else
                {
                    MessageBox.Show("Selecteer een weerstand om te verwijderen.", "Geen weerstand geselecteerd", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }


        private void UpdateSerieListBox()
        {
            if (lijstMetSerieWeerstanden.Count > 0)
            {
                lsbSerie.Items.Clear();

                foreach (double i in lijstMetSerieWeerstanden)
                {
                    lsbSerie.Items.Add(i);
                }
            }
            else
            {
                MessageBox.Show("Er zitten geen weerstanden in de lijst met serie.", "Lege lijst", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateParallelListBox()
        {
            if (lijstMetParallelWeerstanden.Count > 0)
            {
                lsbParallel.Items.Clear();

                foreach (double i in lijstMetParallelWeerstanden)
                {
                    lsbParallel.Items.Add(i);
                }
            }
            else
            {
                MessageBox.Show("Er zitten geen weerstanden in de lijst met parallel.", "Lege lijst", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // AI generated code
        private string FormatOhmValue(double value)
        {
            string prefix = value < 0 ? "-" : "";
            value = Math.Abs(value);

            if (value >= 1_000_000_000)
                return $"{prefix}{value / 1_000_000_000:0.#}GΩ"; // Gigaohms (G)
            else if (value >= 1_000_000)
                return $"{prefix}{value / 1_000_000:0.#}MΩ"; // Megaohms (M)
            else if (value >= 1_000)
                return $"{prefix}{value / 1_000:0.#}kΩ"; // Kiloohms (k)
            else if (value >= 1)
                return $"{prefix}{value}Ω"; // Ohms (Ω)
            else if (value >= 0.001)
                return $"{prefix}{value * 1_000:0.#}mΩ"; // MilliOhms (m)
            else if (value >= 0.000001)
                return $"{prefix}{value * 1_000_000:0.#}μΩ"; // Microohms (μ)
            else if (value >= 0.000000001)
                return $"{prefix}{value * 1_000_000_000:0.#}nΩ"; // Nanoohms (n)
            else
                return $"{prefix}{value * 1_000_000_000_000:0.#}pΩ"; // Picoohms (p)
        }

        private string FormatValueWithUnits(double value, string unit)
        {
            // Controleer de grootte van de waarde en voeg het juiste prefix toe
            if (value >= 1e9)
            {
                return (value / 1e9).ToString("0.##") + " G" + unit; // Giga
            }
            else if (value >= 1e6)
            {
                return (value / 1e6).ToString("0.##") + " M" + unit; // Mega
            }
            else if (value >= 1e3)
            {
                return (value / 1e3).ToString("0.##") + " k" + unit; // Kilo
            }
            else if (value >= 1)
            {
                return value.ToString("0.##") + " " + unit;
            }
            else if (value >= 1e-3)
            {
                return (value * 1e3).ToString("0.##") + " m" + unit; // Milli
            }
            else if (value >= 1e-6)
            {
                return (value * 1e6).ToString("0.##") + " μ" + unit; // Micro
            }
            else if (value >= 1e-9)
            {
                return (value * 1e9).ToString("0.##") + " n" + unit; // Nano
            }
            else
            {
                return value.ToString("0.##") + " " + unit;
            }
        }
    }
}