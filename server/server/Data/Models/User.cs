using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MinLength(4)]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string HashedPass { get; set; } = string.Empty;

    [Required]
    public int Role { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}
