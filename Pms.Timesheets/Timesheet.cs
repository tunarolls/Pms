using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Pms.Timesheets
{
    public class Timesheet
    {
        #region Properties
        [JsonProperty("allowance")]
        public double Allowance { get; set; }

        public TimesheetBankChoices Bank { get; set; }

        public Cutoff Cutoff => new(CutoffId);

        public string CutoffId { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }

        public virtual EmployeeView? EE { get; set; }

        [JsonProperty("employee_id")]
        public string EEId { get; set; } = string.Empty;

        public string Fullname { get; set; } = string.Empty;

        [JsonProperty("is_confirmed")]
        public bool IsConfirmed { get; set; }

        public string Location { get; set; } = string.Empty;

        public int Page { get; set; }

        public string PayrollCode { get; set; } = string.Empty;

        [NotMapped]
        [JsonProperty("pcv")]
        public string[,]? PCV { get; set; }

        public string RawPCV { get; set; } = string.Empty;

        [Key]
        [Column("id")]
        public string TimesheetId { get; set; } = string.Empty;

        [JsonProperty("total_h_ot")]
        public double TotalHOT { get; set; }

        [JsonProperty("total_hours")]
        public double TotalHours { get; set; }

        [JsonProperty("total_nd")]
        public double TotalND { get; set; }

        [JsonProperty("total_ots")]
        public double TotalOT { get; set; }

        [JsonProperty("total_rd_ot")]
        public double TotalRDOT { get; set; }

        [JsonProperty("total_tardy")]
        public double TotalTardy { get; set; }
        #endregion 


        public static string GenerateId(Timesheet timesheet) => $"{timesheet.EEId}_{timesheet.CutoffId}";

        public void SetEmployeeDetail(EmployeeView? employee)
        {
            if (employee != null)
            {
                PayrollCode = employee.PayrollCode;
                Bank = employee.Bank;
                Fullname = employee.Fullname;
                Location = employee.Location;
            }
        }
    }
}