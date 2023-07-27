namespace Dapper;
public static class SqlMapperHelper
{
    public static void AddJsonTypeHandler<T>()
    {
        SqlMapper.AddTypeHandler(new JsonTypeHandler<T>());
    }
}