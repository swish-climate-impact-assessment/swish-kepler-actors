set more off
set output proc
// define input %Input%
// define output %Output%
// define variableName %Variable
clear
use "%Input%"
 generate variable2022098960 = date(%Variable, "DMY")
capture confirm variable variable2022098959
if (_rc == 0){
	drop variable2022098959
}
rename variable2022098960 variable2022098959
format variable2022098959 %td
tsset variable2022098959
tsfill
capture confirm variable %Variable
if (_rc == 0){
	drop %Variable
}
rename variable2022098959 %Variable
save "%Output%"
