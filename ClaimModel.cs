using System;

namespace CMCS.Models
{
    public class ClaimModel
    {
        public int ClaimId { get; set; }
        public int UserId { get; set; }
        public DateTime DateSubmitted { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string Notes { get; set; }
        public string AttachmentPath { get; set; }
        public string Status { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedOn { get; set; }
        public string SubmitterUsername { get; set; } // optional helpful field
    }
}
