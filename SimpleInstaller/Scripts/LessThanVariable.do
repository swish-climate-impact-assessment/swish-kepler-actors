set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName %LeftVariableName%
// define variableName %RightVariableName%
// define variableName optional %ResultVariableName% Working
// define double %Value%
clear
use "%Input%"
 generate double variable189996587 = %LeftVariableName% < %RightVariableName%
capture confirm variable %ResultVariableName%
if (_rc == 0){
	drop %ResultVariableName%
}
rename variable189996587 %ResultVariableName%
save "%Output%"
