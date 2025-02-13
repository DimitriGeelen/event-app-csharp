using System.ComponentModel.DataAnnotations;

namespace EventApp.Web.Models;

public class Event
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    [Display(Name = "Start Date/Time")]
    public DateTime StartDateTime { get; set; }
    
    [Required]
    [Display(Name = "End Date/Time")]
    public DateTime EndDateTime { get; set; }
    
    public string? Location { get; set; }
    
    public string? Address { get; set; }
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
    [Display(Name = "Image")]
    public string? ImagePath { get; set; }
    
    [Display(Name = "Document")]
    public string? DocumentPath { get; set; }
    
    // Navigation properties can be added here if needed
}