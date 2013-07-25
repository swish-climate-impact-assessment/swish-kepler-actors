################################################################
# man-R_sql_subset
# R-Kepler actor connects to Postgres database server and performs SQL subset
# returns a CSV, to tmp if unspecified

################################################################
# R-R_sql_subset
R_sql_subset  <- function(ch,
                          select_cols, limit,
                          subset_where, outfile)
{
  if(is.na(subset_where))
  {
    df <- sql_subset(ch, x = table,
                     select = select_cols,
                     limit = limit, eval = TRUE
    )
  } else {
    df <- sql_subset(ch, x = table,
                     subset = subset_where, select = select_cols,
                     limit = limit, eval = TRUE
    )
  }
  if(is.na(outfile))
  {
    tempFileName <- tempfile("foo", tmpdir = Sys.getenv("TEMP"),
                             fileext = ".csv")
  } else {
    tempFileName <- outfile
  }
  write.csv(df, tempFileName, row.names=FALSE)
  return(tempFileName)
}
################################################################
# test-R_sql_subset
# these are the ports on the Kepler actor, if empty defaults should
# work on a standard postgis db
if(!exists("db"){db <- "ewedb"}
if(!exists("table")){table <- "public.spatial_ref_sys"}
if(!exists("select_cols")){select_cols <- "srid, srtext"}
if(!exists("subset_where")){subset_where <- "srid = 4283"}
if(!exists("limit")){limit <- "10"}
if(!exists("select_cols")){select_cols <- "*"}
if(!exists("limit")){limit <- -1}
if(!exists("subset_where")){subset_where  <- NA}
if(!exists("outfile")){outfile <- NA}

if(!exists("evaluate")){
require(swishdbtools)
ch <- connect2postgres2(db)
#debug(R_sql_subset)
output_file  <- R_sql_subset(ch, select_cols, limit, subset_where, outfile)
}
output_file
