set more off
set output proc
// define input %Input%
// define output %Output%
// define variableNames %Variables%
clear
use "%Input%"
drop %Variables%
save "%Output%"
