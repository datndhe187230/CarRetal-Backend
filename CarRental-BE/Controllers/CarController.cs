using CarRental_BE.Models.Common; // ensure ApiResponse
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using CarEntity = CarRental_BE.Models.NewEntities.Car;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Data;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly DbContext _dbContext;

    public CarController(ICarService carService, CarRentalContext dbContext)
    {
        _carService = carService;
        _dbContext = dbContext;
    }

    // PSEUDOCODE:
    // - Create DTOs to shape schema response: TableSchemaDto and ColumnSchemaDto
    // - Open a DB connection from EF Core DbContext (Database.GetDbConnection)
    // - Query INFORMATION_SCHEMA.TABLES to get all base tables (schema + table name)
    // - For each table, query INFORMATION_SCHEMA.COLUMNS to get column details
    // - Map to DTOs and return in ApiResponse
    // - On error, return 500 with message
    [HttpGet]
    [Route("testconnection")]
    public async Task<ActionResult<ApiResponse<List<TableSchemaDto>>>> GetAllCar()
    {
        try
        {
            var conn = _dbContext.Database.GetDbConnection();
            var shouldClose = false;

            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync();
                shouldClose = true;
            }

            var tables = new List<(string Schema, string Name)>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT TABLE_SCHEMA, TABLE_NAME
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_TYPE = 'BASE TABLE'
                    ORDER BY TABLE_SCHEMA, TABLE_NAME;";
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var schema = reader.GetString(0);
                    var table = reader.GetString(1);
                    tables.Add((schema, table));
                }
            }

            var result = new List<TableSchemaDto>(tables.Count);

            foreach (var (schema, table) in tables)
            {
                var columns = new List<ColumnSchemaDto>();
                using var colCmd = conn.CreateCommand();
                colCmd.CommandText = @"
                    SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table
                    ORDER BY ORDINAL_POSITION;";
                var pSchema = colCmd.CreateParameter();
                pSchema.ParameterName = "@schema";
                pSchema.Value = schema;
                colCmd.Parameters.Add(pSchema);

                var pTable = colCmd.CreateParameter();
                pTable.ParameterName = "@table";
                pTable.Value = table;
                colCmd.Parameters.Add(pTable);

                using var colReader = await colCmd.ExecuteReaderAsync();
                while (await colReader.ReadAsync())
                {
                    var name = colReader.GetString(0);
                    var dataType = colReader.GetString(1);
                    var isNullable = string.Equals(colReader.GetString(2), "YES", StringComparison.OrdinalIgnoreCase);
                    int? maxLen = colReader.IsDBNull(3) ? null : colReader.GetInt32(3);

                    columns.Add(new ColumnSchemaDto
                    {
                        Name = name,
                        DataType = dataType,
                        IsNullable = isNullable,
                        MaxLength = maxLen
                    });
                }

                result.Add(new TableSchemaDto
                {
                    Schema = schema,
                    Table = table,
                    Columns = columns
                });
            }

            if (shouldClose)
            {
                await conn.CloseAsync();
            }

            var response = new ApiResponse<List<TableSchemaDto>>(200, "Connection successful", result);
            return Ok(response);
        }
        catch (Exception ex)
        {
            var error = new ApiResponse<List<TableSchemaDto>>(500, $"Connection failed: {ex.Message}", null);
            return StatusCode(500, error);
        }
    }

    [HttpGet("{accountId}/paginated")]
    [Authorize(Roles = "car_owner")]
    public async Task<ApiResponse<PaginationResponse<CarVO_ViewACar>>> GetCarsByAccountId(Guid accountId, [FromQuery] int PageNumber =1, [FromQuery] int PageSize =10)
    {
        try
        {
            var request = new PaginationRequest { PageNumber = PageNumber, PageSize = PageSize };
            var result = await _carService.GetCarsByAccountId(accountId, request);
            return new ApiResponse<PaginationResponse<CarVO_ViewACar>>(200, "Connection successful", result);
        }
        catch
        {
            return new ApiResponse<PaginationResponse<CarVO_ViewACar>>(500, "Connection failed", null);
        }
    }

    [HttpPatch("edit-car/{carId}")]
    [Authorize(Roles = "car_owner")]
    public async Task<ApiResponse<CarVO_Full>> UpdateCar(Guid carId, [FromBody] CarUpdateDTO updateDto)
    {
        try
        {
            var updatedCar = await _carService.UpdateCarEntity(carId, updateDto);
            if (updatedCar == null) return new ApiResponse<CarVO_Full>(404, "Car not found", null);
            var result = await _carService.GetCarVOById(carId);
            return new ApiResponse<CarVO_Full>(200, "Update Success", result);
        }
        catch (Exception ex)
        {
            return new ApiResponse<CarVO_Full>(500, $"Server error: {ex.Message}", null);
        }
    }

    [HttpGet("{carId}/detail")]
    [AllowAnonymous]
    public async Task<ApiResponse<CarVO_CarDetail>> GetCarDetail(Guid carId)
    {
        try
        {
            var carDetail = await _carService.GetCarDetailById(carId);
            if (carDetail == null) return new ApiResponse<CarVO_CarDetail>(404, "Car not found", null);
            return new ApiResponse<CarVO_CarDetail>(200, "Car details retrieved successfully", carDetail);
        }
        catch (Exception ex)
        {
            return new ApiResponse<CarVO_CarDetail>(500, $"Error retrieving car details: {ex.Message}", null);
        }
    }

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<ApiResponse<PaginationResponse<CarSearchVO>>> SearchCar([FromQuery] SearchDTO searchDTO, [FromQuery] int page =1, [FromQuery] int pageSize =10)
    {
        var requestPage = new PaginationRequest { PageNumber = page, PageSize = pageSize };
        var list = await _carService.SearchCar(searchDTO, requestPage);
        return new ApiResponse<PaginationResponse<CarSearchVO>>(200, "Successful", list);
    }

    [HttpPost("add")]
    [Authorize(Roles = "admin, car_owner")]
    public async Task<ApiResponse<CarVO_CarDetail>> AddCar([FromForm] AddCarDTO addCarDTO)
    {
        var newCar = await _carService.AddCar(addCarDTO);
        return new ApiResponse<CarVO_CarDetail>(201, "Car added successfully", newCar);
    }
}

public class TableSchemaDto
{
    public string Schema { get; set; } = default!;
    public string Table { get; set; } = default!;
    public List<ColumnSchemaDto> Columns { get; set; } = new();
}

public class ColumnSchemaDto
{
    public string Name { get; set; } = default!;
    public string DataType { get; set; } = default!;
    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }
}
