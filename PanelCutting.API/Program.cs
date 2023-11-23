using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using PanelCutting;
using PanelCutting.API.Dtos;
using PanelCutting.API.Dtos.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

const double panelWidth = 500;

builder.Services.AddSingleton<ProfileCutter>(_ => new ProfileCutter(panelWidth));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/cut-profile", CutProfile)
    .WithName(nameof(CutProfile))
    .WithOpenApi();

app.Run();

Results<Ok<ProfileCutDto>, ValidationProblem, ProblemHttpResult> CutProfile(ProfileDto request, ProfileCutter panelCutter)
{
    List<Point> vertices = request.Profile.Select(pointDto => pointDto.ToDomainObject()).ToList();
    var profile = Profile.Create(vertices, out Exception? exception);
    if (exception != null)
    {
        if (exception is ValidationException)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Profile", new[] { exception.Message } }
            });
        }

        return TypedResults.Problem(exception.ToString());
    }

    if (profile == null)
        return TypedResults.Problem();
    
    IEnumerable<PanelCut> result = panelCutter.Cut(profile);
    return TypedResults.Ok(new ProfileCutDto
    {
        Cuts = result.Select(cut => cut.ToDto()).ToList()
    });
}