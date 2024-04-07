using System.Security.Claims;
using System.Text.Json;
using Form.Builder.Api;
using Form.Builder.Api.Entities;
using Form.Builder.Api.Extensions;
using Form.Builder.Api.Models;
using Microsoft.EntityFrameworkCore;
using FormEntity = Form.Builder.Api.Entities.Form;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Authentication:Schemes:Auth0JwtBearer:Authority"];
        options.Audience = builder.Configuration["Authentication:Schemes:Auth0JwtBearer:Audience"];
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .EnableSensitiveDataLogging()
    .LogTo(Console.WriteLine)
    ;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigins"]!.Split(','))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("forms", async (ClaimsPrincipal user, AppDbContext dbContext) =>
{
    var forms = await dbContext.Forms
        .Where(f => f.UserId == user.GetUserId())
        .AsNoTracking()
        .ToListAsync();
        
    return Results.Ok(forms);
}).RequireAuthorization();

app.MapGet("forms/{id:int}", async (ClaimsPrincipal user, AppDbContext dbContext, int id) =>
{
    // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
    var form = await dbContext.Forms
        .Include(f => f.Fields)
        .SingleAsync(f => f.Id == id && f.UserId == user.GetUserId());
        
    return Results.Ok(new
    {
        form.Id,
        form.Name,
        form.Description,
        form.CreatedAt,
        Fields = form.Fields.Select(f => new
        {
            f.Id,
            f.Name,
            f.Type,
            Details = new
            {
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                f.Details.TextInput,
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                f.Details.NumberInput
            }
        })
    });
}).RequireAuthorization();

app.MapPost("forms", async (ClaimsPrincipal user, AppDbContext dbContext, FormCreateUpdateRequest formCreateUpdateRequest) =>
{
    var form = new FormEntity
    {
        UserId = user.GetUserId(),
        Name = formCreateUpdateRequest.Name,
        Description = formCreateUpdateRequest.Description,
        CreatedAt = DateTime.UtcNow
    };
        
    foreach (var field in formCreateUpdateRequest.Fields)
    {
        var newField = new FormField
        {
            Name = field.Name,
            Type = field.Type,
            FormId = form.Id,
            Details = new FormFieldDetails
            {
                TextInput = MapTextInputDetails(field.Details.TextInput),
                NumberInput = MapNumberInputDetails(field.Details.NumberInput),
                DropdownDetails = MapDropdownDetails(field.Details.Dropdown)
            },
            CreatedAt = DateTime.UtcNow
        };

        if (newField.Type == "Dropdown")
        {
            newField.SelectListItems = field.Details.Dropdown!.SelectListItems.Select(i => new SelectListItem
            {
                Id = i.Id,
                Text = i.Text
            }).ToList();
        }
        
        form.Fields.Add(newField);
    }
        
    await dbContext.Forms.AddAsync(form);
    await dbContext.SaveChangesAsync();
    
    return Results.Created(form.Id.ToString(), new { form.Id, form.Name, form.CreatedAt });
}).RequireAuthorization();

app.MapPut("forms/{id:int}", async (ClaimsPrincipal user, AppDbContext dbContext, int id, FormCreateUpdateRequest formCreateUpdateRequest) =>
{
    var form = await dbContext.Forms
        .Include(f => f.Fields)
        .SingleAsync(f => f.Id == id && f.UserId == user.GetUserId());
        
    form.Name = formCreateUpdateRequest.Name;
    form.UpdatedAt = DateTime.UtcNow;
        
    foreach (var field in formCreateUpdateRequest.Fields)
    {
        if (form.Fields.Any(f => f.Id == field.Id))
        {
            // Update
            var currentField = form.Fields.Single(f => f.Id == field.Id);
            currentField.Name = field.Name;
            currentField.Type = field.Type;
            currentField.Details.TextInput = MapTextInputDetails(field.Details.TextInput);
            currentField.Details.NumberInput = MapNumberInputDetails(field.Details.NumberInput);
            currentField.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Insert
            form.Fields.Add(new FormField
            {
                Name = field.Name,
                Type = field.Type,
                FormId = form.Id,
                Details = new FormFieldDetails
                {
                    TextInput = MapTextInputDetails(field.Details.TextInput),
                    NumberInput = MapNumberInputDetails(field.Details.NumberInput)
                },
                CreatedAt = DateTime.UtcNow
            });
        }
    }
        
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
}).RequireAuthorization();

app.MapGet("forms/{id:int}/public", async (AppDbContext dbContext, int id) =>
{
    // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
    var form = await dbContext.Forms
        .Include(f => f.Fields)
        .SingleAsync(f => f.Id == id);

    return Results.Ok(new
    {
        form.Id,
        form.Name,
        form.Description,
        form.CreatedAt,
        Fields = form.Fields.Select(f => new
        {
            f.Id,
            f.Name,
            f.Type,
            Details = new
            {
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                f.Details.TextInput,
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                f.Details.NumberInput
            }
        })
    });
});

app.MapPost("forms/{id:int}/submissions", async (AppDbContext dbContext, int id, List<FormSubmitRequest> values) =>
{
    var form = await dbContext.Forms
        .Include(f => f.Fields)
        .SingleAsync(f => f.Id == id);
    
    if (form.Fields.Any(f => values.All(fs => fs.FieldId != f.Id)))
    {
        return Results.BadRequest();
    }
    
    var submission = new FormSubmission
    {
        Form = form,
        CreatedAt = DateTime.UtcNow
    };
    
    foreach (var field in form.Fields)
    {
        if (values.All(f => f.FieldId != field.Id)) continue;
 
        var value = values.Single(f => f.FieldId == field.Id).Value;

        if (!TryParse(field, (JsonElement)value, out var parsedValue))
        {
            return Results.BadRequest();
        }
        
        submission.Values.Add(parsedValue);
    }
    
    await dbContext.FormSubmissions.AddAsync(submission);
    await dbContext.SaveChangesAsync();

    return Results.Ok();
});

app.MapGet("forms/{id:int}/submissions", async (AppDbContext dbContext, int id, string? q = null) =>
{
    var submissions = await dbContext.FormSubmissions
        .Include(s => s.Values).ThenInclude(formSubmissionFieldValue => formSubmissionFieldValue.Field)
        .Where(s => s.Form.Id == id)
        .AsNoTracking()
        .ToListAsync();

    return Results.Ok(submissions.Select(s => new
    {
        s.Id,
        s.CreatedAt,
        Values = s.Values.Select(v =>
        {
            return new
            {
                FieldId = v.Field.Id,
                Value = Map(v)
            };

            static object? Map(FormSubmissionFieldValue formSubmissionFieldValue)
            {
                return formSubmissionFieldValue.Field.Type switch
                {
                    "TextInput" => formSubmissionFieldValue.StringValue ?? null,
                    "NumberInput" => (formSubmissionFieldValue.StringValue is not null
                        ? int.Parse(formSubmissionFieldValue.StringValue)
                        : null),
                    _ => null
                };
            }
            
        })
    }));
    
    
});

app.Run();
return;

bool TryParse(FormField formField, JsonElement value, out FormSubmissionFieldValue parsedValue)
{
    parsedValue = new FormSubmissionFieldValue
    {
        Field = formField
    };
    switch (formField.Type)
    {
        case "TextInput":
            if (value.ValueKind != JsonValueKind.String)
            {
                return false;
            }
            
            parsedValue.StringValue = value.GetString();
            break;
        case "NumberInput":
            if (value.ValueKind != JsonValueKind.Number)
            {
                return false;
            }

            parsedValue.StringValue = value.GetInt32().ToString();
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

DropdownDetails? MapDropdownDetails(FormFieldDropdownDetails? detailsDropdown)
{
    if (detailsDropdown is null)
    {
        return null;
    }

    return new DropdownDetails()
    {
        
    };
}
