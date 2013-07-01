set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName %VariableName%
// define variableName optional %ResultVariableName% Working
// define double %Value%
clear
use "%Input%"
 generate double variable1787046720 = %VariableName% * %Value%
capture confirm variable %ResultVariableName%
if (_rc == 0){
	drop %ResultVariableName%
}
rename variable1787046720 %ResultVariableName%
save "%Output%"
