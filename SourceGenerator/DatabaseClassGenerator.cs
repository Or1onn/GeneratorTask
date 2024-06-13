using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Text;


namespace SourceGenerator;

public class DatabaseClassGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var connectionString = "Host=localhost;Database=teestdb;Username=postgres;Password=admin";
        var classes = GetClassNamesFromDatabase(connectionString);

        foreach (var className in classes)
        {
            var source = GenerateClassSource(className);
            context.AddSource($"{className}.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private List<string> GetClassNamesFromDatabase(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var compiler = new PostgresCompiler();
        var db = new QueryFactory(connection, compiler);

        var classNames = db.Query("ClassNames").Select("ClassName").Get<string>().ToList();
        return classNames;
    }

    private string GenerateClassSource(string className)
    {
        var source = $@"
            namespace GeneratedClasses
            {{
                public class {className}
                {{
                    public long Id {{ get; set; }}
                }}
            }}";
        return source;
    }
}