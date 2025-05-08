using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class ClientPostDTO
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [Phone]
    public string Telephone { get; set; }
    
    [Required]
    [RegularExpression(@"^\d{11}$")]
    public string Pesel { get; set; }
}