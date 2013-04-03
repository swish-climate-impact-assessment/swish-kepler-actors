set more off
set output proc
// define input %Input%
// define output %Output%
// define expression %Expression%
clear
use "%Input%"
keep if %Expression%
save "%Output%"
