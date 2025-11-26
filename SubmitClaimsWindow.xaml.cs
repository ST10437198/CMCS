using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CMCS.Data;
using CMCS.Models;

namespace CMCS
{
    public partial class SubmitClaimsWindow : Window
    {
        private User _user;
        private string? _uploadedFilePath; // full path saved on disk

        private readonly string UploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");

        public SubmitClaimsWindow(User user)
        {
            InitializeComponent();
            _user = user;
            Title = $"Submit Claim - {_user.Username} ({_user.Role})";

            if (!Directory.Exists(UploadFolder))
                Directory.CreateDirectory(UploadFolder);
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Documents (*.pdf;*.docx;*.xlsx)|*.pdf;*.docx;*.xlsx|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                // copy file to UploadFolder with unique name
                var fname = Path.GetFileName(ofd.FileName);
                var dest = Path.Combine(UploadFolder, $"{Guid.NewGuid()}_{fname}");
                File.Copy(ofd.FileName, dest);
                _uploadedFilePath = dest;
                txtFileName.Text = fname;
            }
        }

        private async void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            lblStatus.Text = "";
            if (!decimal.TryParse(txtHours.Text.Trim(), out decimal hours))
            {
                lblStatus.Text = "Enter valid hours.";
                return;
            }

            if (!decimal.TryParse(txtRate.Text.Trim(), out decimal rate))
            {
                lblStatus.Text = "Enter valid rate.";
                return;
            }

            var claim = new ClaimModel
            {
                UserId = _user.UserId,
                HoursWorked = hours,
                HourlyRate = rate,
                Notes = txtNotes.Text.Trim(),
                AttachmentPath = _uploadedFilePath
            };

            try
            {
                int id = await DatabaseHelper.InsertClaimAsync(claim);
                lblStatus.Foreground = System.Windows.Media.Brushes.Green;
                lblStatus.Text = "Claim submitted successfully (ID: " + id + ")";
                // clear form
                txtHours.Text = "";
                txtRate.Text = "";
                txtNotes.Text = "";
                txtFileName.Text = "";
                _uploadedFilePath = null;
            }
            catch (Exception ex)
            {
                lblStatus.Foreground = System.Windows.Media.Brushes.Red;
                lblStatus.Text = "Error submitting claim: " + ex.Message;
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
