using MockServer.Core.Databases;
using MockServer.Core.Repositories;

public class NoSqlAdapter : DatabaseAdapter
{
    private readonly int _dbId;
    private readonly IDatabaseRepository _databaseRespository;
    public NoSqlAdapter(int dbId, IDatabaseRepository databaseRespository,
                DatabaseAdapterOptions options) : base(options)
    {
        _dbId = dbId;
        _databaseRespository = databaseRespository;
    }

    public override string ReadJson()
    {
        var task = _databaseRespository.Get(_dbId);
        task.Wait();
        var db = task.Result;
        return db.Data;
    }

    public override void Write(object value)
    {
        var json = GetJson(value);
        var task = _databaseRespository.UpdateData(_dbId, json);
        task.Wait();
    }
}