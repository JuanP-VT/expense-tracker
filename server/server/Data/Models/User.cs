
using System.ComponentModel.DataAnnotations;
public class User
{
    [Key]
    public int id { get; set; }
    [Required]
    [MinLength(4)]
    [MaxLength(20)]
    public string name { get; set; }
    [Required]
    [MaxLength(255)]
    public string hashedPass { get; set; }
    [Required]
    public int role { get; set; }
    [Required]
    public DateTime createdAt { get; set; }
}