# copy actors to kepler
destination <- "~/KeplerData/workflows/MyWorkflows/"
filelist <- dir(destination)
for(fname in filelist)  
{
  file.remove(file.path(destination, fname))
}
basedir <- "SimpleInstaller/Actors"
filelist <- dir(basedir)
for(fname in filelist)  
{
  file.copy(file.path(basedir,fname), file.path(destination, fname))
}
basedir <- "SimpleInstaller/SampleData"
filelist <- dir(basedir)
destination <- "~/KeplerData/MyData"
for(fname in filelist)  
{
  file.copy(file.path(basedir,fname), file.path(destination, fname))
}