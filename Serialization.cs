using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using PolyJson;
using static dotnet7_research.SerializationPolymorhicDotnetExample;

namespace dotnet7_research;


public class SerializationPolymorhicPolyJsonExample
{
    [PolyJsonConverter("_t")]
    [PolyJsonConverter.SubType(typeof(Dog), "dog")]
    [PolyJsonConverter.SubType(typeof(Cat), "cat")]
    public abstract class AnimalBase
    {
        public abstract string Sound { get; }
    }

    public class Cat : AnimalBase
    {
        public override string Sound => "Mew";
    }

    public class Dog : AnimalBase
    {
        public override string Sound => "Gav";
    }

    public class Container
    {
        public AnimalBase Animal { get; set; }
    }

    public static void RunIt()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        AnimalBase animal1 = new Cat();
        AnimalBase animal2 = new Dog();
        Console.WriteLine($"Sound: {animal1.Sound}");
        Console.WriteLine($"Sound: {animal2.Sound}");

        var serialized = JsonSerializer.Serialize<AnimalBase>(animal1, options);

        var container = new Container();
        container.Animal = animal2;

        var serialized2 = JsonSerializer.Serialize(container);

        Console.WriteLine(serialized);
    }
}

public class SerializationPolymorhicDotnetExample
{
    [JsonDerivedType(typeof(Animal), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(Cat), typeDiscriminator: "cat")]
    [JsonDerivedType(typeof(Dog), typeDiscriminator: "dog")]
    public class Animal
    {
        public string Sound => "...";
    }

    public class Cat : Animal
    {
        public string Sound => "Mew";
    }

    public class Dog : Animal
    {
        public string Sound => "Gav";
    }

    public static void RunIt()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        Animal animal = new Dog();
        Console.WriteLine($"Sound: {animal.Sound}");

        var serialized = JsonSerializer.Serialize<Animal>(animal, options);
        Console.WriteLine(serialized);
    }
}


public class SerializationContractExample
{
    // Custom attribute to annotate the property
    // we want to be incremented.
    [AttributeUsage(AttributeTargets.Property)]
    class SerializationCountAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    private class JsonIncludePrivateFieldsAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    private class JsonIncludePrivateFieldAttribute : Attribute { }

    [JsonIncludePrivateFields]
    // Example type to serialize and deserialize.
    private class Product
    {
        [JsonIncludePrivateField]
        private string _secondName;

        public string Name { get; set; } = "";

        [SerializationCount]
        public int RoundTrips { get; set; }

        public Product(string secondName)
        {
            _secondName = secondName;
        }

        public Product()
        {
            _secondName = null!;
        }
    }



    // Custom modifier that increments the value
    // of a specific property on deserialization.
    static void IncrementCounterModifier(JsonTypeInfo typeInfo)
    {
        foreach (var propertyInfo in typeInfo.Properties)
        {
            if (propertyInfo.PropertyType != typeof(int)) continue;

            var serializationCountAttributes = propertyInfo.AttributeProvider?.GetCustomAttributes(typeof(SerializationCountAttribute), true) ?? Array.Empty<object>();
            var attribute = serializationCountAttributes.Length == 1 ? (SerializationCountAttribute)serializationCountAttributes[0] : null;

            if (attribute != null)
            {
                var setProperty = propertyInfo.Set;
                if (setProperty is not null)
                {
                    propertyInfo.Set = (obj, value) =>
                    {
                        // Increment the value by 1.
                        value = (int?)value + 1;
                        setProperty(obj, value);
                    };
                }
            }
        }
    }

    static void AddPrivateFieldsModifier(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object) return;

        if (!jsonTypeInfo.Type.IsDefined(typeof(JsonIncludePrivateFieldsAttribute), inherit: false)) return;

        foreach (var field in jsonTypeInfo.Type
                     .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                     .Where(a => a.IsDefined(typeof(JsonIncludePrivateFieldAttribute))))
        {
            var jsonPropertyInfo = jsonTypeInfo.CreateJsonPropertyInfo(field.FieldType, field.Name);
            jsonPropertyInfo.Get = field.GetValue;
            jsonPropertyInfo.Set = field.SetValue;

            jsonTypeInfo.Properties.Add(jsonPropertyInfo);
        }
    }

    public static void RunIt()
    {
        var product = new Product("bzz")
        {
            Name = "Aquafresh"
        };

        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { AddPrivateFieldsModifier, IncrementCounterModifier }
            }
        };

        // First serialization and deserialization.
        var serialized = JsonSerializer.Serialize(product, options);
        Console.WriteLine(serialized);
        // {"Name":"Aquafresh","RoundTrips":0}

        var deserialized = JsonSerializer.Deserialize<Product>(serialized, options)!;
        Console.WriteLine($"{deserialized.RoundTrips}");
        // 1

        // Second serialization and deserialization.
        serialized = JsonSerializer.Serialize(deserialized, options);
        Console.WriteLine(serialized);
        // { "Name":"Aquafresh","RoundTrips":1}

        deserialized = JsonSerializer.Deserialize<Product>(serialized, options)!;
        Console.WriteLine($"{deserialized.RoundTrips}");
        // 2
    }
}

/*
private class StringPartitioner : Partitioner<string>
{
    private readonly IEnumerable<string> _data;

    public StringPartitioner(IEnumerable<string> data)
    {
        _data = data;
    }

    public override bool SupportsDynamicPartitions => false;

    public override IList<IEnumerator<string>> GetPartitions(int partitionCount)
    {
        var result = new List<IEnumerator<string>>(2)
        {
            CreateEnumerator(true),
            CreateEnumerator(false)
        };
        return result;
    }

    private IEnumerator<string> CreateEnumerator(bool isEven)
    {
        foreach (var d in _data)
        {
            if (d.Length % 2 == 0 && isEven) yield return d;
            else if (d.Length % 2 == 1 && !isEven) yield return d;
        }
    }
}
*/