using Microsoft.AspNetCore.Http.HttpResults;

namespace POE_PART_1.Models
{
    public class Bookingg
    {
        public int Id { get; set; }
        public int VenueeId { get; set; }
        public int EvventId { get; set; }
        public DateTime BookingDate { get; set; }

        // Add navigation properties to resolve the error  
        public Venuee Venuee { get; set; }
        public Evvent Evvent { get; set; }
    }
}
//CREATE TABLE Bookingg (
//    Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
//    VenueeId INT NOT NULL,
//    EvventId INT NOT NULL,
//    BookingDate DATE NOT NULL,
//    CONSTRAINT FK_Bookingg_Venuee FOREIGN KEY (VenueeId) REFERENCES Venuee(Id),
//    CONSTRAINT FK_Bookingg_Evvent FOREIGN KEY (EvventId) REFERENCES Evvent(Id)
//);
