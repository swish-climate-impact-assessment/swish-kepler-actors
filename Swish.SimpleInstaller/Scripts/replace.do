set more off
set output proc
// define input %Input%
// define output %Output%
// define string %Condition%
// define string %Value%
clear
use "%Input%"
replace %Value% if %Condition%
save "%Output%"