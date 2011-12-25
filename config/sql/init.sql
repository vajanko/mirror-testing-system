-------------------------------------------------------------------------------
-- INIT SCRIPT FOR MTS DATABASE

-- AUTHOR: ONDREJ KOVAC
-- DATE: 14.12.2011

-------------------------------------------------------------------------------

USE mts;

-- Add system operator with default password (admin)
INSERT INTO Operator (Name, Surname, Login, Password, Type)
	VALUES('System', 'Administrator', 'admin', 
	-- SHA2 hash of word 'admin', Type of administrator account is 0
	'8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 0);

-- Testing company (should be removed)
INSERT INTO Supplier (CompanyName) VALUES('Super Company');

INSERT INTO Mirror (SerialNumber, Name, Type, SupplierId)
	VALUES('123-456-798', 'Ford Power Mirror', 0, 
		(SELECT Id FROM Supplier WHERE CompanyName = 'Super Company'));

SELECT * from Supplier;		
select * from mirror;
select * from operator;

select * from testoutput order by id;
select * from test order by id;
select * from paramoutput order by id;
select * from param order by id;


select * from shift order by id;
select * from testshift order by id;
select * from test order by id;
select * from testparam order by id;
select * from param order by id;

-- delete everythig
DELETE FROM ParamOutput;
DELETE FROM TestOutput;
DELETE FROM TestShift;
DELETE FROM TestParam;
DELETE FROM Param;
DELETE FROM Test;
DELETE FROM Shift;