using System.Data.SqlClient;
using BenchmarkDotNet.Engines;

namespace dotnet7_research;

public static class RefLocals
{
    public static void RunIt()
    {
        var b = true;
        ref var rb = ref b;

        Console.WriteLine(b);
        Console.WriteLine(rb);

        rb = false;

        Console.WriteLine(b);
        Console.WriteLine(rb);

        var t = (rb, "a");
        rb = true;

        Console.WriteLine(t);
        Console.WriteLine(b);
        Console.WriteLine(rb);

        var host = "172.21.224.133";
        string? subHost = (string?)"SQL102";

        var hostFinal = $"{host}";
        if (subHost != null) hostFinal += @$"\{subHost}";

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = hostFinal,
            UserID = "username",
            Password = "password",
            InitialCatalog = "database",
            Encrypt = false,
        };
        Console.WriteLine(builder.ConnectionString);
    }
}