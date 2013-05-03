set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName %Variable%
// define variableName optional %ResultVariableName% Working
// define double %Value%
clear
use "%Input%"
 generate double variable1689087515 = %Variable% < %Value%
capture confirm variable %ResultVariableName%
if (_rc == 0){
	drop %ResultVariableName%
}
rename variable1689087515 %ResultVariableName%
save "%Output%"
