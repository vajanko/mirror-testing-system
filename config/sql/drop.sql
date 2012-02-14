-------------------------------------------------------------------------------
-- DROP SCRIPT FOR MTS DATABASE

-- AUTHOR: ONDREJ KOVAC
-- DATE: 17.12.2011

-------------------------------------------------------------------------------

--USE mts;

-- DROP ALL PROCEDURES
-- param output
DROP PROCEDURE udpDeleteParamOutput;
DROP PROCEDURE udpAddParamOutput;
DROP PROCEDURE udpParamResults;
-- test output
DROP PROCEDURE udpDeleteTestOutput;
DROP PROCEDURE udpAddTestOutput;
DROP PROCEDURE udpTestResults;
-- shift
DROP PROCEDURE udpStartShift;
DROP PROCEDURE udpFinishShift;
DROP PROCEDURE udpDeleteShift;
-- param
DROP PROCEDURE udpAddParam;
-- test
DROP PROCEDURE udpAddTest;
-- operator
DROP PROCEDURE udpAddOperator;
DROP PROCEDURE udpDeleteOperator;
-- supplier
DROP PROCEDURE udpAddSupplier;
DROP PROCEDURE udpDeleteSupplier;
-- mirror
DROP PROCEDURE udpAddMirror;
DROP PROCEDURE udpDeleteMirror;

-- DROP ALL FUNCTIONS
DROP FUNCTION udfBestOperators;
DROP FUNCTION udfGetShiftStat;


-- DROP ALL VIEWS
DROP VIEW ShiftResult;
DROP VIEW TestResult;
DROP VIEW MirrorResult;
DROP VIEW OperatorResult;
DROP VIEW MirrorRate;
DROP VIEW TestRate;

-- DROP ALL TABLES
DROP TABLE ParamOutput;		-- referenging: Param, Shift
DROP TABLE TestOutput;		-- referencing: Test, Shift
DROP TABLE TestShift;		-- referencing: Test, Shift
DROP TABLE TestParam;		-- referencing: Test, Param

DROP TABLE Param;			--

DROP TABLE Test;			--

DROP TABLE Shift;			-- referencing: Mirror, Operator
DROP TABLE Operator;		--
DROP TABLE Mirror;			-- referencing Supplier

DROP TABLE Supplier;		--
-------------------------------------------------------------------------------