set more off
set output proc
// define input %Input1%
// define input %Input2%
// define output %Output%
// define variableNames %VariableNames%
// define temporaryFile %TemporaryFile%
// define variableName %FillVariable%
clear
use "%Input2%"
sort %VariableNames%, stable
save "%TemporaryFile%"
clear
clear
use "%Input1%"
sort %VariableNames%, stable
merge %VariableNames%, using "%TemporaryFile%"
sort %VariableNames%, stable
replace %FillVariable%=0 if missing(%FillVariable%)
capture confirm variable _merge
if (_rc == 0){
	drop _merge
}
save "%Output%"
