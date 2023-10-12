using System.ComponentModel.DataAnnotations;

namespace MoviesApplication.Models.MoviesModels
{
    public class MoviesShow
    {
        public int Id { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 4)]
        public string Title { get; set; } = string.Empty;
        [Required]
        public Genre Genre { get; set; }
        [Required]
        public decimal Rating { get; set; }
        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Imdb Link")]
        public string ImdbUrl { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Poster")]
        public string ImageUrl { get; set; } = string.Empty;

    }
}
