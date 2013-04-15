# copy actors to kepler
basedir <- "SimpleInstaller/Actors"
filelist <- dir(basedir)
for(fname in filelist)
{
  file.copy(file.path(basedir,fname), file.path("~/KeplerData/workflows/MyWorkflows/", fname))
}