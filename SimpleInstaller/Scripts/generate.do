set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName optional %ResultVariableName% Working
// define string %Expression%
// define token optional %Type%
clear
use "%Input%"
 generate %Type% variable579822704 = %Expression%
capture confirm variable %ResultVariableName%
if (_rc == 0){
	drop %ResultVariableName%
}
rename variable579822704 %ResultVariableName%
save "%Output%"
