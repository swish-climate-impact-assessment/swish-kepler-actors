set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName optional %Variable% Working
// define string %Expression%
// define token optional %Type%
clear
use "%Input%"
 generate %Type% variable579822704 = %Expression%
capture confirm variable %Variable%
if (_rc == 0){
	drop %Variable%
}
rename variable579822704 %Variable%
save "%Output%"
