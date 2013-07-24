# copy actors to kepler
# NB for Ian's windows installer need to:
# add new actor to actors dir in /simpleinstaller
# copy to /SWISHKeplerActorsInstallerDDMMMMYYYY/Bin
# along with the installer exe stuff
# make shortcuts
# zip up and should work

destination <- "~/KeplerData/workflows/MyWorkflows/"
filelist <- dir(destination)
for(fname in filelist)
{
  file.remove(file.path(destination, fname))
}
basedir <- "Swish.SimpleInstaller/Actors"
filelist <- dir(basedir)
for(fname in filelist)
{
  filelist
  file.copy(file.path(basedir,fname), file.path(destination, fname))
}
basedir <- "SimpleInstaller/SampleData"
filelist <- dir(basedir)
destination <- "~/KeplerData/MyData"
for(fname in filelist)
{
  file.copy(file.path(basedir,fname), file.path(destination, fname))
}
