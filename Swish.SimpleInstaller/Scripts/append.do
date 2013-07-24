set more off
set output proc
// define input %Input1%
// define input %Input2%
// define output %Output%
clear
use "%Input1%"
append using "%Input2%"
save "%Output%"
