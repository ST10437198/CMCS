using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CMCS.Data;
using CMCS.Models;

namespace CMCS
{
    public partial class ApprovalsWindow : Window
    {
        private User _user;

        public ApprovalsWindow(User user)
        {
            InitializeComponent();
            _user = user;
            Title = $"Approvals - {_user.Username} ({_user.Role})";
            LoadClaims();
        }

        private async void LoadClaims()
        {
            try
            {
                var list = await DatabaseHelper.GetAllClaimsAsync(); // ✅ SHOW ALL CLAIMS
                dgClaims.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading claims: " + ex.Message);
            }
        }



        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadClaims();

        private async void BtnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is ClaimModel cm)
            {
                if (cm.Status == "Approved" || cm.Status == "Rejected")
                {
                    MessageBox.Show("This claim has already been processed.");
                    return;
                }

                if (MessageBox.Show($"Approve claim {cm.ClaimId}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    await DatabaseHelper.UpdateClaimStatusAsync(cm.ClaimId, "Approved", _user.UserId);
                    LoadClaims();
                }
            }
        }

        private async void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is ClaimModel cm)
            {
                if (cm.Status == "Approved" || cm.Status == "Rejected")
                {
                    MessageBox.Show("This claim has already been processed.");
                    return;
                }

                if (MessageBox.Show($"Reject claim {cm.ClaimId}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    await DatabaseHelper.UpdateClaimStatusAsync(cm.ClaimId, "Rejected", _user.UserId);
                    LoadClaims();
                }
            }
        }


        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is ClaimModel cm)
            {
                try
                {
                    if (!string.IsNullOrEmpty(cm.AttachmentPath))
                    {
                        Process.Start(new ProcessStartInfo(cm.AttachmentPath) { UseShellExecute = true });
                    }
                    else

                    if (cm.Status == "Approved" || cm.Status == "Rejected")
                    {
                        MessageBox.Show("This claim has already been processed.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("No attachment for this claim.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening file: " + ex.Message);
                }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
