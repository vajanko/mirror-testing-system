-------------------------------------------------------------------------------
-- CREATE SCRIPT FOR MTS DATABASE

-- AUTHOR: ONDREJ KOVAC
-- DATE: 12.2.2011

-------------------------------------------------------------------------------

-- REMOVE THIS WHEN EXECUTING SCRIPT DURING INSTALATION
--USE mts;

---------------------
--- CREATE TALBES ---
---------------------

--#region CREATE TABLE SUPPLIER
-- Suppliers of mirror products. Basic info about name and address. This table is not very important foe
-- out application
IF OBJECT_ID('Supplier', N'U') IS NOT NULL
	DROP TABLE Supplier;
CREATE TABLE Supplier (
	-- primary key
	Id INT IDENTITY(1,1),
	-- Name of supplier company
	CompanyName NVARCHAR(50) NOT NULL,
	-- following items are address supplier company residence there may be also more addresses of supplier
	-- this could be made better with a table of addresses
	City NVARCHAR(20),
	Street NVARCHAR(20),
	Number INT,
	Zip INT,
	-- state could be reference to table of all states, but depends on how smart our application should be
	State NVARCHAR(30),
	
	-- primary key Id
	CONSTRAINT pk_supplier_id PRIMARY KEY (Id),
	-- unique company name
	CONSTRAINT uk_supplier_companyName UNIQUE(CompanyName)
);

--#region CREATE PROCEDURE udpAddSupplier
IF OBJECT_ID('udpAddSupplier') IS NOT NULL
	DROP PROCEDURE udpAddSupplier;
GO
-- Add a new supplier of mirrors
CREATE PROCEDURE udpAddSupplier(@companyName NVARCHAR(50), @city NVARCHAR(20), @street NVARCHAR(20),
		@number INT, @zip INT, @state NVARCHAR(30))
AS
BEGIN TRAN
	-- This is very simple procedure and INSERT could be called directly. But in the future additional
	-- operations could be added
	INSERT INTO Supplier(CompanyName, City, Street, Number, Zip, State)
		VALUES(@companyName, @city, @street, @number, @zip, @state)
COMMIT
GO
--#endregion

--#region CREATE PROCEDURE udpDeleteSupplier
IF OBJECT_ID('udpDeleteSupplier') IS NOT NULL
	DROP PROCEDURE udpDeleteSupplier;
GO
-- Delete supplier and all data asocaited with it. Use this procedure very carefully. It is probalbly
-- unnecesary to use it (is some strange cases when business strategy is changing its direction ...)
-- This procedure will delete all mirrors supplied by specified supplier, all shifts that has been
-- executed to test a mirror type supplied by this suppliers, ...
CREATE PROCEDURE udpDeleteSupplier(@supplierId INT)
AS
BEGIN TRAN
	-- Before supplier will be deleted all its data must be removed
	-- Delete all mirrors supplied by suppler that should be deleted
	
	-- 1) delete all parameter outputs produced in a shift testing mirror supplied
	-- by deleting supplier
	DELETE P FROM ParamOutput P
		JOIN TestOutput T ON (P.TestOutputId = T.Id)
		JOIN Shift S ON (T.ShiftId =  S.Id)
		JOIN Mirror M ON (S.MirrorId = M.Id)
		WHERE M.SupplierId = @supplierId;
		
	-- 2) delete all test outputs produces in a shift testing mirror supplied by
	-- deleting supplier
	DELETE T FROM TestOutput T
		JOIN Shift S ON (T.ShiftId = S.Id)
		JOIN Mirror M ON (S.MirrorId = M.Id)
		WHERE M.SupplierId = @supplierId;
		
	-- 3) delete shift references to used tests
	DELETE TS FROM TestShift TS
		JOIN Shift S ON (TS.ShiftId = S.Id)
		JOIN Mirror M ON (S.MirrorId = M.Id)
		WHERE M.SupplierId = @supplierId;
	
	-- 4) delete all shifts which have tested mirror supplied by deleting supplier
	DELETE S FROM Shift S
		JOIN Mirror M ON (M.Id = S.MirrorId)
		WHERE M.SupplierId = @supplierId;
	
	-- 5) delete all mirrors supplied by deleing supplier
	DELETE FROM Mirror WHERE Mirror.SupplierId = @supplierId;
	
	-- 6) At the end supplier can be deleted as nothing is referencing it
	DELETE FROM Supplier WHERE Supplier.Id = @supplierId;
COMMIT
GO
--#endregion

--#endregion

--#region CREATE TABLE MIRROR
-- Mirror types supplied by one of suppliers
IF OBJECT_ID('Mirror', N'U') IS NOT NULL
	DROP TABLE Mirror;
CREATE TABLE Mirror (
	-- primary key
	Id INT IDENTITY(1,1),
	-- serial number must be unic, it has standard format of constant length
	SerialNumber CHAR(15) NOT NULL,
	-- commercial Name (short description) of mirror
	Name NVARCHAR(20) NOT NULL,
	-- long description of mirror (must not be defined)
	Description NVARCHAR(50),
	-- type of mirror (enum): left or right, for left or right handed car
	Type TINYINT DEFAULT(0) NOT NULL,
	-- Id of supplier who is producing this mirror
	SupplierId INT NOT NULL,
	
	-- primary key Id
	CONSTRAINT pk_mirror_id PRIMARY KEY (Id),
	-- unique serial number
	CONSTRAINT uq_mirror_serialNumber UNIQUE (SerialNumber),
	-- min and max value of mirror type enum
	CONSTRAINT chk_mirror_type CHECK (Type >= 0 AND TYPE < 4),
	-- foreign key referencing supplier who is procuding this mirror
	CONSTRAINT fk_mirror_supplier_id FOREIGN KEY (SupplierId) REFERENCES Supplier(Id)
);
-- Create index for foreign key referencing supplier of the mirror
CREATE INDEX fk_mirror_supplier_id ON Mirror(SupplierId);

--#region CREATE PROCEDURE udpAddMirror
IF OBJECT_ID('udpAddMirror') IS NOT NULL
	DROP PROCEDURE udpAddMirror;
GO
-- Add a new operator
CREATE PROCEDURE udpAddMirror(@serial CHAR(15), @name NVARCHAR(20), @desc NVARCHAR(50),
	@type TINYINT, @supplierId INT)
AS
BEGIN TRAN
	INSERT INTO Mirror(SerialNumber, Name, Description, Type, SupplierId)
		VALUES(@serial, @name, @desc, @type, @supplierId);
COMMIT
GO
--#endregion

--#region CREATE PROCEDURE udpDeleteMirror
IF OBJECT_ID('udpDeleteMirror') IS NOT NULL
	DROP PROCEDURE udpDeleteMirror;
GO
CREATE PROCEDURE udpDeleteMirror(@mirrorId INT)
AS
BEGIN TRAN
	-- 1) delete all parameter outputs produced in a shift testing deleting mirror
	DELETE P FROM ParamOutput P
		JOIN TestOutput ON (P.TestOutputId = TestOutput.Id)
		JOIN Shift ON (TestOutput.ShiftId =  Shift.Id)
		WHERE Shift.MirrorId = @mirrorId;
		
	-- 2) delete all test outputs produces in a shift executed testing deleting mirror
	DELETE T FROM TestOutput T
		JOIN Shift ON (T.ShiftId = Shift.Id)
		WHERE Shift.MirrorId = @mirrorId;
		
	-- 3) delete shift references to used tests
	DELETE TS FROM TestShift TS
		JOIN Shift ON (TS.ShiftId = Shift.Id)
		WHERE Shift.MirrorId = @mirrorId;
	
	-- 4) delete all shifts which have tested deleting mirror
	DELETE FROM Shift WHERE Shift.MirrorId = @mirrorId;
	
	-- 5) delete mirror
	DELETE FROM Mirror WHERE Mirror.Id = @mirrorId;
	
COMMIT
GO
--#endregion

--#endregion

--#region CREATE TABLE OPERATOR
-- Users that log into the application and execute shifts and tests
IF OBJECT_ID('Operator', N'U') IS NOT NULL
	DROP TABLE Operator;
CREATE TABLE Operator (
	-- primary key
	Id INT IDENTITY (1,1),
	-- Name of operator
	Name NVARCHAR(20) NOT NULL,
	-- Surname of operator
	Surname NVARCHAR(20) NOT NULL,
	-- Name of operator used to log into the application
	Login VARCHAR(20) NOT NULL,
	-- SHA2 hash value of operator password used to authenticate in the application
	Password VARCHAR(64) NOT NULL,
	-- Type of operator (Role): application defines security permissions based
	-- on this value. Allowed values are 0-255
	Type TINYINT DEFAULT(255) NOT NULL,	-- expecting that 0 will be admin
	
	-- primary key Id
	CONSTRAINT pk_operator_id PRIMARY KEY (Id),
	-- unique operator login
	CONSTRAINT uq_operator_login UNIQUE (LOGIN),
	-- unique name and surname of operator
	CONSTRAINT uq_operator_name_surname UNIQUE(Name, Surname)
);

--#region CREATE PROCEDURE udpAddOperator
IF OBJECT_ID('udpAddOperator') IS NOT NULL
	DROP PROCEDURE udpAddOperator;
GO
-- Add a new operator
CREATE PROCEDURE udpAddOperator(@name NVARCHAR(20), @surname NVARCHAR(20), @login VARCHAR(20),
	@password VARCHAR(64), @type TINYINT)
AS
BEGIN TRAN
	INSERT INTO Operator(Name, Surname, Login, Password, Type)
		VALUES(@name, @surname, @login, @password, @type);
COMMIT
GO
--#endregion

--#region CREATE PROCEDURE udpDeleteOperator
IF OBJECT_ID('udpDeleteOperator', N'P') IS NOT NULL
	DROP PROCEDURE udpDeleteOperator;
GO
CREATE PROCEDURE udpDeleteOperator(@operatorId INT)
AS
BEGIN TRAN
	-- 1) delete all parameter outputs produced in a shift executed by deleing operator
	DELETE P FROM ParamOutput P
		JOIN TestOutput T ON (P.TestOutputId = T.Id)
		JOIN Shift S ON (T.ShiftId =  S.Id)
		WHERE S.OperatorId = @operatorId;
		
	-- 2) delete all test outputs produces in a shift executed by deleting operator
	DELETE T FROM TestOutput T
		JOIN Shift S ON (T.ShiftId = S.Id)
		WHERE S.OperatorId = @operatorId;
		
	-- 3) delete shift references to used tests
	DELETE TS FROM TestShift TS
		JOIN Shift S ON (TS.ShiftId = S.Id)
		WHERE S.OperatorId = @operatorId;
	
	-- 4) delete all shifts which have been executed by deleting operator
	DELETE FROM Shift WHERE Shift.OperatorId = @operatorId;
	
	-- 5) delete operator
	DELETE FROM Operator WHERE Operator.Id = @operatorId;
COMMIT
GO		
--#endregion

--#endregion

--#region CREATE TABLE TEST
-- In this table are stored any tests that have been used in some shift. When new shift is added to
-- database its test are added to this table, but only when they are not already in. Otherwise they 
-- are referenced only. By now test has no other properties (only Name), so probalbly this table will 
-- not change very often (like Param). When test is not referenced by any shift, it is automatically 
-- deleted by a trigger (this happens when shift or operator and his or her data are deleted)
IF OBJECT_ID('Test', N'U') IS NOT NULL
	DROP TABLE Test;
CREATE TABLE Test (
	-- primary key
	Id INT IDENTITY (1,1),
	-- Name of test (in application this is its Id - must be unique)
	Name VARCHAR(25) NOT NULL,
	
	-- primary key Id
	CONSTRAINT pk_test_id PRIMARY KEY (Id),
	-- unique Name of test
	CONSTRAINT uq_test_name UNIQUE (Name)
);

--#region CREATE TRIGGER trTest_del_after
IF OBJECT_ID('trTest_del_after') IS NOT NULL
	DROP TRIGGER trTest_del_after;
GO
CREATE TRIGGER trTest_del_after ON Test
AFTER DELETE
AS
BEGIN
	-- delete all unsused parameters (without references)
	DELETE P FROM Param P
		WHERE NOT EXISTS (SELECT * FROM TestParam WHERE TestParam.ParamId = P.Id)
		AND NOT EXISTS (SELECT * FROM ParamOutput WHERE ParamOutput.ParamId = P.Id);	
END
GO
--#endregion

--#region CREATE PROCEDURE udpAddTest

IF OBJECT_ID('udpAddTest', N'P') IS NOT NULL
	DROP PROCEDURE udpAddTest;
-- Add a new test to Test table (if it does not exists) and return inserted or already
-- existing value
GO
CREATE PROCEDURE udpAddTest(@name VARCHAR(25), @shiftId INT)
AS
BEGIN TRAN
	-- check if such a test already exists
	DECLARE @count INT;
	SET @count = (SELECT COUNT(*) FROM Test 
		WHERE Name = @name);
		
	-- if test does not exists yet - add a new one. Even if parallel transaction are running
	-- unique constraint will rollback one of them if both try to insert new parameter
	IF @count = 0
		-- insert new test
		INSERT INTO Test (Name)
			VALUES(@name)
	-- add test to shift
	INSERT INTO TestShift(TestId, ShiftId)
		VALUES((SELECT TOP 1 Id FROM Test WHERE Name = @name), @shiftId);
	
	-- now select added or already existing test. This strange behaviour is required by
	-- entity framework - this value will be returned as output
	SELECT TOP 1 * FROM Test
		WHERE (Name = @name);
COMMIT TRAN
GO
--#endregion  

--#endregion

--#region CREATE TABLE PARAM
-- In this table are stored any parameters that has been used in some test. When new test is added to
-- database its parameters are added to this table, but only when they are not already in. Otherwise they
-- are referenced only. When parameter is not referenced by any test it is automatically deleted by a
-- trigger (this happens when shift or operator and his or her data are deleted)
IF OBJECT_ID('Param', N'U') IS NOT NULL
	DROP TABLE Param;
CREATE TABLE Param (
	-- primary key
	Id INT IDENTITY (1,1),
	-- Name of parameter (this value is unique inside particular test)
	Name VARCHAR(25) NOT NULL,
	-- value of parameter in string representation
	Value VARCHAR(50) NOT NULL,
	-- type of value data (int, string, bool, ...). This is application dependant data
	Type TINYINT DEFAULT(0) NOT NULL,
	-- unit of numeric parameter (some parameters are witout unit)
	Unit VARCHAR(2),
	
	-- primary key Id
	CONSTRAINT pk_param_id PRIMARY KEY (Id),
	-- unique Name and Value of parmater 
	CONSTRAINT uq_param_name_value_type_unit UNIQUE (Name, Value, Type, Unit),
	-- min value of parameter value type
	CONSTRAINT chk_param_type CHECK (Type >= 0)
);

--#region CREATE PROCEDURE udpAddParam
IF OBJECT_ID('udpAddParam', N'P') IS NOT NULL
	DROP PROCEDURE udpAddParam;
-- Add a new parameter to Param table (if it does not exists) and return inserted or already
-- existing values
GO
CREATE PROCEDURE udpAddParam(@testId INT, @name VARCHAR(25), @value VARCHAR(50), @type TINYINT,
	@unit VARCHAR(2))
AS
BEGIN TRAN
	-- check if such a parameter already exists
	DECLARE @count INT;
	SET @count = (SELECT COUNT(*) FROM Param WITH (HOLDLOCK)
		WHERE Name = @name AND Value = @value AND Type = @type AND (Unit IS NULL OR Unit = @unit));
		
	-- if parameter does not exists yet - add a new one. Even if parallel transaction are running
	-- unique constraint will rollback one of them if both try to insert new parameter
	IF @count = 0
	BEGIN
		-- insert new parameter
		INSERT INTO Param (Name, Value, Type, Unit)
			VALUES(@name, @value, @type, @unit)
		-- insert relationship between test and this parameter
		INSERT INTO TestParam (TestId, ParamId)
			VALUES(@testId, @@IDENTITY);
	END;
		
	-- now select added or already existing parameter. This strange behaviour is required by
	-- entity framework - this value will be returned as output
	SELECT TOP 1 * FROM Param
		WHERE (Name = @name AND Value = @value AND Type = @type AND (Unit IS NULL OR Unit = @unit));
COMMIT TRAN
GO
--#endregion
--#endregion

--#region CREATE TABLE TESTPARAM
-- This table describes relationship between Test and Param. In one test there
-- are multiple parameters and parameter may be contained in multiple tests
IF OBJECT_ID('TestParam', N'U') IS NOT NULL
	DROP TABLE TestParam;
CREATE TABLE TestParam (
	-- primary key
	Id INT IDENTITY(1,1),
	-- This table creates a relationship between test and parameter: parameter referenced by
	-- ParamId belongs to test referenced by TestId
	TestId INT NOT NULL,
	ParamId INT NOT NULL,
	
	-- primary key Id
	CONSTRAINT pk_testParam_id PRIMARY KEY (Id),
	-- foreign key referencing test
	CONSTRAINT fk_testParam_testId FOREIGN KEY (TestId) REFERENCES Test(Id),
	-- foreign key referencing param
	CONSTRAINT fk_testParam_paramId FOREIGN KEY (ParamId) REFERENCES Param(Id),
	-- unique test, param and shift id
	CONSTRAINT uq_testParam_testId_paramId
		UNIQUE(TestId, ParamId)
);

-- Create index for foreign key referencing test
CREATE INDEX fk_testParam_testId ON TestParam(TestId);
-- Create index for foreign key referencing param
CREATE INDEX fk_testParam_paramId ON TestParam(ParamId);
--#endregion

--#region CREATE TABLE SHIFT
-- Data related to sequence of executed tests. These tests are executed by a particular operator on
-- a particular mirror type
IF OBJECT_ID('Shift', N'U') IS NOT NULL
	DROP TABLE Shift;
CREATE TABLE Shift (
	-- primary key
	Id INT IDENTITY(1,1),
	-- Date and time when shift has been started
	Start DATETIME NOT NULL,
	-- Date and time when shift has been finished
	Finish DATETIME NOT NULL,
	-- Id of mirror that has been tested in this shift
	MirrorId INT NOT NULL,
	-- Id of operator who has executed this shift
	OperatorId INT NOT NULL,
	
	-- primary key Id
	CONSTRAINT pk_shift_id PRIMARY KEY(Id),
	-- start time must be lower than end time
	CONSTRAINT chk_shift_start_finish CHECK(Start <= Finish),
	-- foreign key referencing Id of mirror being tested in this shift
	CONSTRAINT fk_shift_mirrorId FOREIGN KEY(MirrorId) REFERENCES Mirror(Id),
	-- foreign key referencing Id of operator who has executed this shift
	CONSTRAINT fk_shift_operatorId FOREIGN KEY(OperatorId) 
		REFERENCES Operator(Id)
);

--#region CREATE TRIGGER trShift_del_after
IF OBJECT_ID('trShift_del_after') IS NOT NULL
	DROP TRIGGER trShift_del_after;
GO
-- When a shift is deleted all relations to tests will be deleted (this realtions means that
-- test have been used in this shift
CREATE TRIGGER trShift_del_after ON Shift
AFTER DELETE
AS
BEGIN
	-- 0) create temporary table to store ids of test that should be deleted
	IF OBJECT_ID('tempdb..#testId') IS NULL
		CREATE TABLE #testId(Id INT);
	ELSE
		DELETE FROM #testId;
	-- get ids of tests that should be deleted (without references)
	INSERT INTO #testId(Id)
		SELECT Test.Id FROM Test
			WHERE NOT EXISTS(SELECT * FROM TestShift WHERE TestShift.TestId = Test.Id)
				AND NOT EXISTS(SELECT * FROM TestOutput WHERE TestOutput.TestId = Test.Id);
	
	-- 1) delete all relationship between tests that will be deleted and some parameters
	DELETE TP FROM TestParam TP
		JOIN #testId ON (#testId.Id = TP.TestId);
		
	-- 2) delete all test that are without references (unused)
	DELETE T FROM Test T
		JOIN #testId ON (#testId.Id = T.Id);
END
GO
--#endregion

--#region CREATE PROCEDURE udpStartShift

IF OBJECT_ID('udpStartShift', N'P') IS NOT NULL
	DROP PROCEDURE udpStartShift;
GO
CREATE PROCEDURE udpStartShift(@start DATETIME, @mirrorId INT, @operatorId INT)
AS
BEGIN
	-- insert new shift - it has been not finished yet (start == finish)
	INSERT INTO Shift(Start, Finish, MirrorId, OperatorId)
		VALUES(@start, @start, @mirrorId, @operatorId);
	
	-- now select added shift. This strange behaviour is required by entity framework 
	--- this value will be returned as output
	SELECT TOP 1 * FROM Shift
		WHERE (Id = @@IDENTITY);
END
GO
--#endregion

--#region CREATE PROCEDURE udpFinishShift

IF OBJECT_ID('udpFinishShift', N'P') IS NOT NULL
	DROP PROCEDURE udpFinishShift;
GO
CREATE PROCEDURE udpFinishShift(@shiftId INT, @finish DATETIME)
AS
BEGIN TRAN
	UPDATE Shift SET Finish = @finish WHERE Id = @shiftId;
COMMIT
GO
--#endregion

--#region CREATE PROCEDURE udpDeleteShift

IF OBJECT_ID('udpDeleteShift', N'P') IS NOT NULL
	DROP PROCEDURE udpDeleteShift;
GO
CREATE PROCEDURE udpDeleteShift(@shiftId INT)
AS
BEGIN
	-- 1) delete all parameter outputs produced in deleting shift 
	DELETE P FROM ParamOutput P
		JOIN TestOutput T ON (P.TestOutputId = T.Id)
		WHERE T.ShiftId = @shiftId;
		
	-- 2) delete all test outputs produces in deleting shift
	DELETE FROM TestOutput
		WHERE TestOutput.ShiftId = @shiftId;
		
	-- 3) delete shift references to used tests
	DELETE FROM TestShift
		WHERE TestShift.ShiftId = @shiftId;
	
	-- 4) delete all shifts which have been executed by deleting operator
	DELETE FROM Shift WHERE Shift.Id = @shiftId;
END
GO
--#endregion  

-- Create index for foreign key referencing mirror
CREATE INDEX fk_shift_mirrorId ON Shift(MirrorId);
-- Create index for foreign key referencing operator
CREATE INDEX fk_shift_operatorId ON Shift(OperatorId);
--#endregion

--#region CREATE TABLE TESTSHIFT
-- This table describes relationship between Test and Shift: In one shift there
-- are executed several tests
IF OBJECT_ID('TestShift', N'U') IS NOT NULL
	DROP TABLE TestShift;
CREATE TABLE TestShift (
	-- primary key
	Id INT IDENTITY(1,1),
	-- This table creates a relationshiop between shift and test output: test output referenced by
	-- TestId has been executed in shift referenced by ShiftId
	TestId INT NOT NULL,
	ShiftId INT NOT NULL,
	
	-- primary key Id
	CONSTRAINT pk_testShift_id PRIMARY KEY (Id),
	-- foreign key referencing test
	CONSTRAINT fk_testShift_testId FOREIGN KEY (TestId) REFERENCES Test(Id),
	-- foreign key referencing shift
	CONSTRAINT fk_testShift_shiftId FOREIGN KEY (ShiftId) REFERENCES Shift(Id),
	-- unique test, param and shift id
	CONSTRAINT uq_testShift_testId_shiftId UNIQUE(TestId, ShiftId)
);

-- Create index for foreign key referencing test
CREATE INDEX fk_testShift_testId ON TestShift(TestId);
-- Create index for foreign key referencing shift
CREATE INDEX fk_testShift_shiftId ON TestShift(ShiftId);
--#endregion

--#region CREATE TABLE TESTOUTPUT
-- Data generated by execution of a test in a particular shift
IF OBJECT_ID('TestOutput', N'U') IS NOT NULL
	DROP TABLE TestOutput;
CREATE TABLE TestOutput (
	-- primary key
	Id INT IDENTITY(1,1),
	-- Result of executed test. Application defines types of test result. At
	-- least could be: Correct, Failed, Aborted
	Result TINYINT DEFAULT(0) NOT NULL,
	-- Number of test sequence inside shift
	Sequence SMALLINT NOT NULL,
	-- Date and time when test has been started
	Start DATETIME NOT NULL,
	-- Date and time when test has been finished
	Finish DATETIME NOT NULL,
	-- Id of test which was used to produce output value on currernt row
	TestId INT NOT NULL,
	-- Id of shift where test output on current row was produced
	ShiftId INT NOT NULL,
	
	-- primary key Id
	CONSTRAINT pk_testOutput_id PRIMARY KEY(Id),
	-- foreign key referencing Id of test which was used to measure this output value
	CONSTRAINT fk_testOutput_testId FOREIGN KEY (TestId)
		REFERENCES Test(Id),
	-- foreign key referencing Id of shift where this test output was produced
	CONSTRAINT fk_testOutput_shiftId FOREIGN KEY (ShiftId) 
		REFERENCES Shift(Id)
);

--#region CREATE PROCEDURE udpDeleteTestOutput
IF OBJECT_ID('udpDeleteTestOutput') IS NOT NULL
	DROP PROCEDURE udpDeleteTestOutput;
GO
-- @testOutputId: Id of test output that should be deleted
CREATE PROCEDURE udpDeleteTestOutput(@testOutputId INT)
AS
BEGIN TRAN
	-- 1) delete all parameter outputs within deleting test output
	DELETE FROM ParamOutput
		WHERE TestOutputId = @testOutputId;
		
	-- 2) delete test output
	DELETE FROM TestOutput WHERE Id = @testOutputId;
COMMIT
GO
--#endregion

--#region CREATE PROCEDURE udpAddTestOutput
IF OBJECT_ID('udpAddTestOutput') IS NOT NULL
	DROP PROCEDURE udpAddTestOutput;
GO
CREATE PROCEDURE udpAddTestOutput(@result TINYINT, @sequence SMALLINT, @start DATETIME,
	@finish DATETIME, @testId INT, @shiftId INT)
AS
BEGIN TRAN
	INSERT INTO TestOutput(Result, Sequence, Start, Finish, TestId, ShiftId)
		VALUES(@result, @sequence, @start, @finish, @testId, @shiftId);
COMMIT
GO
--#endregion

-- Create index for foreign key referencing test
CREATE INDEX fk_testOutput_testId ON TestOutput(TestId);
-- Create index for foreign key referencing shift
CREATE INDEX fk_testOutput_shiftId ON TestOutput(ShiftId);
--#endregion

--#region CREATE TABLE PARAMOUTPUT
-- 
IF OBJECT_ID('ParamOutput', N'U') IS NOT NULL
	DROP TABLE ParamOutput;
CREATE TABLE ParamOutput (
	-- primary key
	Id	INT IDENTITY(1,1),
	-- Value of parameter output in string representation
	Value VARCHAR(50),
	-- Id of the parameter which was used to measure output value on current row
	ParamId INT NOT NULL,
	-- Id of test output to which parameter output on current row belongs to
	TestOutputId INT NOT NULL,
	
	-- primary key Id
	CONSTRAINT pk_paramOutput_id PRIMARY KEY (Id),
	-- foreign key referencing Id of parameter which was used to measure this
	-- output value
	CONSTRAINT fk_paramOutput_paramId FOREIGN KEY(ParamId) 
		REFERENCES Param(Id),
	-- foreign key referencing test output
	CONSTRAINT fk_paramOutput_testOutputId FOREIGN KEY (TestOutputId) 
		REFERENCES TestOutput(Id)
);

--#region CREATE PROCEDURE udpDeleteParamOutput
IF OBJECT_ID('udpDeleteParamOutput') IS NOT NULL
	DROP PROCEDURE udpDeleteParamOutput;
GO
-- Delete parameter output with given id
-- @paramOutputId: Id of parameter output that should be deleted
CREATE PROCEDURE udpDeleteParamOutput(@paramOutputId INT)
AS
BEGIN TRAN
	-- delete parameter output with given id
	DELETE FROM ParamOutput WHERE Id = @paramOutputId;
COMMIT
GO
--#endregion

--#region CREATE PROCEDURE udpAddParamOutput
IF OBJECT_ID('udpAddParamOutput') IS NOT NULL
	DROP PROCEDURE udpAddParamOutput;
GO
CREATE PROCEDURE udpAddParamOutput(@value VARCHAR(50), @paramId INT, @testOutputId INT)
AS
BEGIN TRAN
	INSERT INTO ParamOutput(Value, ParamId, TestOutputId)
		VALUES(@value, @paramId, @testOutputId);
COMMIT
GO
--#endregion

-- Create index for foreign key referencing parameter
CREATE INDEX fk_paramOutput_paramId ON ParamOutput(ParamId);
-- Create index for foreign key referencing test output
CREATE INDEX fk_paramOutput_testOutputId ON ParamOutput(TestOutputId);
--#endregion

---------------------------------------
--- CREATE PROCEDURES AND FUCNTIONS ---
---------------------------------------
		-- (ENTITY FRAMEWORK) --

--#region CREATE PROCEDURE udpParamResults
IF OBJECT_ID('udpParamResults', N'P') IS NOT NULL
	DROP PROCEDURE udpParamResults;
GO
-- @testId: Id of test output to which parameter output should be retrieved
CREATE PROCEDURE udpParamResults(@testId INT)
AS
BEGIN
	SELECT Param.Id, Param.Name, Param.Value, 
		Param.Type AS ValueType, Param.Unit, ParamOutput.Value AS OutputValue,
		Test.Name AS TestName
		FROM TestOutput 
		JOIN ParamOutput ON (TestOutput.Id = ParamOutput.TestOutputId)
		JOIN Param ON (ParamOutput.ParamId = Param.Id)
		JOIN Test ON (Test.Id = TestOutput.TestId)
		WHERE TestOutput.Id = @testId;
END
GO
--#endregion

--#region CREATE PROCEDURE udpTestResults
IF OBJECT_ID('udpTestResults', N'P') IS NOT NULL
	DROP PROCEDURE udpTestResults;
GO
CREATE PROCEDURE udpTestResults(@shiftId INT)
AS
BEGIN
	SELECT TestOutput.Id, Test.Name, TestOutput.Result, 
	CAST (TestOutput.Finish - TestOutput.Start AS TIME) AS Duration,
	TestOutput.Sequence
	FROM Shift 
		JOIN TestOutput ON (Shift.Id = TestOutput.ShiftId)
		JOIN Test ON (TestOutput.TestId = Test.Id)
		WHERE Shift.Id = @shiftId
	ORDER BY TestOutput.Sequence;
END
GO
--#endregion

--------------------
--- CREATE VIEWS ---
--------------------

--#region CREATE VIEW ShiftResult
-- For each shift get these information: Date and time when shift was started
-- and finished, name of tested mirror, full name of operator who has executed
-- that shift, number of sequences where all tests have been completed (correctly),
-- number of tests where at least one failed, and where at least one was aborted,
-- number of completed, failed and aborted tests. Notice that aborted test is failed
-- as well
IF OBJECT_ID('ShiftResult', N'V') IS NOT NULL
	DROP VIEW ShiftResult;
GO
-- ShiftId, TotalSequences, CompletedSequences, FailedSequences, AbortedSequences, TotalTests,
-- CompletedTests, FailedTests, AbortedTests, Start, Finish, MirrorId, Mirror, OperatorId, Operator
CREATE VIEW ShiftResult AS
	SELECT t2.ShiftId as Id, t2.TotalSequences, t2.CompletedSequences, t2.FailedSequences, t2.AbortedSequences,
		t2.TotalTests, t2.CompletedTests, t2.FailedTests, t2.AbortedTests,
		Shift.Start, Shift.Finish, Mirror.Id AS MirrorId, Mirror.Name AS Mirror, 
		Operator.Id AS OperatorId, Operator.Name + ' ' + Operator.Surname AS Operator
	FROM
	(SELECT ShiftId,
		-- test outputs are grouped by sequences - count number of sequences in each shift
		COUNT(*) AS TotalSequences,
		-- count number of sequences where all tests are completed
		SUM(CASE WHEN Failed = 0 AND Aborted = 0 THEN 1 ELSE 0 END) AS CompletedSequences,
		-- count number of sequences where at least one test is failed or aborted
		SUM(CASE WHEN Failed > 0 OR Aborted > 0 THEN 1 ELSE 0 END) AS FailedSequences,
		-- count number of sequence where at least one test is aborted
		SUM(CASE WHEN Aborted > 0 THEN 1 ELSE 0 END) AS AbortedSequences,
		-- sum all tests in all sequences
		SUM(TotalTests) AS TotalTests,
		-- sum all completed tests in all sequences
		SUM(Completed) AS CompletedTests,
		-- sum all failed tests in all sequences
		SUM(Failed) AS FailedTests,
		-- sum all aborted tests in all sequences
		SUM(Aborted) AS AbortedTests
	FROM
		-- group tests in shift by sequence number and in each sequence count nubmer of completed,
		-- failed and aborted tests (sequence where are only completed tests is completed, otherwise
		-- it is failed and can be aborted if thare is at least one aborted test)
		(SELECT ShiftId, Sequence,
			-- count total number of tests
			COUNT(*) TotalTests,
			-- for each completed test add 1
			SUM(CASE WHEN Result = 0 THEN 1 ELSE 0 END) AS Completed,
			-- for each failed (or aborted) test add 1
			SUM(CASE WHEN Result >= 1 THEN 1 ELSE 0 END) AS Failed,
			-- for each aborted test add 1
			SUM(CASE WHEN Result >= 2 THEN 1 ELSE 0 END) AS Aborted
		FROM TestOutput
		-- create groups of sequences for each shift
		GROUP BY TestOutput.ShiftId, TestOutput.Sequence) t1
		-- group sequences to one shift
		GROUP BY t1.ShiftId) t2
	-- add shift data: start, finish
	JOIN Shift ON (Shift.Id = t2.ShiftId)
	-- add mirror data: name of mirror
	JOIN Mirror ON (Shift.MirrorId = Mirror.Id)
	-- add name opeartor who executed this shift
	JOIN Operator ON (Shift.OperatorId = Operator.Id)
GO
--#endregion

--#region CREATE VIEW TestResult
IF OBJECT_ID('TestResult', N'V') IS NOT NULL
	DROP VIEW TestResult;
GO
CREATE VIEW TestResult(Id, Name, Result, Duration, Sequence) AS
	SELECT TestOutput.Id, Test.Name, TestOutput.Result, 
	CAST (TestOutput.Finish - TestOutput.Start AS TIME),
	TestOutput.Sequence
	FROM Shift 
		JOIN TestOutput ON (Shift.Id = TestOutput.ShiftId)
		JOIN Test ON (TestOutput.TestId = Test.Id) 
GO
--#endregion

--#region CREATE VIEW MirrorResult
IF OBJECT_ID('MirrorResult', N'V') IS NOT NULL
	DROP VIEW MirrorResult;
GO
CREATE VIEW MirrorResult(Id, Name, Sequences, Completed, Failed, Aborted, AverageDuration) AS
	SELECT Mirror.Id, Mirror.Name, t.Sequences, t.Completed, t.Failed, t.Aborted, t.AverageDuration FROM
	(SELECT MirrorId, COUNT(*) AS Sequences, SUM(CompletedSequences) AS Completed, 
		SUM(FailedSequences) AS Failed,
		SUM(AbortedSequences) AS Aborted, 
		CAST(CAST(AVG(CAST(Finish - Start AS FLOAT)) AS DATETIME) AS TIME) AS AverageDuration
		FROM ShiftResult
		GROUP BY MirrorId) t
	JOIN Mirror ON (t.MirrorId = Mirror.Id)
GO
--#endregion

--#region CREATE VIEW OperatorResult

IF OBJECT_ID('OperatorResult', N'V') IS NOT NULL
	DROP VIEW OperatorResult;
GO
CREATE VIEW OperatorResult(Id, Name, Surname, TotalSequences, TotalTests, TotalTime) AS
	SELECT Operator.Id, Operator.Name, Operator.Surname, ISNULL(TotalSequences, 0),
		ISNULL(TotalMirrors, 0), ISNULL(TotalTime, '00:00:00')
	FROM
	(SELECT Operator.Id AS OperatorId, COUNT(*) AS TotalMirrors,
		CAST(CAST(SUM(CAST(TestOutput.Finish - TestOutput.Start AS FLOAT)) AS DATETIME) AS TIME) AS TotalTime
		FROM Operator 
		JOIN Shift ON (Operator.Id = Shift.OperatorId)
		JOIN TestOutput ON (Shift.Id = TestOutput.ShiftId)
		GROUP BY Operator.Id) t1
	JOIN 
	(SELECT OperatorId, COUNT(*) AS TotalSequences
		FROM Shift
		GROUP BY OperatorId) t2
	ON(t1.OperatorId = t2.OperatorId)
	RIGHT JOIN Operator ON (t2.OperatorId = Operator.Id)
GO
--#endregion

--#region CREATE VIEW TestRate
IF OBJECT_ID('TestRate', N'V') IS NOT NULL
	DROP VIEW TestRate;
GO
-- List of all tests used for any testing with statistics:
-- Id (of test), Name (of test), Total (numer of test executions), Completed (number of completed
-- executions), Failed (number of failed executions), Aborted (number of aborted executions), 
-- TotalTime (sum of all executions duration)
CREATE VIEW TestRate AS
	SELECT t1.Id, Test.Name, t1.Total, t1.Completed, t1.Failed, t1.Aborted, t1.TotalTime FROM
	(SELECT Test.Id,
		-- test outputs are grouped by used test id, count number of executions of this test
		COUNT(*) AS Total,
		-- count number of completed executions
		SUM(CASE WHEN Result = 0 THEN 1 ELSE 0 END) AS Completed,
		-- count number of failed (or aborted) executions
		SUM(CASE WHEN Result >= 1 THEN 1 ELSE 0 END) AS Failed,
		-- count number of aborted executinos
		SUM(CASE WHEN Result >= 2 THEN 1 ELSE 0 END) AS Aborted,
		-- sum durations of all tests
		CAST(CAST(SUM(CAST(Finish - Start AS FLOAT)) AS DATETIME) AS TIME) AS TotalTime
	 FROM
		TestOutput
		JOIN Test ON (TestOutput.TestId = Test.Id)
		GROUP BY Test.Id) t1
	-- add test data: name
	JOIN Test ON (t1.Id = Test.Id)
GO
--#endregion

--#region CREATE VIEW MirrorRate
IF OBJECT_ID('MirrorRate', N'V') IS NOT NULL
	DROP VIEW MirrorRate;
GO
-- List of all mirrors and statisticks about tests executed on this type of mirror:
-- Id, SerialNumber, Name, Description, Type, SupplierId, Total, Completed, Failed, Aborted
CREATE VIEW MirrorRate AS
	SELECT Mirror.*, t.Total, t.Completed, t.Failed, t.Aborted FROM
	(SELECT Mirror.Id,
		COUNT(*) AS Total,
		SUM(CASE WHEN Result = 0 THEN 1 ELSE 0 END) AS Completed,
		SUM(CASE WHEN Result >= 1 THEN 1 ELSE 0 END) AS Failed,
		SUM(CASE WHEN Result >= 2 THEN 1 ELSE 0 END) AS Aborted
	FROM 
		Shift
		JOIN
		TestOutput ON (TestOutput.ShiftId = Shift.Id)
		RIGHT JOIN	-- include mirror that have not been tested yet
		Mirror ON (Shift.MirrorId = Mirror.Id)
		GROUP BY Mirror.Id) t
	JOIN
	-- to result add info about particular mirror
	Mirror ON (Mirror.Id = t.Id)
GO
--#endregion

-- PARAMETRIZED VIEWS --
-- These views are auxiliary only. Allows to create more specific views that will be used by an
-- application. For example: function is parametrized by a range of dates and produce some statistics. 
-- But in application we only want to have annual statistics. Create a function (or porcedure if you are
-- using Entity Framework) that will pass to this function a range of current year. Advantage of this is
-- that a new procedure for month statistics (for example) can be created easily.

--#region CREATE FUNCTION udfGetStat

IF OBJECT_ID('udfGetShiftStat') IS NOT NULL
	DROP FUNCTION udfGetShiftStat;
GO
CREATE FUNCTION udfGetShiftStat(@begin DATETIME, @end DATETIME)
RETURNS TABLE
AS
	RETURN (SELECT * FROM ShiftResult 
		WHERE Start >= @begin AND Finish <= @end);
GO
--#endregion

--#region CREATE FUNCTION udfBestOperators

IF OBJECT_ID('udfBestOperators') IS NOT NULL
	DROP FUNCTION udfBestOperators;
GO
-- Get @count of best operators during given period of time (@begin - @end)
-- Operators are evaluated by sum of duration of all tests executed in a shift by this operator
CREATE FUNCTION udfBestOperators(@count INT, @begin DATETIME, @end DATETIME)
RETURNS TABLE
AS
	RETURN 
	(SELECT TOP (@count)
		Operator.Id, Operator.Name, Operator.Surname, ISNULL(TotalSequences, 0) AS TotalSequences,
		ISNULL(TotalTests, 0) AS TotalTests, ISNULL(TotalTestTime, '00:00:00') AS TotalTestTime, 
		ISNULL(TotalShiftTime, '00:00:00') AS TotalShiftTime
	FROM
	(SELECT Operator.Id AS OperatorId, COUNT(*) AS TotalTests,
		-- count operator testing time
		CAST(CAST(SUM(CAST(TestOutput.Finish - TestOutput.Start AS FLOAT)) AS DATETIME) AS TIME) AS TotalTestTime
		FROM Operator 
		JOIN Shift ON (Operator.Id = Shift.OperatorId)
		JOIN TestOutput ON (Shift.Id = TestOutput.ShiftId)
		-- only count shift in given time range
		WHERE Shift.Start >= @begin AND Shift.Finish <= @end
		GROUP BY Operator.Id) t1
	JOIN 
	-- for each operator count number of his or her serquences
	(SELECT OperatorId, COUNT(*) AS TotalSequences, 
		CAST(CAST(SUM(CAST(Shift.Finish - Shift.Start AS FLOAT)) AS DATETIME) AS TIME) AS TotalShiftTime
		FROM Shift
		-- only count shift in given range
		WHERE Shift.Start >= @begin AND Shift.Finish <= @end
		GROUP BY OperatorId) t2
	ON(t1.OperatorId = t2.OperatorId)
	RIGHT JOIN Operator ON (t2.OperatorId = Operator.Id)
	ORDER BY TotalTestTime DESC, TotalShiftTime DESC);
GO
--#endregion
