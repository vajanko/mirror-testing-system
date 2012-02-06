-------------------------------------------------------------------------------
-- DROP SCRIPT FOR MTS DATABASE

-- AUTHOR: ONDREJ KOVAC
-- DATE: 17.12.2011

-------------------------------------------------------------------------------

USE mts;

-- DROP ALL PROCEDURES
DROP PROCEDURE udpDeleteParamOutput;
DROP PROCEDURE udpDeleteTestOutput;
DROP PROCEDURE udpDeleteShift;
DROP PROCEDURE udpParamResults;
DROP PROCEDURE udpTestResults;
DROP PROCEDURE udpDeleteOperator;


-- DROP ALL VIEWS
DROP VIEW ShiftResult;
DROP VIEW TestResult;
DROP VIEW MirrorResult;
DROP VIEW OperatorResult;

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