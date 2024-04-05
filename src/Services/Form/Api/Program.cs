using System.Text.Json;
using Form.Builder.Api;
using Form.Builder.Api.Entities;
using Form.Builder.Api.Models;
using Microsoft.EntityFrameworkCore;
using FormEntity = Form.Builder.Api.Entities.Form;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .EnableSensitiveDataLogging()
    .LogTo(Console.WriteLine)
    ;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("forms", async (AppDbContext dbContext, FormCreateUpdateRequest formCreateUpdateRequest) =>
{
    await using var trans = await dbContext.Database.BeginTransactionAsync();

    try
    {
        var form = new FormEntity
        {
            Name = formCreateUpdateRequest.Name,
            CreatedAt = DateTime.UtcNow
        };
            
        await dbContext.Forms.AddAsync(form);
        await dbContext.SaveChangesAsync();
        
        var insertedFields = new List<FormField>();
        foreach (var field in formCreateUpdateRequest.Fields)
        {
            insertedFields.Add(new FormField()
            {
                Name = field.Name,
                Type = field.Type,
                FormId = form.Id,
                Details = new FormFieldDetails()
                {
                    TextInput = MapTextInputDetails(field.Details.TextInput),
                    NumberInput = MapNumberInputDetails(field.Details.NumberInput)
                },
                CreatedAt = DateTime.UtcNow
            });
        }
        
        await dbContext.FormFields.AddRangeAsync(insertedFields);
        await dbContext.SaveChangesAsync();
        
        await trans.CommitAsync();
    }
    catch (Exception e)
    {
        await trans.RollbackAsync();
        Console.WriteLine(e);
        throw;
    }
    
    return Results.Created();
});

app.MapPost("forms/{id:int}", async (AppDbContext dbContext, int id, FormCreateUpdateRequest formCreateUpdateRequest) =>
{
    await using var trans = await dbContext.Database.BeginTransactionAsync();
    try
    {
        var form = await dbContext.Forms.SingleAsync(f => f.Id == id);
        form.Name = formCreateUpdateRequest.Name;
        await dbContext.SaveChangesAsync();
        
        var currentFields = await dbContext.FormFields
            .Where(ff => ff.FormId == form.Id)
            .ToListAsync();
        
        var insertedFields = new List<FormField>();
        foreach (var field in formCreateUpdateRequest.Fields)
        {
            if (currentFields.Any(f => f.Id == field.Id))
            {
                // Update
                var currentField = currentFields.Single(f => f.Id == field.Id);
                currentField.Name = field.Name;
                currentField.Type = field.Type;
                currentField.Details.TextInput = MapTextInputDetails(field.Details.TextInput);
                currentField.Details.NumberInput = MapNumberInputDetails(field.Details.NumberInput);
            }
            else
            {
                // Insert
                insertedFields.Add(new FormField()
                {
                    Name = field.Name,
                    Type = field.Type,
                    FormId = form.Id,
                    Details = new FormFieldDetails()
                    {
                        TextInput = MapTextInputDetails(field.Details.TextInput),
                        NumberInput = MapNumberInputDetails(field.Details.NumberInput)
                    }
                });
            }
        }
        
        await dbContext.FormFields.AddRangeAsync(insertedFields);
        await dbContext.SaveChangesAsync();
        
        await trans.CommitAsync();
    }
    catch (Exception e)
    {
        await trans.RollbackAsync();
        Console.WriteLine(e);
        throw;
    }
});

app.MapPost("forms/{id:int}/submit", async (AppDbContext dbContext, int id, List<FormSubmitRequest> formSubmitRequest) =>
{
    var form = await dbContext.Forms.SingleAsync(f => f.Id == id);
    
    var submission = new FormSubmission
    {
        Form = form
    };
    
    var fields = await dbContext.FormFields
        .Where(ff => ff.FormId == id)
        .ToListAsync();
    
    var insertValues = new List<FormFieldValue>();
    var requiredFields = fields.Where(f => f.IsRequired).ToList();
    
    if (requiredFields.Any(f => formSubmitRequest.All(fs => fs.FieldId != f.Id)))
    {
        return Results.BadRequest();
    }
    
    foreach (var field in fields)
    {
        if (formSubmitRequest.Any(f => f.FieldId == field.Id))
        {
            var value = formSubmitRequest.Single(f => f.FieldId == field.Id).Value;

            if (!TryParse(field, (JsonElement)value, out var parsedValue))
            {
                return Results.BadRequest();
            }
            parsedValue.FormSubmission = submission;
            insertValues.Add(parsedValue);
        }
    }
    
    await dbContext.FormFieldValues.AddRangeAsync(insertValues);
    await dbContext.SaveChangesAsync();

    return Results.Ok();
});

app.Run();
return;

bool TryParse(FormField formField, JsonElement o, out FormFieldValue parsedValue)
{
    parsedValue = new FormFieldValue
    {
        Field = formField
    };
    switch (formField.Type)
    {
        case "TextInput":
            if (o.ValueKind != JsonValueKind.String)
            {
                return false;
            }
            
            parsedValue.StringValue = o.GetString();
            break;
        case "NumberInput":
            if (o.ValueKind != JsonValueKind.Number)
            {
                return false;
            }

            parsedValue.StringValue = o.GetInt32().ToString();
            break;
        default:
            return false;
    }
    return true;
}

TextInputDetails? MapTextInputDetails(FormFieldTextInputDetails? detailsTextInput)
{
    if (detailsTextInput is null)
    {
        return null;
    }

    return new()
    {
        MinLength = detailsTextInput.MinLength,
        MaxLength = detailsTextInput.MaxLength
    };
}
NumberInputDetails? MapNumberInputDetails(FormFieldNumberInputDetails? numberInputDetails)
{
    if (numberInputDetails is null)
    {
        return null;
    }

    return new()
    {
        Min = numberInputDetails.Min,
        Max = numberInputDetails.Max
    };
}
