set more off
set output proc
// define input %Input1%
// define input %Input2%
// define output %Output%
// define variableNames %VariableNames%
clear
use "%Input1%"
joinby %VariableNames% using "%Input2%"
save "%Output%"
