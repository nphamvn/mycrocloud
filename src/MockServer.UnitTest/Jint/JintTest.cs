using System.Text.Json;
using Jint;

namespace MockServer.UnitTest.Jint;

public class JsonFileAdapter {
    private readonly string _name;
    public JsonFileAdapter(string name)
    {
        _name = name;
    }

    public string read() {
        return JsonSerializer.Serialize(new
        {
            name = "Nam"
        });
    }

    public void write(object value) {
        var json = JsonSerializer.Serialize(value);
        Console.WriteLine(json);
    }
}
public class JintTest
{
    public JsonFileAdapter GetAdapter(string name) {
        return new JsonFileAdapter(name);
    }
    [Fact]
    public void Test()
    {
        var engine = new Engine();
        engine.SetValue("log", new Action<object>(Console.WriteLine));
        engine.SetValue("getAdapter", new Func<string, JsonFileAdapter>(GetAdapter));
        engine.Execute(
            """
            class Db {
                constructor(adapter) {
                    this._adapter = adapter;
                }
                get data() {
                    return this._data;
                }
                set data(value) {
                    this._data = value;
                }
            }
            Db.prototype.read = function() {
                let json = this._adapter.read();
                this._data = JSON.parse(json);
            };
            Db.prototype.write = function() {
                this._adapter.write(this._data);
            };
            """);
        engine.Execute(
            """
            const adapter = getAdapter('tiny_blog');
            const db = new Db(adapter);
            db.read();
            db.data = {name: 'nam'};
            db.write();
            """);
    }

    [Fact]
    public void Test2() {
        // create an instance of the Jint engine
        var engine = new Engine();

        engine.SetValue("log", new Action<object>(Console.WriteLine));
        engine.SetValue("getAdapter", new Func<string, JsonFileAdapter>(GetAdapter));
        engine.Execute(File.ReadAllText("Db.js"));
        // call a method on the instance
        engine.Execute(
            """
            const adapter = getAdapter('tiny_blog');
            const db = new Db(adapter);
            db.read();
            db.data = {name: 'nam'};
            db.write();
            """);
    }
}
