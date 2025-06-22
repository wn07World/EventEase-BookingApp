--dropping the EventEaseDB if it exists
drop database EventEaseSystemDatabase;

--database creation and use
create database EventEaseSystemDatabase;

use EventEaseSystemDatabase;

-- Create Venuee table
CREATE TABLE Venuee (
    Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    Name VARCHAR(250) NOT NULL,
    Location VARCHAR(250) NOT NULL,
    Capacity INT NOT NULL,
    ImageURL VARCHAR(250)
);

-- Create EvventTable
CREATE TABLE Evvent (
    Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    VenueeId INT NOT NULL,
    Name VARCHAR(250) NOT NULL,
    Date DATE NOT NULL,
    Description VARCHAR(250) NOT NULL,
    CONSTRAINT FK_Evvent_Venuee FOREIGN KEY (VenueeId) REFERENCES Venuee(id)
);

-- Create Bookingg table (fixed foreign key to reference EventTable)
CREATE TABLE Bookingg (
    Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    VenueeId INT NOT NULL,
    EvventId INT NOT NULL,
    BookingDate DATE NOT NULL,
    CONSTRAINT FK_Bookingg_Venuee FOREIGN KEY (VenueeId) REFERENCES Venuee(Id),
    CONSTRAINT FK_Bookingg_Evvent FOREIGN KEY (EvventId) REFERENCES Evvent(Id)
);



insert into Venuee (Name, Location, Capacity, ImageURL)
values ( 'Aruba','CountrySide', 50 ,'/images/pic123.jpeg');

drop table Bookingg;

insert into Evvent (VenueeId, Name, Date, Description)
values (1,'OntrackConnect', '02-20-2025','Dev ops meeting');


insert into Bookingg (VenueeId,EvventId, BookingDate)
values (1,1,'02-11-2002');

select * from Venuee;
select * from Evvent;