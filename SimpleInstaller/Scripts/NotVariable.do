set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName %VariableName%
// define variableName optional %ResultVariableName% Working
clear
use "%Input%"
 generate double variable1847210186 = !%VariableName%
capture confirm variable %ResultVariableName%
if (_rc == 0){
	drop %ResultVariableName%
}
rename variable1847210186 %ResultVariableName%
save "%Output%"
