using Microsoft.AspNetCore.Http.HttpResults;
using POE_PART_1.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POE_PART_1.Models
{
    public class Venuee
    {

        public int VenueeId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Location { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater that 0.")]
        public int Capacity { get; set; }
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }
        public List<Evvent> Evvents { get; set; } = new();

    }
}
//--Create Venuee table
//CREATE TABLE Venuee (
//    Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
//    Name VARCHAR(250) NOT NULL,
//    Location VARCHAR(250) NOT NULL,
//    Capacity INT NOT NULL,
//    ImageURL VARCHAR(250)
//);
