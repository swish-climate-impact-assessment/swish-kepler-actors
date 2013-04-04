# kepler_subset
# expected to work like base R subset
#?subset
head(subset(airquality, subset = Temp > 80, select = c(Ozone, Temp)))
