// define input %Input%
// define output %Output%
// define variableName optional %VariableName% Working
// define expression %Expression%
// define token optional %Type%
set more off
set output proc
clear
use "%Input%"
 generate %type% variable983718451 = %Expression%
capture confirm variable %VariableName%
if (_rc == 0){
	drop %VariableName%
}
rename variable983718451 %VariableName%
save "%Output%"
