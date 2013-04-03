set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName %VariableName%
// define variableName %NewVariableName%
clear
use "%Input%"
capture confirm variable %NewVariableName%
if (_rc == 0){
	drop %NewVariableName%
}
rename %VariableName% %NewVariableName%
save "%Output%"
