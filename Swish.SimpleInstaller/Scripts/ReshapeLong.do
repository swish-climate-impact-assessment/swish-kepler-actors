set more off
set output proc
// define input %Input%
// define output %Output%
// define token %VariableNamePrefix%
// define token %I%
// define token %J%
clear
use "%Input%"
reshape long %VariableNamePrefix% , i(%I%) j(%J%)
save "%Output%"
