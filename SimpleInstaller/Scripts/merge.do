// define input %Input1%
// define input %Input2%
// define output %Output%
// define variableNames %VariableNames%
// define temporaryFile %TemporaryFile%
set more off
set output proc
clear
use "%Input2%"
sort %VariableNames%, stable
save "%TemporaryFile%"
clear
clear
use "%Input1%"
sort %VariableNames%, stable
merge %VariableNames%, using "%TemporaryFile%"
sort %VariableNames%, stable
save "%Output%"
