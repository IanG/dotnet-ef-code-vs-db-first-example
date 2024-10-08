# .NET EF Database-first vs Code-first Example

## Introduction

This repository provides examples of how you can use .NET Entity Framework from two different perspectives:

- **DB First** - Start with an existing database schema and build entity classes into a DbContext From it
- **Code First** - Start with entity classes and create the BbContext and database tables from them

## The Database

For this example we are using [PostgreSQL](https://www.postgresql.org/) as the database hosted inside a [Docker](https://www.docker.com/) container.

The `docker-compose.yml` file will create a Postgres container and then continue to create the 2 databases used in this example.

- dbfirst
- codefirst

### The `etc/docker-entrypoint-initdb.d` Directory.

This directory contains a pair of `.sql` files which will be used when the container is initialised to create the above two databases.

| Script | Description                                                                                                                                                                                                                          |
|--------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
|01-create-dbfirst-db.sql        | This script creates the `dbfirst` database and its associated user `dbfirstuser`.  This script also creates the tables which we'll use to create entities from when we scaffold the db context.                                      |
|02-create-codefirst-db.sql| This script creates the `codefirst` database with the user `codefirstuser`.  This destination database will be populated with tables from our code-first entities when we create the initial migration and apply it to the database. |

### Starting The Postgres Database Container

Ensure you have Docker running and bring up the Postgres instance by running the following command from the root of the cloned repository:

```
docker compose up -d
```
You should now be able to connect to the Postgres instance using your SQL client of choice or you can access them via the web-based `pgAdmin` instance that runs alongside the database on [http://localhost:5050](http://localhost:5050/browser/) (**Note** the hostname to the database in the pgAdmin container is `db`)

### Stopping The Database Container

There are 2 ways you can stop the database container depending upon what you want to happen to the data.
#### Stopping & Keeping your Data
If you want to stop the container and keep your data run:

```
docker compose down
```

#### Stopping & Removing Containers, Volumes and Data

```
docker compose down --volumes
```

## Entity Framework

For both the database-first and code-first approaches we'll be using [Entity Framework](https://learn.microsoft.com/en-us/ef/).  This is an Object Relational Mapper (ORM) which provides an abstraction from the database so we can think more about our data in terms of sets/collections of related objects rather the underlying database structure and the mechanics of SQL.

### Ensuring EF Tools Are Installed And Up To Date.

Make sure the .NET Entity Framework Tools are installed and up to date.  To install/update the Entity Framework tools use:

```
dotnet tool update --global dotnet-ef
```

Once you have done this you can check the version of the .NET Entity Framework Tools by running:

```
dotnet ef -version
```

## Database-First with Entity Framework.

### Introduction

When we use the term "database-first" with Entity Framework, what we mean is that a database already exists with a populated schema and possibly data.   We want to use Entity Framework to be able to manipulate the data held in this existing database by creating EF entities and DbSets from the existing tables.  This is a typical approach if we are:

- Integrating with an existing system with an established database
- Building a replacement for an existing system where the existing database needs to remain

### Scaffolding the DbContext and Entities from the existing database schema

In the `Common` project we already have:

`Common/Data/DBFirst/DbFirstDbContext.cs`

This is nothing more than an empty class that extends `DbContext` and connects up to the database.  If you look in:

`Common/Data/EFExampleDataServiceBuilderExtensions.cs`

You will see that `DbFirstDbContext` is added into the service collection and connects to the Postgres database with the connection string `DBFirstDb` defined in `appsettings.json`.

The database `dbfirst` has been created in the docker container and already contains a series of tables and data that we want to be able to manipulate via Entity Framework.  If you connect to the `dbfirst` database and run:

```
SELECT
    *
FROM
    information_schema.tables
WHERE
    table_catalog = 'dbfirst'
    AND table_schema = 'public'
```

You will notice the following tables exist:

- customers
- orders
- order_items
- products
- product_suppliers
- suppliers

### Creating Entities and Updating DbFirstDbContext via Scaffolding

We can create the EF entity classes that represent the above tables with the following command 

```
dotnet ef dbcontext scaffold "Name=ConnectionStrings:DBFirstDb" Npgsql.EntityFrameworkCore.PostgreSQL --startup-project API --project Common --output-dir Data/DBFirst/Entities --context-dir Data/DBFirst --context DbFirstDbContext --schema public --force
```

When we run this command we should see the following output:

```
Build started...
Build succeeded.
```

Now when we look at `Common/Data/DBFirst/DbFirstDbContext.cs` we should see that it now has `DBSet<T>`'s which represent the tables that existed in this database and in the `OnModelCreating` method you will see that it binds the attributes of the entity classes onto the columns of the database.  We should also see that the command has created a bunch of class files in `Common/Data/DBFirst/Entities` - these are the classes which are associated with the `DBSet<T>`'s in `DbFirstDbContext.cs` and we can now manipulate data in the database with EF.

#### A Thing Of Note Regarding Identity Columns..

Something to be aware of.  If we look at the DDL commands that create the tables in `etc/docker-entrypoint-initdb.d/01-create-dbfirst-db.sql` you'll notice the `customers` table has the following definition for its `id` column:

```
id INTEGER PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY NOT NULL,
```
So, this means this is an auto-generated identity/sequence value that is generated by the database upon insert.  The EF scaffold process has not adhered to this when it created the mapping:

```
entity.Property(e => e.Id).HasColumnName("id");
```

If we use the entity and create a new Customer with something like:

```
Customer customer = new Customer() { FirstName = "First", LastName = "Last", Email = "first.last@test.com", Phone = "666555"};
        
_dbContext.Customers.Add(customer);
await _dbContext.SaveChangesAsync();
```

The row still gets its new identity value created.   If we wanted to be a bit more explicit about this we'd have to modify the lines that define identity columns to look like this:

```
entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
```
### Enabling the code in the API Project's Controller `DbFirstCustomersController`

Now we have scaffolded our dbContext and Entities we can un-comment the code in this controller.  Run the project and you can now use the `API.http` file to make requests against the endpoint and manipulate the customers in the database.  You can also use the Swagger page that opens in your browser when you launch the project.

## Code-First with Entity Framework.

### Introduction

When we use the term "code-first" with Entity Framework, what we mean is that we have already designed and modeled all our Entity classes and DbSets in C# code.  What we want to do next is create the table objects within a database so we can store these entities.  Typical cases when a code-first approach is taken are:

- When building a brand-new system with no existing database
- Developers want to have the ORM (in this case Entity Framework) do all the heavy lifting and create the database.
- Developers do not maybe have the in-depth knowledge of RDBMS systems are not comfortable with a database-first approach.

### Creating the database objects from an existing DbContext and Entities

In the `Common` project we already have `Common/Data/CodeFirst/CodeFirstDbContext.cs` and in `Common/Data/CodeFirst/Entitles` we have a pre-built set of Entity classes.

Examine the classes and you will see they have benn annotated to guide EF regarding things like:

- Column naming and ordering
- Datatypes
- Lengths

You will also notice there is additional logic in the `OnModelCreating` method of `CodeFirstDbContext`.  This is to ensure things like:

- Unique keys for fields get created
- Foreign keys exist between tables
- Default values for fields are specified

### Creating the Initial EF Migration

Now we need to create the initial EF migration based upon the `CodeFirstDbContext` and Entities we have defined.  You can do this with the following command:

```
dotnet ef migrations add InitialCreate --startup-project API --project Common --output-dir Data/CodeFirst/Migrations --context CodeFirstDbContext
```
You will see the following output:

```
Build started...
Build succeeded.
Done. To undo this action, use 'ef migrations remove'
```
We can query EF to get a list of migrations.  Lets do that now with:

```
dotnet ef migrations list --startup-project API --project Common --context CodeFirstDbContext
```
Which should give us the following output:

```
Build started...
Build succeeded.
??????????????_InitialCreate (Pending)
```
The migration we have just created shows `Pending` as it has not yet been applied to the database.

### Applying the `InitialCreate` Migration to our `codefirst` Database

To populate the `codefirst` database with tables that represent our entities we can now run the following:

```
dotnet ef database update --startup-project API --project Common --context CodeFirstDbContext
```
You will now see the following output:

```
Build started...
Build succeeded.
Done.
```
This has now applied the migration `InitialCreate` to the database.  If we run:

```
dotnet ef database update --startup-project API --project Common --context CodeFirstDbContext
```

You'll see our migration is no longer pending.


If you take a look in the `public` schema of the `codefirst` database you should now see we have tables representing our Entity classes.  The schema objects that exist should match those already present in the `dbfirst` database.

#### The `__EFMigrationsHistory` Table

If you take a look in the `public` schema of the `codefirst` database you will also notice that there is an additional table called `__EFMigrationsHistory`.  This table is used by Entity Framework to track which migrations have been applied to the database with `dotnet ef database update` commands.  You can read more about that [here](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/history-table).

### Testing the `codefirst` Database with `CodeFirstCustomersController`

You can now use the `CodeFirstCustomersController` and the Swagger page to test the `CodeFirstDbContext` or you can use the `API.http` file.

## Conclusion

In this example we have taken a look at both approaches for using Entity Framework.  We have shown we can start fromm an existing database or we can start with entities and create the equivalent database.

I hope this has maybe served as a useful reference.  Thanks for looking.