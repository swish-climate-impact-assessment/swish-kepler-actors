// define input %Input%
// define output %Output%
// define variableNames %VariableNames%
// define temporaryFile %TemporaryFile%
// define string %Format%
set more off
set output proc
clear
use "%Input%"
format %VariableNames% %Format%
save "%Output%"
