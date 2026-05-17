using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy to allow requests from any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Replace with your Angular app's URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Later, use the following terminal commands with redirection:
// dotnet dev-certs https --trust
// dotnet run --launch-profile "https"
// app.UseHttpsRedirection();

// Use the defined CORS policy
app.UseCors("AllowAngular");

// Hello world endpoint
app.MapGet("/api/hello", () => new { message = "Chicken butt!" })
   .WithName("GetHello");

// Reharmonization endpoint
app.MapGet("/api/jazz/harmonize", (string melodyNote, [FromQuery] int[] degrees) =>
{
    var chromatic = new List<string> { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

    // Non-dominant chord templates
    var templates = new Dictionary<string, Dictionary<int, int>>
    {
        ["Maj7"]  = new() { {1, 0}, {3, 4}, {5, 7}, {7, 11}, {9, 2}, {11, 6}, {13, 9} },
        ["m7"]    = new() { {1, 0}, {3, 3}, {5, 7}, {7, 10}, {9, 2}, {11, 5}, {13, 9} },
        ["m7b5"]  = new() { {1, 0}, {3, 3}, {5, 6}, {7, 10}, {9, 2}, {11, 5}, {13, 8} }, 
        ["mMaj7"] = new() { {1, 0}, {3, 3}, {5, 7}, {7, 11}, {9, 2}, {11, 5}, {13, 9} },
        ["Maj+7"] = new() { {1, 0}, {3, 4}, {5, 8}, {7, 11}, {9, 2}, {11, 6} },          
        ["dim7"]  = new() { {1, 0}, {3, 3}, {5, 6}, {7, 9},  {9, 2}, {11, 5}, {13, 8} }
    };

    int targetIndex = chromatic.IndexOf(melodyNote);
    if (targetIndex == -1)
        return Results.BadRequest($"Invalid melody note: {melodyNote}. Use standard capitalization (e.g., 'Ab').");

    var harmonizationResults = new List<HarmonizationGroup>();

    // Step 1: Iterate through user's requested degrees
    foreach (var targetDegree in degrees)
    {
        var chordsForThisDegree = new List<NonDominantChordDto>();

        // Step 2: Iterate through chord templates
        foreach (var template in templates)
        {
            string quality = template.Key;
            var intervals = template.Value;

            // Step 3: Does this chord contain the target degree?
            if (intervals.TryGetValue(targetDegree, out int semitoneOffset))
            {
                // Step 4: Find root note
                int rootIndex = (targetIndex - semitoneOffset + 12) % 12;
                string rootNote = chromatic[rootIndex];

                // Step 5: Build chord notes
                var fullChordNotes = new List<string>();
                foreach (var interval in intervals.Values)
                {
                    fullChordNotes.Add(chromatic[(rootIndex + interval) % 12]);
                }

                chordsForThisDegree.Add(new NonDominantChordDto($"{rootNote}{quality}", fullChordNotes.ToArray()));
            }
        }

        if (chordsForThisDegree.Any())
        {
            harmonizationResults.Add(new HarmonizationGroup(targetDegree, chordsForThisDegree.ToArray()));
        }
    }

    return Results.Ok(harmonizationResults);
})
.WithName("GetHarmonizationOptions");

// Dominant harmonization endpoint
app.MapGet("/api/jazz/harmonize-dominant", (string melodyNote, [FromQuery] string[] targetDegrees) =>
{
    var chromatic = new List<string> { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

    // Dominant chord templates
    var templates = new Dictionary<string, Dictionary<string, int>>
    {
        ["7"]    = new() { {"1", 0}, {"b9", 1}, {"9", 2}, {"#9", 3}, {"3", 4}, {"#11", 6}, {"5", 7}, {"b13", 8}, {"13", 9}, {"b7", 10} },
        ["7sus"] = new() { {"1", 0}, {"b9", 1}, {"9", 2}, {"#9", 3}, {"b11", 4}, {"4", 5}, {"5", 7}, {"b13", 8}, {"13", 9}, {"b7", 10} },
        ["+7"]   = new() { {"1", 0}, {"b9", 1}, {"9", 2}, {"#9", 3}, {"3", 4}, {"#11", 6}, {"#5", 8}, {"13", 9}, {"b7", 10} }
    };

    int targetIndex = chromatic.IndexOf(melodyNote);
    if (targetIndex == -1) 
        return Results.BadRequest($"Invalid melody note: {melodyNote}. Use standard capitalization.");

    var harmonizationResults = new List<DominantHarmonizationGroup>();
    // Step 1: Iterate through user's requested degrees
    foreach (var targetDegree in targetDegrees)
    {
        var chordsForThisDegree = new List<DominantChordDto>();

        // Step 2: Iterate through chord templates
        foreach (var template in templates)
        {
            string quality = template.Key;
            var intervals = template.Value;

            // Step 3: Does this chord contain the target degree?
            if (intervals.TryGetValue(targetDegree, out int semitoneOffset))
            {
                // Step 4: Find root note
                int rootIndex = (targetIndex - semitoneOffset + 12) % 12;
                string rootNote = chromatic[rootIndex];

                // Step 5: Build chord notes
                var fullChordNotes = new List<string>();
                foreach (var interval in intervals.OrderBy(x => x.Value))
                {
                    fullChordNotes.Add(chromatic[(rootIndex + interval.Value) % 12]);
                }

                chordsForThisDegree.Add(new DominantChordDto($"{rootNote}{quality}", fullChordNotes.Distinct().ToArray()));
            }
        }

        if (chordsForThisDegree.Any())
        {
            harmonizationResults.Add(new DominantHarmonizationGroup(targetDegree, chordsForThisDegree.ToArray()));
        }
    }

    return Results.Ok(harmonizationResults);
})
.WithName("GetDominantHarmonizationOptions");

app.Run();

// DTOs
public record HarmonizationGroup(int TargetDegree, NonDominantChordDto[] Chords);
public record NonDominantChordDto(string Name, string[] Notes);
public record DominantHarmonizationGroup(string TargetDegree, DominantChordDto[] Chords);
public record DominantChordDto(string Name, string[] Notes);