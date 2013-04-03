set more off
set output proc
// define input %Input1%
// define input %Input2%
// define output %Output%
// define variableNames %VariableNames%
// define temporaryFile %TemporaryFile%
clear
use "%Input2%"
save "C:\Users\Ian\AppData\Local\Temp\tmp8B03.tmp2.dta"
clear
use "%Input1%"
append using "C:\Users\Ian\AppData\Local\Temp\tmp8B03.tmp2.dta"
save "%Output%"
