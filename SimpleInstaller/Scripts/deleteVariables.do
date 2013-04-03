set more off
set output proc
// define input %Input%
// define output %Output%
// define variableNames %VariableNames%
clear
use "%Input%"
drop %VariableNames%
save "%Output%"
