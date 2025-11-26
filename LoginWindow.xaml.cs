using CMCS.Data;
using CMCS.Models;
using System.Windows;
using System.Windows.Controls;

namespace CMCS
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            cbRole.SelectedIndex = 0;
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            lblStatus.Text = "";
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;
            string role = ((ComboBoxItem)cbRole.SelectedItem).Content.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblStatus.Text = "Please enter username and password.";
                return;
            }

            var user = await DatabaseHelper.GetUserByUsernameAsync(username);
            if (user == null || user.Password != password || user.Role != role)
            {
                lblStatus.Text = "Invalid credentials or role.";
                return;
            }

            // Open respective window
            if (role == "Lecturer")
            {
                var submitWindow = new SubmitClaimsWindow(user);
                submitWindow.Show();
                this.Close();
            }
            else if (role == "Coordinator")
            {
                var approvalsWindow = new ApprovalsWindow(user);
                approvalsWindow.Show();
                this.Close();
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
