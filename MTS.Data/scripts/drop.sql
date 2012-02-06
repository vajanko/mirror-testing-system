-------------------------------------------------------------------------------
-- DROP SCRIPT FOR MTS DATABASE

-- AUTHOR: ONDREJ KOVAC
-- DATE: 17.12.2011

-------------------------------------------------------------------------------

USE mts;

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