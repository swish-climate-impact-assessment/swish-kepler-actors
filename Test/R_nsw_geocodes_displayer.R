require(swishdbtools)
require(oz)
require(maps)
tempShapeFileName <- dir("/tmp", pattern = ".shp", full.names=T)[1]
locations <- read_file(tempShapeFileName)
# make a default map
m <- matrix(c(2,1,1,1),2,2)
layout(m, widths=c(1, 2),heights=c(1.5, 2))
# layout.show(2)
par(cex = 1.2, mar = c(2,1,1,1))
y1 <- -37.0
y2 <- -28.0
x1 <- 144.0
x2 <- 160.0
ndg  <- .5
oz(states = T, sections= 4, ylim = c(y1-ndg, y2+ndg), xlim=c(x1-ndg,x2+ndg))
plot(add=T, pch = 16, locations,cex = .5)
box()
#axis(1);axis(2)
text(locations, 
     labels = locations@data$address,
     adj = c(0,0), cex = .6
     )
map.scale(155, ratio=F)  
legend('topright', pch = 16, c("Study locations"))
# inset
oz()
polygon(c(105,165,165,105),c(-8,-8,-46,-46), col = "white")
oz(add=T)
xdj <- 0; ydj <- 0
polygon(c(x1-xdj,x2+xdj,x2+xdj,x1-xdj),c(y1+ydj,y1+ydj,y2-ydj,y2-ydj), lwd = 2)
box()
