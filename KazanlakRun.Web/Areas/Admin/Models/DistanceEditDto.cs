using KazanlakRun.GCommon;
using System.ComponentModel.DataAnnotations;

namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class DistanceEditDto
    {
        public int Id { get; set; }

        public string Distans { get; set; } = null!;

        [Range(ValidationConstants.RegRunnersMinNumber,
               ValidationConstants.RegRunnersMaxNumber,
               ErrorMessage = "Number of runners must be between {1} and {2}.")]
        public int RegRunners { get; set; } = 0;
    }
}


