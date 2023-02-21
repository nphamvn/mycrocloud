using AutoMapper;
using MockServer.Core.Databases;
using MockServer.Core.Repositories;
using MockServer.Web.Models.Database;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Services;

public class DatabaseWebService : BaseWebService, IDatabaseWebService
{
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IMapper _mapper;

    public DatabaseWebService(IDatabaseRepository databaseRepository, 
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
    {
        _databaseRepository = databaseRepository;
        _mapper = mapper;
    }
    public Task Create(SaveDatabaseViewModel vm)
    {
        var db = _mapper.Map<Database>(vm);
        return _databaseRepository.Add(AuthUser.Id, db);
    }

    public Task Edit(int id, SaveDatabaseViewModel vm)
    {
        var db = _mapper.Map<Database>(vm);
        return _databaseRepository.Update(AuthUser.Id, db);
    }

    public async Task<IndexViewModel> GetIndexViewModel()
    {
        var vm = new IndexViewModel();
        var dbs = await _databaseRepository.GetAll(AuthUser.Id);
        vm.DatabaseItems = _mapper.Map<IEnumerable<DatabaseItem>>(dbs);
        return vm;
    }

    public async Task<SaveDatabaseViewModel> GetViewModel(int id)
    {
        var db = await _databaseRepository.Get(id);
        return _mapper.Map<SaveDatabaseViewModel>(db);
    }

    public async Task<SaveDatabaseViewModel> GetViewModel(string name)
    {
        var db = await _databaseRepository.Find(AuthUser.Id, name);
        return _mapper.Map<SaveDatabaseViewModel>(db);
    }
}
