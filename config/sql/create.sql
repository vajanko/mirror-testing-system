-------------------------------------------------------------------------------
-- CREATE SCRIPT FOR MTS DATABASE

-- AUTHOR: ONDREJ KOVAC
-- DATE: 14.12.2011

-------------------------------------------------------------------------------

USE mts;

---------------------
--- CREATE TALBES ---
---------------------

--#region CREATE SUPPLIER TABLE
CREATE TABLE Supplier (
	Id INT IDENTITY(1,1),
	-- Name of supplier company
	CompanyName NVARCHAR(50) NOT NULL,
	-- following items are address supplier company residence
	-- there may be also more addresses of supplier
	City NVARCHAR(20),
	Street NVARCHAR(20),
	Number INT,
	Zip INT,
	-- state could be reference to table of all states, but depends on how
	-- smart application should be
	State NVARCHAR(30),
	
	CONSTRAINT pk_supplier_id PRIMARY KEY (Id),
	CONSTRAINT uk_supplier_companyName UNIQUE(CompanyName)
);
--#endregion

--#region CREATE MIRROR TABLE
CREATE TABLE Mirror (
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
--#endregion

--#region CREATE OPERATOR TABLE
-- In this table are stored users that are executing shifts and tests
CREATE TABLE Operator (
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
--#endregion

--#region CREATE TEST TABLE
-- In this table are stored any tests that has been used in some shift. By now 
-- test has no other properties (only Name), so probalbly this table will not 
-- change very often (like Param)
CREATE TABLE Test (
	Id INT IDENTITY (1,1),
	-- Name of test (in application this is its Id - must be unique)
	Name VARCHAR(25) NOT NULL,
	
	-- primary key Id
	CONSTRAINT pk_test_id PRIMARY KEY (Id),
	-- unique Name of test
	CONSTRAINT uq_test_name UNIQUE (Name)
);
--#endregion

--#region CREATE PARAM TABLE
-- In this table are stored any parameters that has been used in some test.
CREATE TABLE Param (
	Id INT IDENTITY (1,1),
	-- Name of parameter (this value is unique inside particular test)
	Name VARCHAR(25) NOT NULL,
	-- value of parameter in string representation
	Value VARCHAR(50) NOT NULL,
	-- type of value data (int, string, bool, ...)
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
--#endregion

--#region CREATE TABLE TESTPARAM
-- This table describes relationship between Test and Param. In one test there
-- are multiple parameters and parameter may be contained in multiple tests
CREATE TABLE TestParam (
	Id INT IDENTITY(1,1),
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

--#region CREATE SHIFT TABLE
CREATE TABLE Shift (
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
-- Create index for foreign key referencing mirror
CREATE INDEX fk_shift_mirrorId ON Shift(MirrorId);
-- Create index for foreign key referencing operator
CREATE INDEX fk_shift_operatorId ON Shift(OperatorId);
--#endregion

--#region CREATE TESTSHIFT TABLE
-- This table describes relationship between Test and Shift: In one shift there
-- are executed several tests
CREATE TABLE TestShift (
	Id INT IDENTITY(1,1),
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

--#region CREATE TESTOUTPUT TABLE
CREATE TABLE TestOutput (
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
	--
	ShiftId INT NOT NULL,
	
	-- primary key Id
	CONSTRAINT pk_testOutput_id PRIMARY KEY(Id),
	-- foreign key referencing Id of test which was used to measure this
	-- output value
	CONSTRAINT fk_testOutput_testId FOREIGN KEY (TestId)
		REFERENCES Test(Id),
	-- foreign key referencing Id of shift where this test output was produced
	CONSTRAINT fk_testOutput_shiftId FOREIGN KEY (ShiftId) 
		REFERENCES Shift(Id)
);

-- Create index for foreign key referencing test
CREATE INDEX fk_testOutput_testId ON TestOutput(TestId);
-- Create index for foreign key referencing shift
CREATE INDEX fk_testOutput_shiftId ON TestOutput(ShiftId);
--#endregion

--#region CREATE PARAMOUTPUT TABLE
CREATE TABLE ParamOutput (
	Id	INT IDENTITY(1,1),
	-- Value of parameter output in string representation
	Value VARCHAR(50),
	-- Id of the parameter which was used to measure output value on current
	-- row
	ParamId INT NOT NULL,
	--
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

-- Create index for foreign key referencing parameter
CREATE INDEX fk_paramOutput_paramId ON ParamOutput(ParamId);
-- Create index for foreign key referencing test output
CREATE INDEX fk_paramOutput_testOutputId ON ParamOutput(TestOutputId);
--#endregion

---------------------------------------
--- CREATE PROCEDURES AND FUCNTIONS ---
---------------------------------------

  -- DELETE --

--#region CREATE PROCEDURE udpDeleteParamOutput
IF OBJECT_ID('udpDeleteParamOutput') IS NOT NULL
	DROP PROCEDURE udpDeleteParamOutput;
GO
-- @paramOutputId: Id of parameter output that should be deleted
CREATE PROCEDURE udpDeleteParamOutput(@paramOutputId INT)
AS
BEGIN
	-- delete parameter output with given id
	DELETE FROM ParamOutput WHERE Id = @paramOutputId;
END
GO
--#endregion

--#region CREATE PROCEDURE udpDeleteTestOutput
IF OBJECT_ID('udpDeleteTestOutput') IS NOT NULL
	DROP PROCEDURE udpDeleteTestOutput;
GO
-- @testOutputId: Id of test output that should be deleted
CREATE PROCEDURE udpDeleteTestOutput(@testOutputId INT)
AS
BEGIN
	-- first delete all parameters of this test output
	DELETE FROM ParamOutput WHERE TestOutputId = @testOutputId;
	-- now it is safe to delete this test output because nothing is referencing it
	DELETE FROM TestOutput WHERE Id = @testOutputId;	
END
GO
--#endregion

--#region CREATE PROCEDURE udpDeleteShift
IF OBJECT_ID('udpDeleteShift') IS NOT NULL
	DROP PROCEDURE udpDeleteShift;
GO
-- @shiftId: Id of shift that should be deleted
CREATE PROCEDURE udpDeleteShift(@shiftId INT)
AS
BEGIN
	-- 1) delete all parameters that are in some test that was executed in shift
	-- that should be deleted
	DELETE PO FROM ParamOutput PO
		INNER JOIN TestOutput ON (PO.TestOutputId = TestOutput.Id)
		WHERE TestOutput.ShiftId = @shiftId;
	-- 2) delete all tests executed in this shift
	DELETE FROM TestOutput WHERE ShiftId = @shiftId;
	-- 3) delete all references to test (parameters) that have been used in this shift
	DELETE FROM TestShift WHERE ShiftId = @shiftId;	
	-- now it is save to delete this shift because nothing is referencing it
	DELETE FROM Shift WHERE Id = @shiftId;
END
GO
--#endregion

--#region CREATE PROCEDURE udpDeleteOperator
IF OBJECT_ID('udpDeleteOperator') IS NOT NULL
	DROP PROCEDURE udpDeleteOperator;
GO
CREATE PROCEDURE udpDeleteOperator(@operatorId INT)
AS
BEGIN
	-- 1) delete all parameters which have been executed in some test which has been
	-- executed in a shift which has executed operator that should be deleted
	DELETE P FROM ParamOutput P
		INNER JOIN TestOutput ON (P.TestOutputId = TestOutput.Id)
		INNER JOIN Shift ON (TestOutput.ShiftId = Shift.Id)
		WHERE Shift.OperatorId = @operatorId;

	-- 2) delete all tests that have been executed in some shift which has beed executed
	-- by operator that should be deleted
	DELETE T FROM TestOutput T
		INNER JOIN Shift ON (T.ShiftId = Shift.Id)
		WHERE Shift.OperatorId = @operatorId;
	
	-- 3) delete all references to test which has been used in a shift which has been executed
	-- by operator that should be deleted
	DELETE TS FROM TestShift TS
		INNER JOIN Shift ON (TS.ShiftId = Shift.Id)
		WHERE Shift.OperatorId = @operatorId;
	
	-- 4) delete all shifts which have been executed by operator that should be deleted
	DELETE FROM Shift WHERE Shift.OperatorId = @operatorId;
	
	-- 5) delete operator
	DELETE FROM Operator WHERE Operator.Id = @operatorId;
END
GO		
--#endregion

	-- SELECT (ENTITY FRAMEWORK) --

--#region CREATE PROCEDURE udpParamResults
IF OBJECT_ID('udpParamResults') IS NOT NULL
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
IF OBJECT_ID('udpTestResults') IS NOT NULL
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

  -- ADD OR MODIFY --
  
--#region CREATE PROCEDURE udpAddTest

IF OBJECT_ID('udpAddTest') IS NOT NULL
	DROP PROCEDURE udpAddTest;
-- Add a new test to Test table (if it does not exists) and return inserted or already
-- existing value
GO
CREATE PROCEDURE udpAddTest(@name VARCHAR(25))
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
	
	-- now select added or already existing test. This strange behaviour is required by
	-- entity framework - this value will be returned as output
	SELECT TOP 1 * FROM Test
		WHERE (Name = @name);
COMMIT TRAN
GO
--#endregion  
  
--#region CREATE PROCEDURE udpAddParam
IF OBJECT_ID('udpAddParam') IS NOT NULL
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
	SET @count = (SELECT COUNT(*) FROM Param 
		WHERE Name = @name AND Value = @value AND Type = @type AND (Unit IS NULL OR Unit = @unit));
	PRINT @count;
		
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

	-- PARAMETRIZED VIEWS --

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
		Operator.Id, Operator.Name, Operator.Surname, TotalSequences, TotalTests, 
		TotalTestTime, TotalShiftTime
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
	JOIN Operator ON (t2.OperatorId = Operator.Id)
	ORDER BY TotalTestTime, TotalShiftTime DESC);
GO
--#endregion

--#region CREATE FUNCTION udf

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
IF OBJECT_ID('ShiftResult') IS NOT NULL
	DROP VIEW ShiftResult;
GO
-- ShiftId, TotalSequences, CompletedSequences, FailedSequences, AbortedSequences, TotalTests,
-- CompletedTests, FailedTests, AbortedTests, Start, Finish, MirrorId, Mirror, OperatorId, Operator
CREATE VIEW ShiftResult AS
	SELECT t2.*, Shift.Start, Shift.Finish, Mirror.Id AS MirrorId, Mirror.Name AS Mirror, 
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
IF OBJECT_ID('TestResult') IS NOT NULL
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
IF OBJECT_ID('MirrorResult') IS NOT NULL
	DROP VIEW MirrorResult;
GO
CREATE VIEW MirrorResult(Id, Name, Sequences, Completed, Failed, Aborted, AverageDuration) AS
	SELECT Mirror.Id, Mirror.Name, t.Sequences, t.Completed, t.Failed, t.Aborted, t.AverageDuration FROM
	(SELECT MirrorId, COUNT(*) AS Sequences, SUM(Completed) AS Completed, SUM(Failed) AS Failed,
		SUM(Aborted) AS Aborted, 
		CAST(CAST(AVG(CAST(Finish - Start AS FLOAT)) AS DATETIME) AS TIME) AS AverageDuration
		FROM ShiftResult
		GROUP BY MirrorId) t
	JOIN Mirror ON (t.MirrorId = Mirror.Id)
GO
--#endregion

--#region CREATE VIEW OperatorResult
IF OBJECT_ID('OperatorResult') IS NOT NULL
	DROP VIEW OperatorResult;
GO
CREATE VIEW OperatorResult(Id, Name, Surname, TotalSequences, TotalTests, TotalTime) AS
	SELECT Operator.Id, Operator.Name, Operator.Surname, TotalSequences, TotalMirrors, TotalTime
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
	JOIN Operator ON (t2.OperatorId = Operator.Id)
GO
SELECT * FROM OperatorResult ORDER BY TotalSequences DESC
--#endregion

--#region CREATE VIEW TestRate
IF OBJECT_ID('TestRate') IS NOT NULL
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
IF OBJECT_ID('MirrorRate') IS NOT NULL
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
select * from mirrorrate;
--#endregion

-----------------------
--- CREATE TRIGGERS ---
-----------------------

--#region CREATE TRIGGER trParamOutput_del
IF OBJECT_ID('trParamOutput_del') IS NOT NULL
	DROP TRIGGER trParamOutput_del;
GO
-- after some parameter outputs have been deleted check if there are any unused parameters
-- which have been referenced by these delted outputs
CREATE TRIGGER trParamOutput_del ON ParamOutput
-- this trigger will be fired after param outputs have been deleted so we are shure that Params
-- won't be referenced
AFTER DELETE
AS
BEGIN TRAN
	-- delete all parameters
	DELETE FROM Param
		-- that are not referenced by some parameter output just deleted
		WHERE NOT EXISTS (SELECT * FROM deleted
			WHERE Param.Id = deleted.ParamId)
COMMIT
GO
--#endregion

--#region CREATE TRIGGER trTestOutput_del
IF OBJECT_ID('trTestOutput_del') IS NOT NULL
	DROP TRIGGER trTestOutput_del;
GO
-- after some test outputs have been deleted check if there are any unused tests
-- which have been referenced by these delted outputs
CREATE TRIGGER trTestOutput_del ON TestOutput
-- this trigger will be fired after test outputs have been deleted so we are shure that Tests
-- won't be referenced
AFTER DELETE
AS
BEGIN TRAN
	-- delete all tests
	DELETE FROM Test
		-- that are not referenced by some test output just deleted
		WHERE NOT EXISTS (SELECT * FROM deleted
			WHERE Test.Id = deleted.TestId)
COMMIT
GO
--#endregion