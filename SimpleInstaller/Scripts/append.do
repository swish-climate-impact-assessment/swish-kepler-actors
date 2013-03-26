// define input %Input1%
// define input %Input2%
// define output %Output%
// define variableNames %VariableNames%
// define temporaryFile %TemporaryFile%
set more off
set output proc
clear
use "%Input2%"
save "C:\Users\u5265691\AppData\Local\Temp\tmpF7E1.tmp2.dta"
clear
use "%Input1%"
append using "C:\Users\u5265691\AppData\Local\Temp\tmpF7E1.tmp2.dta"
save "%Output%"
