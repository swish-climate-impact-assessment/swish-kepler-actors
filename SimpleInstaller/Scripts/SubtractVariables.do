set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName %LeftVariable%
// define variableName %RightVariable%
// define variableName optional %ResultVariableName% Working
clear
use "%Input%"
 generate double variable1721871084 = %LeftVariable% - %RightVariable%
capture confirm variable %ResultVariableName%
if (_rc == 0){
	drop %ResultVariableName%
}
rename variable1721871084 %ResultVariableName%
save "%Output%"
