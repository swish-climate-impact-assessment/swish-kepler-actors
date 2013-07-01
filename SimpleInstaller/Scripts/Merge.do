set more off
set output proc
// define input %Input1%
// define input %Input2%
// define output %Output%
// define variableNames %Variables%
// define temporaryFile %TemporaryFile%
clear
use "%Input2%"
sort %Variables%, stable
save "%TemporaryFile%"
clear
clear
use "%Input1%"
sort %Variables%, stable
merge %Variables%, using "%TemporaryFile%"
drop _merge
sort %Variables%, stable
save "%Output%"
