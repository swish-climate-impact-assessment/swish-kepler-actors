set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName %LeftVariableName%
// define variableName %RightVariableName%
// define variableName optional %ResultVariableName% Working
clear
use "%Input%"
 generate double variable1031206661 = %LeftVariableName% & %RightVariableName%
capture confirm variable %ResultVariableName%
if (_rc == 0){
	drop %ResultVariableName%
}
rename variable1031206661 %ResultVariableName%
save "%Output%"
