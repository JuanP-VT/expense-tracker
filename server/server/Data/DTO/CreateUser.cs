using System.ComponentModel.DataAnnotations;

public class CreateUserDto
{
    [Required]
    [MinLength(4)]
    [MaxLength(20)]
    public string Name { set; get; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    public string Password { set; get; } = string.Empty;

}