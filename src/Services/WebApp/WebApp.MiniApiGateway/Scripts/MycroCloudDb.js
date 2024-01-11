class MycroCloudDb {
    constructor(connectionString) {
        this.adapter = new DbConnection(connectionString);
    }
    
    connect() {
        this.adapter.Connect();
    }
    
    read() {
        const { data } = this.adapter.Read();
        this.data = data;
    }
    
    write() {
        this.adapter.Write(this.data);
    }
}