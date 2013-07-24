set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName %VariableName%
clear
use "%Input%"
 generate variable440539507 = date(%VariableName%, "DMY")
capture confirm variable variable440539506
if (_rc == 0){
	drop variable440539506
}
rename variable440539507 variable440539506
format variable440539506 %td
capture confirm variable %VariableName%
if (_rc == 0){
	drop %VariableName%
}
rename variable440539506 %VariableName%
save "%Output%"
