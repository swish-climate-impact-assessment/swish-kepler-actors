# kepler_subset
# expected to work like base R subset
#?subset
head(subset(x=airquality, subset = Temp > 80, select = c(Ozone, Temp)))
