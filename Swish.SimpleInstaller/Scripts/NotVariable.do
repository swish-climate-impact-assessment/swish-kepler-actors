set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName %VariableName%
// define variableName optional %ResultVariableName% Working
clear
use "%Input%"
 generate double variable1888716318 = !%VariableName%
capture confirm variable %ResultVariableName%
if (_rc == 0){
	drop %ResultVariableName%
}
rename variable1888716318 %ResultVariableName%
save "%Output%"
