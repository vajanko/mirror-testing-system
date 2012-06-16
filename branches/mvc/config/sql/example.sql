-------------------------------------------------------------------------------
-- SAMPLE SCRIPT FOR MTS DATABASE

-- AUTHOR: ONDREJ KOVAC
-- DATE: 13.2.2011

-------------------------------------------------------------------------------

--USE mts;
BEGIN TRAN


-- 1) Add a new operator who will execute shifts. Password is transformed with SHA2.
-- Operator type is admin (0)
EXEC udpAddOperator N'Ondrej', N'Kovac', 'kovaco', 
	'6e1b7b1f2b56396f6093f6ef9f7d62551b6a98734f2b4a18b03e7645c4a9a97e', 0;
	-- see added operator
SELECT * FROM Operator;

-- 2) See how much operators are working
SELECT * FROM OperatorResult;

-- 3) Lets executed some shift (take first mirror type)
DECLARE @mirrorId INT;
SET @mirrorId = (SELECT TOP 1 Id FROM Mirror);
DECLARE @operatorId INT;
SET @operatorId = (SELECT TOP 1 Id FROM Operator WHERE Login = 'kovaco');	
DECLARE @now DATETIME;
SET @now = GETDATE();
-- created shift is returned as a result when using Entity Framework
EXEC udpStartShift @now, @mirrorId, @operatorId;
DECLARE @shiftId INT;
SET @shiftId = @@IDENTITY;

-- 4) We are going to used these tests in already created shift (all these tests already have parameters
-- defined)
EXEC udpAddTest 'DirectionLight', @shiftId;
EXEC udpAddTest 'Powerfold', @shiftId;
EXEC udpAddTest 'TravelNorth', @shiftId;

-- 5) Lets execute them
DECLARE @testId1 INT;
SET @testId1 = (SELECT TOP 1 Id FROM Test WHERE Name = 'DirectionLight');
DECLARE @testId2 INT;
SET @testId2 = (SELECT TOP 1 Id FROM Test WHERE Name = 'Powerfold');
DECLARE @testId3 INT;
SET @testId3 = (SELECT TOP 1 Id FROM Test WHERE Name = 'TravelNorth');

DECLARE @i INT;		-- sequence number
SET @i = 1;

WHILE @i < 5
	BEGIN
		SET @now = GETDATE();
		EXEC udpAddTestOutput 0, @i, @now, @now, @testId1, @shiftId;
		EXEC udpAddTestOutput 2, @i, @now, @now, @testId2, @shiftId;
		EXEC udpAddTestOutput 0, @i, @now, @now, @testId3, @shiftId;
		SET @i = @i + 1;
	END;

-- finish shift
SET @now = GETDATE();
EXEC udpFinishShift @shiftId, @now;

-- 6) See executed shifts
SELECT * FROM ShiftResult;

-- 7) Take a look at a specific shift
DECLARE @shift INT;
SET @shift = (SELECT TOP 1 Id FROM Shift);
EXEC udpTestResults @shift;

-- 8) Take a look at specific test
DECLARE @test INT;
SET @test = (SELECT TOP 1 Id FROM Test);
EXEC udpParamResults @test;

-- 9) See which mirror types are more defective
SELECT * FROM MirrorRate ORDER BY Failed DESC;

-- 10) See results of mirrors testing (statistics)
SELECT * FROM MirrorResult ORDER BY AverageDuration DESC;

-- 11) See which tests are less defective
SELECT * FROM TestRate ORDER BY Failed DESC;
-- or which mainly caused shift to be aborted
SELECT * FROM TestRate WHERE Aborted > 0 ORDER BY Aborted DESC;
-- Which tests takes most time of execution (this is important as it could tell us which
-- test should be improved)
SELECT Name, 
	CAST (CAST (CASE WHEN Completed = 0 THEN 0 ELSE CAST(CAST(TotalTime AS DATETIME) AS FLOAT) / Completed END AS DATETIME) AS TIME) AS AvgTime
	FROM TestRate ORDER BY AvgTime DESC;
	
-- 12) Select 3 most working operators for last year
SELECT * FROM udfBestOperators(3, DATEADD(YEAR, -1, GETDATE()), GETDATE());

-- 13) Get shift statistics for last year
SELECT * FROM udfGetShiftStat(DATEADD(YEAR, -1, GETDATE()), GETDATE());

-- 14) This operator is not working very good. Fire him (delete all his or her data)
SET @operatorId = (SELECT TOP 1 Id FROM Operator WHERE Login = 'kovaco');	
EXEC udpDeleteOperator @operatorId;

-- 15) Release contract with first supplier. (Economic crisis is very hard)
DECLARE @supplier INT;
SET @supplier = (SELECT TOP 1 Id FROM Supplier);
EXEC udpDeleteSupplier @supplier;

-- 16) These data are important for parallel test scheduling
SELECT Name, Total, TotalTime  FROM TestRate

ROLLBACK

