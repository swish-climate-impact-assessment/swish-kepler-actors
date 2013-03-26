// define input %Input%
// define output %Output%
// define string %Condition%
// define string %Value%
set more off
set output proc
clear
use "%Input%"
replace %Value% if %Condition%
save "%Output%"
