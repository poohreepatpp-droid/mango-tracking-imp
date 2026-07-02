using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("hr_emp")]
public class HrEmp
{
    [Column("empno")]          public int     EmpNo       { get; set; }
    [Column("maincode")]       public string  MainCode    { get; set; } = "";
    [Column("userid")]         public string? UserId      { get; set; }
    [Column("userpass")]       public string? UserPass    { get; set; }
    [Column("empcode")]        public string? EmpCode     { get; set; }
    [Column("empfullname_t")]  public string? FullNameTh  { get; set; }
    [Column("empfullname_e")]  public string? FullNameEn  { get; set; }
    [Column("empname_t")]      public string? EmpNameTh   { get; set; }
    [Column("prename_t")]      public string? PreNameTh   { get; set; }
    [Column("email")]          public string? Email       { get; set; }
    [Column("emptel")]         public string? Phone       { get; set; }
    [Column("telephone")]      public string? Telephone   { get; set; }
    [Column("dpt_code")]       public string? DptCode     { get; set; }
    [Column("emppos")]         public string? EmpPos      { get; set; }
    [Column("empstatus")]      public string? EmpStatus   { get; set; }
    [Column("allow_login")]    public string? AllowLogin  { get; set; }
    [Column("entrelevel")]     public int?    EntreLevel  { get; set; }
    [Column("level")]          public int?    Level       { get; set; }
    [Column("lang_web")]       public string? LangWeb     { get; set; }
    [Column("active_sale")]    public string? ActiveSale  { get; set; }
    [Column("pathpic")]        public string? PathPic     { get; set; }
    [Column("pre_event")]      public string? PreEvent    { get; set; }
    [Column("ma_sign_ic")]     public string? MaSignIc    { get; set; }
    [Column("workstartdate")]  public DateTime? WorkStartDate { get; set; }
    [Column("add_dt")]         public DateTime? AddDate   { get; set; }
}
