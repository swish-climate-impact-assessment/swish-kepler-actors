##Connects to Postgres database server and performs SQL subset
# db <- "ewedb"
# table <- "weather_sla.weather_sla"
# select_cols <- "date, sla_code, maxave"
# subset_by <- "sla_code = 105453800 AND year = 2009"
#limit <- "1"

require(swishdbtools)
ch <- connect2postgres2(db)
if(!exists("evaluate")){evaluate <- TRUE}
if(!exists("select_cols")){select_cols <- "*"}
if(!exists("limit")){limit <- -1}

if(!exists("subset_by"))
{
  df <- sql_subset(ch, x = table, 
                   select = select_cols, 
                   limit = limit, eval = evaluate
  )
} else {
  df <- sql_subset(ch, x = table, 
                   subset = subset_by, select = select_cols, 
                   limit = limit, eval = evaluate
  )
}
tempFileName <- tempfile("foo", tmpdir = Sys.getenv("TEMP"), fileext = ".csv")
write.csv(df, tempFileName, row.names=FALSE)
tempFileName
