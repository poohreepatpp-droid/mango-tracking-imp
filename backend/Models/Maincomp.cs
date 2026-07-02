using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("maincomp")]
public class Maincomp
{
    [Key]
    [Column("maincode")]      public string  MainCode     { get; set; } = "";
    [Column("mainname")]      public string? MainName     { get; set; }
    [Column("web_re")]        public string? WebRe        { get; set; }
    [Column("show")]          public string? Show         { get; set; }
    [Column("default_login")] public string? DefaultLogin { get; set; }
}
