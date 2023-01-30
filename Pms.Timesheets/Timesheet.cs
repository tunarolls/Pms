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
        [Key]
        [Column("id")]
        public string TimesheetId { get; set; } = string.Empty;

        [JsonProperty("employee_id")]
        public string? EEId { get; set; }
        public virtual EmployeeView? EE { get; set; }

        public string CutoffId { get; set; } = string.Empty;
        public Cutoff Cutoff => new(CutoffId);

        [JsonProperty("total_hours")]
        public double TotalHours { get; set; }

        [JsonProperty("total_ots")]
        public double TotalOT { get; set; }

        [JsonProperty("total_rd_ot")]
        public double TotalRDOT { get; set; }

        [JsonProperty("total_h_ot")]
        public double TotalHOT { get; set; }

        [JsonProperty("total_nd")]
        public double TotalND { get; set; }

        [JsonProperty("total_tardy")]
        public double TotalTardy { get; set; }

        [JsonProperty("allowance")]
        public double Allowance { get; set; }

        [NotMapped]
        [JsonProperty("pcv")]
        public string[,] PCV { get; set; } = new string[,] { };

        public string RawPCV { get; set; } = string.Empty;

        public double Adjust1 { get; set; }

        public double Adjust2 { get; set; }


        [JsonProperty("is_confirmed")]
        public bool IsConfirmed { get; set; }

        public int Page { get; set; }

        public DateTime DateCreated { get; set; }
        #endregion 

        public bool IsValid { get => IsConfirmed; }

    }

    public static class TimesheetExtensions
    {
        public static string GenerateId(this Timesheet timesheet)
        {
            return $"{timesheet.EEId}_{timesheet.CutoffId}";
        }
    }
}