using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace POE_PART_1.Models
{
    public class Evvent
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int VenueeId { get; set; }  // Foreign key
        public DateTime Date { get; set; }
        public string Description { get; set; }
        

        public Venuee Venuee { get; set; } // Navigation property  

        public int? EvventTypeId { get; set; }

        public EvventType? EvventType { get; set; } 


        public List<Bookingg> Bookingg { get; set; } = new(); // Navigation property                                                                 

    }
}
//CREATE TABLE Evvent (
//    Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
//    VenueeId INT NOT NULL,
//    Name VARCHAR(250) NOT NULL,
//    Date DATE NOT NULL,
//    Description VARCHAR(250) NOT NULL,
//    CONSTRAINT FK_Evvent_Venuee FOREIGN KEY (VenueeId) REFERENCES Venuee(id)
//);
