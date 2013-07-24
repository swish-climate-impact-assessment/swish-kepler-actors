##Connects to Postgres database server and performs SQL subset
# db <- "ewedb"
# table <- "weather_sla.weather_sla"
# select_cols <- "date, sla_code, maxave"
# subset_by <- "sla_code = 105453800 AND year = 2009"
#limit <- "10"

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
