using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connection factory
builder.Services.AddScoped<IDbConnection>(_ =>
{
    var cs = builder.Configuration.GetConnectionString("Sql") 
             ?? "Server=localhost;Database=Ostra;Trusted_Connection=True;TrustServerCertificate=True;";
    return new SqlConnection(cs);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Simple health endpoint
app.MapGet("/", () => new { ok = true, name = "FrontaPrace.Api" });

// Workplaces (distinct from view)
app.MapGet("/api/workplaces", async (IDbConnection db) =>
{
    var sql = """
        SELECT DISTINCT WpId, WpCode, WpName
        FROM dbo.VIEWMANTA_WorkQueue WITH (NOLOCK)
        ORDER BY WpCode
    """;
    var items = await db.QueryAsync(sql);
    return Results.Ok(items);
});

// Paged WorkQueue
app.MapGet("/api/workqueue", async (IDbConnection db, int? wpId, string? search, int page = 1, int pageSize = 50) =>
{
    page = Math.Max(1, page);
    pageSize = Math.Clamp(pageSize, 1, 200);

    var where = "WHERE 1=1";
    var dyn = new DynamicParameters();
    if (wpId.HasValue)
    {
        where += " AND WpId = @wpId";
        dyn.Add("wpId", wpId.Value);
    }
    if (!string.IsNullOrWhiteSpace(search))
    {
        where += " AND (ProductOrderCode LIKE @s OR OperationName LIKE @s OR ProductName LIKE @s OR ProductCode LIKE @s)";
        dyn.Add("s", $"%{search}%");
    }

    var sql = $@"
        SELECT *
        FROM dbo.VIEWMANTA_WorkQueue WITH (NOLOCK)
        {where}
        ORDER BY PriorityOperation DESC, HelpOrder ASC, Id DESC
        OFFSET {(page-1)*pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY;

        SELECT COUNT(1) FROM dbo.VIEWMANTA_WorkQueue WITH (NOLOCK) {where};
    ";

    using var multi = await db.QueryMultipleAsync(sql, dyn);
    var rows = (await multi.ReadAsync()).ToList();
    var total = await multi.ReadFirstAsync<int>();
    return Results.Ok(new { total, page, pageSize, items = rows });
});

// RFID verify (example: check by OperationBarCode or RFID fields)
app.MapPost("/api/workqueue/rfid-check", async (IDbConnection db, RfidCheckRequest req) =>
{
    var sql = """
        SELECT TOP 1 Id, ProductOrderCode, OperationName, RFID
        FROM dbo.VIEWMANTA_WorkQueue WITH (NOLOCK)
        WHERE (OperationBarCode = @code OR RFID = @code)
        ORDER BY Id DESC
    """;

    var row = await db.QueryFirstOrDefaultAsync(sql, new { code = req.Code });
    if (row is null) return Results.NotFound(new { message = "Nenalezeno." });
    return Results.Ok(row);
});

app.Run();

public record RfidCheckRequest(string Code);