set more off
set output proc
// define input %Input%
// define output %Output%
// define variableNames %Variables%
// define string %Format%
clear
use "%Input%"
format %Variables% %Format%
save "%Output%"
