set more off
set output proc
// define input %Input%
// define output %Output%
// define token %VariableNamePrefix%
// define token %I%
// define token %J%
clear
use "%Input%"
reshape wide %VariableNamePrefix% , i(%I%) j(%J%)
save "%Output%"
