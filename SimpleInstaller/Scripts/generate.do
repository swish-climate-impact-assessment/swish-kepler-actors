set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName optional %VariableName% Working
// define expression %Expression%
// define token optional %Type%
clear
use "%Input%"
 generate %type% variable1847210191 = %Expression%
capture confirm variable %VariableName%
if (_rc == 0){
	drop %VariableName%
}
rename variable1847210191 %VariableName%
save "%Output%"
