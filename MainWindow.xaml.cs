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
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;

namespace CMCS
{
    public partial class MainWindow : Window
    {
        private readonly Claim CurrentClaim;

        public MainWindow()
        {
            InitializeComponent();
            CurrentClaim = new Claim();
            dgLineItems.ItemsSource = CurrentClaim.LineItems;
        }

        private void UpdatePreview()
        {
            CurrentClaim.CalculateTotals();

            lblContractValue.Text = CurrentClaim.ContractValue.ToString("C");
            lblThisMonthTotal.Text = CurrentClaim.ThisMonthTotal.ToString("C");
            lblAccumulatedTotal.Text = CurrentClaim.AccumulatedTotal.ToString("C");
            lblRetention.Text = CurrentClaim.Retention.ToString("C");
            lblNetClaim.Text = CurrentClaim.NetClaim.ToString("C");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
            MessageBox.Show("Claim saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
            MessageBox.Show("Claim submitted successfully.", "Submit", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
            MessageBox.Show("Claim exported (placeholder).", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
