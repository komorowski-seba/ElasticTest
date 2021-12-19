### Migration ###

### add ###
dotnet ef --startup-project ../ElasticTest migrations add init_db -o Persistence/Migrations -c ApplicationDbContext