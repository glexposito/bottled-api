using System.ComponentModel.DataAnnotations;

namespace Bottled.Api.Dtos;

public class MessageDto
{
    [Required]
    [MaxLength(50)]
    public string? Author { get; set; }

    [MaxLength(255)]
    [Required]
    public string? Content { get; set; }
}