using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MoviesApplication.Models.MoviesModels
{
    public enum Genre
    {
        Drama,
        Comedy,
        Romance,
        [Display(Name = "Romantic Comedy")]
        RomCom,
        Crime,
        Mystery
    }
}
