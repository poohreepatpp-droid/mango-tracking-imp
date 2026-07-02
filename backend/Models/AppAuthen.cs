using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("app_authen")]
public class AppAuthen
{
    [Key]
    [Column("session_id")]    public string  SessionId    { get; set; } = "";
    [Column("empno")]         public int     EmpNo        { get; set; }
    [Column("maincode")]      public string  MainCode     { get; set; } = "";
    [Column("app_name")]      public string  AppName      { get; set; } = "TEMPLATE";
    [Column("platform")]      public string? Platform     { get; set; } = "WEB";
    [Column("pass_code")]     public string? PassCode     { get; set; }
    [Column("device_id")]     public string? DeviceId     { get; set; }
    [Column("extension")]     public string? Extension    { get; set; }
    [Column("state_code")]    public string? StateCode    { get; set; }
    [Column("session_count")] public int?    SessionCount { get; set; }
    [Column("start_date")]    public DateTime  StartDate  { get; set; } = DateTime.Now;
    [Column("end_date")]      public DateTime  EndDate    { get; set; }
    [Column("last_access")]   public DateTime  LastAccess { get; set; } = DateTime.Now;
}
