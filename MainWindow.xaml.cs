using Claimsapps;
using System.Windows;

namespace CMCS
{
    public partial class MainWindow : Window
    {
        private InMemoryRepository Repo = new InMemoryRepository();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpenSubmit_Click(object sender, RoutedEventArgs e)
        {
            // pass repository and default lecturer user
            var submitWin = new SubmitClaimWindow(Repo);
            submitWin.Owner = this;
            submitWin.Show();
        }

        private void BtnOpenApprovals_Click(object sender, RoutedEventArgs e)
        {
            var appWin = new ApprovalsWindow(Repo);
            appWin.Owner = this;
            appWin.Show();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
