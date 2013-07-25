################################################################
# man-R_sql_subset
# R-Kepler actor connects to Postgres database server and performs SQL subset
# returns a CSV, to tmp if unspecified
################################################################
# R-R_sql_subset
R_sql_subset  <- function(ch, table_name,
                          subset_where, select_cols, limit,
                          outfile)
{
  df <- sql_subset(ch, x = table_name,
                   subset = subset_where, select = select_cols,
                   limit = limit, eval = TRUE
                   )
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
# these are the ports on the Kepler actor, if empty defaults are chosen
if(!exists("db")){db <- "ewedb"}
if(!exists("table_name")){table_name <- "public.dbsize"}
if(!exists("subset_where")){subset_where  <- NA}
if(!exists("select_cols")){select_cols <- "*"}
if(!exists("limit")){limit <- -1}
if(!exists("outfile")){outfile <- NA}
if(!exists("evaluate")){evaluate <- TRUE}
# evaluate? if not just return the filename
if(evaluate)
  {
require(swishdbtools)
  ch <- connect2postgres2(db)
  output_file  <- R_sql_subset(ch, table_name,  subset_where, select_cols,
                               limit, outfile
                               )
} else {
  if(!is.na(outfile))
    {
      output_file  <- outfile
    } else {
      output_file  <- "Temporary Filename not recorded"
    }
}
output_file
