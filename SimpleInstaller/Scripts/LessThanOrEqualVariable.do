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
 generate double variable1847210181 = %LeftVariableName% <= %RightVariableName%
capture confirm variable %ResultVariableName%
if (_rc == 0){
	drop %ResultVariableName%
}
rename variable1847210181 %ResultVariableName%
save "%Output%"
