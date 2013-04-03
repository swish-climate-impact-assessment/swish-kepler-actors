set more off
set output proc
// define input %Input%
// define output %Output%
// define variableNames %VariableNames%
// define string %Format%
clear
use "%Input%"
format %VariableNames% %Format%
save "%Output%"
