set more off
set output proc
// define variableName optional %ResultVariableName% Working
// define output %Output%
// define date %StartDate%
// define date %EndDate%
 generate variable1310531161 = 1
capture confirm variable %ResultVariableName%
if (_rc == 0){
	drop %ResultVariableName%
}
rename variable1310531161 %ResultVariableName%
format %ResultVariableName% %td
local variable1310531162 = _N + 1
set obs `variable1310531162'
replace %ResultVariableName% = date("%StartDate%", "DMY") if missing(%ResultVariableName%)
local variable1310531163 = _N + 1
set obs `variable1310531163'
replace %ResultVariableName% = date("%EndDate%", "DMY") if missing(%ResultVariableName%)
tsset %ResultVariableName%
tsfill
save "%Output%"
