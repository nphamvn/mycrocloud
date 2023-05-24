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
Db.prototype.read = function () {
  let json = this._adapter.ReadJson();
  this._data = JSON.parse(json);
};
Db.prototype.write = function () {
  this._adapter.Write(this._data);
};