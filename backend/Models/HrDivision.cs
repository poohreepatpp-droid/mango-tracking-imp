using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("hr_division")]
public class HrDivision
{
    [Column("maincode")]     public string MainCode { get; set; } = "";
    [Column("divcode")]      public string DivCode { get; set; } = "";
    [Column("divname")]      public string? DivName { get; set; }
    [Column("divname_eng")]  public string? DivNameEng { get; set; }
}
