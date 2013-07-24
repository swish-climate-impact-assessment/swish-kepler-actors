set more off
set output proc
// define input %Input%
// define output %Output%
// define string %Command%
clear
use "%Input%"
%Command%
save "%Output%"
