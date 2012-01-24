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
INSERT INTO Supplier (CompanyName, City, Street, Number, Zip, State) 
	VALUES('Super Company', 'Praha', 'Dejvická', '156', '16000', 'Czech Republic');
INSERT INTO Supplier (CompanyName, City, Street, Number, Zip, State) 
	VALUES('AutoMirror', 'Úvaly u Prahy', 'Ostrovní', '4856', '15601', 'Czech Republic');
INSERT INTO Supplier (CompanyName, City, Street, Number, Zip, State) 
	VALUES('Cyril Trnka', 'Praha', 'Nábøežní', '1', '12003', 'Czech Republic');
INSERT INTO Supplier (CompanyName, City, Street, Number, Zip, State) 
	VALUES('Mein Factory', 'Dusseldorf', 'Leibnitz strasse', '56', '68952', 'Germany');
select * from supplier;


INSERT INTO Mirror (SerialNumber, Name, Type, SupplierId)
	VALUES('123-456-798', 'Ford Power Mirror', 0, 
		(SELECT Id FROM Supplier WHERE CompanyName = 'Super Company'));
INSERT INTO Mirror (SerialNumber, Name, Type, SupplierId)
	VALUES('789-456-468', 'Ford Power Mirror', 1, 
		(SELECT Id FROM Supplier WHERE CompanyName = 'Super Company'));
INSERT INTO Mirror (SerialNumber, Name, Type, SupplierId)
	VALUES('963-686-798', 'Ford Mirror', 0, 
		(SELECT Id FROM Supplier WHERE CompanyName = 'Super Company'));
INSERT INTO Mirror (SerialNumber, Name, Type, SupplierId)
	VALUES('963-686-798', 'Ford Mirror', 1, 
		(SELECT Id FROM Supplier WHERE CompanyName = 'Super Company'));
INSERT INTO Mirror (SerialNumber, Name, Type, SupplierId)
	VALUES('897-456-321', 'Ford Mirror', 0, 
		(SELECT Id FROM Supplier WHERE CompanyName = 'AutoMirror'));
INSERT INTO Mirror (SerialNumber, Name, Type, SupplierId)
	VALUES('459-465-129', 'Ford Mirror', 1, 
		(SELECT Id FROM Supplier WHERE CompanyName = 'AutoMirror'));
INSERT INTO Mirror (SerialNumber, Name, Type, SupplierId)
	VALUES('785-169-359', 'Ford Mirror', 2, 
		(SELECT Id FROM Supplier WHERE CompanyName = 'AutoMirror'));
INSERT INTO Mirror (SerialNumber, Name, Type, SupplierId)
	VALUES('489-897-359', 'Ford Mirror', 3, 
		(SELECT Id FROM Supplier WHERE CompanyName = 'AutoMirror'));
		
		
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