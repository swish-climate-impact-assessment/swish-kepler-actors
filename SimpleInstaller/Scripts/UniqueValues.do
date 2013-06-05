set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName %VariableName%
clear
use "%Input%"
keep %VariableName%
by %VariableName%, sort: gen variable920022982=_n
keep if variable920022982==1
capture confirm variable variable920022982
if (_rc == 0){
	drop variable920022982
}
save "%Output%"
