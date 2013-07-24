set more off
set output proc
// define input %Input%
// define output %Output%
// define string %Expression%
clear
use "%Input%"
keep if %Expression%
save "%Output%"
