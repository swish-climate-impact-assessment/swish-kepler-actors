// define input %Input%
// define output %Output%
// define variableNames %VariableNames%
set more off
set output proc
clear
use "%Input%"
drop %VariableNames%
save "%Output%"
