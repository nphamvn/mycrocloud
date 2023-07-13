using MockServer.Shared;

namespace Dapper;
public static class SqlMapperHelper{
    public static void AddTypeHandler<T>(){
        var addedTypeName = new List<string>();
        var type = typeof(T);
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            var jsonColumnAttribute = property.GetCustomAttributes(typeof(JsonColumnAttribute), true);
            if (jsonColumnAttribute != null)
            {
                var propertyType = property.GetType();
                if (!addedTypeName.Contains(propertyType.FullName))
                {
                    var method = typeof(SqlMapperHelper).GetMethod(nameof(SqlMapperHelper.AddJsonTypeHandler));
                    var generic = method.MakeGenericMethod(propertyType);
                    generic.Invoke(null, null);
                    addedTypeName.Add(propertyType.FullName);
                }
            }
        }
    }

    public static void AddTypeHandler(Type T) {

    }

    public static void AddJsonTypeHandler<T>(){
        SqlMapper.AddTypeHandler(new JsonTypeHandler<T>());
    }
}